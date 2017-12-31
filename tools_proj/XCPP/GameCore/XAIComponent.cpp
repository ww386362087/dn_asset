#include "XAIComponent.h"
#include "XEntity.h"
#include "AITreeArg.h"
#include "XAttributes.h"
#include "XPlayer.h"

XAIComponent::XAIComponent()
{
	state = FRAME;
}

XAIComponent::~XAIComponent()
{
}

void XAIComponent::OnInitial(XObject* _obj)
{
	XComponent::OnInitial(_obj);
	_entity = dynamic_cast<XEntity*>(_obj);
	InitVariables();
	InitTree();
}

void XAIComponent::OnUninit()
{
	_entity = 0;
	_target = 0;
	_tick = 0;
	_tree = 0;
	XComponent::OnUninit();
}


void XAIComponent::EventSubscribe()
{
	XComponent::EventSubscribe();
	XDelegate start = DelCALLBACK(XAIComponent, OnStartSkill, this);
	XDelegate end = DelCALLBACK(XAIComponent, OnStartSkill, this);
	RegisterEvent(XEvent_AIStartSkill, &start);
	RegisterEvent(XEvent_AIEndSkill, &end);
}

bool XAIComponent::OnStartSkill(IArgs* e, void*)
{
	XAIStartSkillEventArgs* startSkill = (XAIStartSkillEventArgs*)e;
	if (startSkill->IsCaster)
	{
		_is_hurt_oppo = false;
		_is_casting_skill = true;
		_cast_skillid = startSkill->SkillId;
		_tree->SetVariable(AITreeArg::SkillID, _cast_skillid);
		_tree->SetVariable(AITreeArg::IsHurtOppo, _is_hurt_oppo);
		_tree->SetVariable(AITreeArg::IsCastingSkill, _is_casting_skill);
	}
	return true;
}

bool XAIComponent::FindTargetByDistance(float dist, float angle)
{
	std::vector<XEntity*> list = XEntityMgr::Instance()->GetAllEnemy(_entity);
	targets.clear();
	for (int i = 0; i < list.size(); i++)
	{
		XEntity* enemy = list[i];
		if (XEntity::Valide(enemy))
		{
			Vector3 v = enemy->getPostion() - _entity->getPostion();
			float distance = Vector3::sqrtMagnitude(v);
			if (distance < dist * dist)
			{
				float targetangle = Vector3::Angle(v, _entity->getForward());
				if (targetangle < angle)
				{
					targets.push_back(enemy);
				}
			}
		}
	}
	return targets.size() > 0;
}

bool XAIComponent::ResetTarget()
{
	targets.clear();
	_target = NULL;
	_tree->ResetVariable(AITreeArg::TARGET);
	return true;
}

bool XAIComponent::DoSelectNearest()
{
	float near = 1 << 10;
	for (int i = 0; i < targets.size(); i++)
	{
		Vector3 v = _entity->getPostion() - targets[i]->getPostion();
		float dis = v.magnitude();
		if (dis < near)
		{
			_target = targets[i];
			near = dis;
		}
	}
	SetTarget(_target);
	return true;
}

bool XAIComponent::DoSelectFarthest()
{
	float far = 0;
	for (int i = 0; i < targets.size(); i++)
	{
		float dis = (_entity->getPostion()- targets[i]->getPostion()).magnitude();
		if (dis > far)
		{
			_target = targets[i];
			far = dis;
		}
	}
	SetTarget(_target);
	return true;
}

bool XAIComponent::DoSelectRandom()
{
	if (targets.size() > 0)
	{
		int cnt = (int)targets.size();
		int randx = Random(0, cnt);
		_target = targets[randx];
		SetTarget(_target);
	}
	return true;
}

bool XAIComponent::OnEndSkill(IArgs* e, void*)
{
	XAIEndSkillEventArgs* endSkill = (XAIEndSkillEventArgs*)e;

	if (endSkill->IsCaster)
	{
		//if ((int)_entity->SkillManager()->GetDashIdentity() != endSkill->SkillId)
		{
			_is_casting_skill = false;
			_cast_skillid = 0;
			_tree->SetVariable(AITreeArg::SkillID, _is_casting_skill);
		}
	}
	return true;
}

void XAIComponent::OnUpdate(float delta)
{
	XComponent::OnUpdate(delta);
	if (_tick > 0 && _tree)
	{
		_timer += delta;
		if (_timer >= _tick)
		{
			OnTickAI();
			_timer = 0;
		}
	}
}

void XAIComponent::SetTarget(XEntity* target)
{
	if (target)
	{
		_target = 0;
	}
	else
	{
		_target = target;
		if (XEntity::Valide(_target))
		{
			Vector3 diff = _entity->getPostion() - _target->getPostion();
			_target_distance = diff.magnitude();
			_tree->SetVariable(AITreeArg::TargetDistance, _target_distance);
			_tree->SetVariable(AITreeArg::TARGET, _target->getEntityObject());
		}
	}
}

void XAIComponent::InitTree()
{
	_tree = new AITree();
	const char* tree = "";
	if (_entity->IsPlayer())
	{
		tree = "PlayerAutoFight";
	}
	else
	{
		XAttributes* _attr = _entity->getAttributes();
		tree = _attr->AiBehavior;
	}
	SetBehaviorTree(tree);
}

void XAIComponent::SetBehaviorTree(const char* tree)
{
	_is_inited = true;
	_tree->Initial(_entity);
	_tree->EnableBehaviourTree(true);
	_tree->SetBehaviourTree(tree);
	_tick = _ai_tick * _tick_factor;
}

void XAIComponent::InitVariables()
{
	XAttributes* attr = _entity->getAttributes();
	_normal_attack_prob = attr->NormalAttackProb;
	_enter_fight_range = attr->EnterFightRange;
	_tick = attr->AIActionGap;
	_is_fixed_in_cd = attr->IsFixedInCD;
	_fight_together_dis = attr->FightTogetherDis;
}

void XAIComponent::OnTickAI()
{
	if (_is_inited && _tree && XEntity::Valide(_entity))
	{
		UpdateVariable();
		SetTreeVariable(_tree);
		_tree->TickBehaviorTree();
	}
}

void XAIComponent::UpdateVariable()
{
	if (_entity->getAttributes())
	{
		_max_hp = (float)_entity->getAttributes()->GetAttr(XAttr_MaxHP_Total);
		_current_hp = (float)_entity->getAttributes()->GetAttr(XAttr_CurrentHP_Total);
		_max_super_armor = (float)_entity->getAttributes()->GetAttr(XAttr_MaxSuperArmor_Total);
		_current_super_armor = (float)_entity->getAttributes()->GetAttr(XAttr_CurrentSuperArmor_Total);
	}
	XPlayer* player = XEntityMgr::Instance()->Player;
	Vector3 v1 = _entity->getPostion();
	Vector3 v2 = player->getPostion();
	if (XEntity::Valide((XEntity*)player))
	{
		Vector3 diff = v1 - v2;
		_master_distance = diff.magnitude();
	}
	if (_target  && XEntity::Valide(_target))
	{
		_target_distance = (v1 - v2).magnitude();
		//_target_distance -= _target.Radius;
		if (_target_distance < 0) _target_distance = 0;
		Vector3 oDirect1 = v1 - v2;
		//_target_rotation = Mathf.Abs(XCommon.singleton.AngleWithSign(oDirect1, _target.Forward));
	}
	else
	{
		_target_distance = 100000.0f;
		_target_rotation = 0;
	}
}


void XAIComponent::SetTreeVariable(AITree* tree)
{
	XPlayer* player = XEntityMgr::Instance()->Player;
	tree->SetVariable(AITreeArg::TARGET, _target ? _target->getEntityObject() : 0);
	tree->SetVariable(AITreeArg::MASTER, player ? player->getEntityObject() : 0);
	tree->SetVariable(AITreeArg::IsOppoCastingSkill, _is_oppo_casting_skill);
	tree->SetVariable(AITreeArg::IsHurtOppo, _is_hurt_oppo);
	tree->SetVariable(AITreeArg::TargetDistance, _target_distance);
	tree->SetVariable(AITreeArg::MasterDistance, _master_distance);
	tree->SetVariable(AITreeArg::IsFixedInCD, _is_fixed_in_cd);
	tree->SetVariable(AITreeArg::NormalAttackProb, _normal_attack_prob);
	tree->SetVariable(AITreeArg::EnterFightRange, _enter_fight_range);
	tree->SetVariable(AITreeArg::FightTogetherDis, _fight_together_dis);
	tree->SetVariable(AITreeArg::MaxHP, _max_hp);
	tree->SetVariable(AITreeArg::CurrHP, _current_hp);
	tree->SetVariable(AITreeArg::MaxSupperArmor, _max_super_armor);
	tree->SetVariable(AITreeArg::CurrSuperArmor, _current_super_armor);
	tree->SetVariable(AITreeArg::EntityType, (int)_entity->GetType());
	tree->SetVariable(AITreeArg::TargetRot, _target_rotation);
	tree->SetVariable(AITreeArg::AttackRange, _attack_range);
	tree->SetVariable(AITreeArg::MinKeepRange, _min_keep_range);
	tree->SetVariable(AITreeArg::IsCastingSkill, _is_casting_skill);
	tree->SetVariable(AITreeArg::IsFighting, _is_fighting);
	tree->SetVariable(AITreeArg::IsQteState, _is_qte_state);
}


bool XAIComponent::getIsCastingSkill()
{
	return _is_casting_skill;
}

bool XAIComponent::getIsOppoCastingSkill()
{
	return _is_oppo_casting_skill;
}

bool XAIComponent::getIsFixedInCd()
{
	return _is_fixed_in_cd;
}


bool XAIComponent::getIsHurtOppo()
{
	return _is_hurt_oppo;
}


float XAIComponent::getEnterFightRange()
{
	return _enter_fight_range;
}

