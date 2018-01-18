#include "AITreeImpleted.h"
#include "../XEntityMgr.h"
#include "../Transform.h"
#include "../XEntity.h"
#include "../XAIComponent.h"
#include "../XAttributes.h"
#include "../XNavigationComponent.h"

#define CHECKENTY(entity) if (!XEntity::Valide(entity)) return Failure;


XEntity* AITreeImpleted::Transform2Entity(Transform* t)
{
	uint id = 0;
	std::stringstream ss(t->name);
	ss >> id;
	return XEntityMgr::Instance()->GetEntity(id);
}

AIStatus AITreeImpleted::NavToTargetUpdate(XEntity* entity, GameObject* mAIArgTarget, GameObject* mAIArgNavTarget, Vector3 mAIArgNavPos)
{
	CHECKENTY(entity);
	if (mAIArgTarget)
	{
		if (NavToTarget(entity, mAIArgTarget))
			return Success;
		else
			return Failure;
	}
	else
	{
		if (mAIArgNavTarget)
		{
			if (NavToTarget(entity, mAIArgNavTarget))
				return Success;
			else
				return Failure;
		}
		else
		{
			if (mAIArgNavPos == Vector3::zero)
				return Failure;
			else
			{
				if (ActionNav(entity, mAIArgNavPos))
					return Success;
				else
					return Failure;
			}
		}
	}
}

AIStatus AITreeImpleted::StopNavMoveUpdate(XEntity* entity)
{
	CHECKENTY(entity);
	if (StopNavMove(entity)) return Success;
	return Failure;
}

AIStatus AITreeImpleted::RotateToTargetUpdate(XEntity* entity, GameObject* mAIArgTarget, float ang)
{
	CHECKENTY(entity);
	if (RotateToTarget(entity->getTransfer(), mAIArgTarget, ang)) return Success;
	return Failure;
}

AIStatus AITreeImpleted::RotateSelfUpdate(XEntity* entity, float max, float min)
{
	CHECKENTY(entity);
	float ang = Random((int)(min * 100), (int)(max * 100)) / 100.0f;
	//Vector3 dir = XCommon.singleton.HorizontalRotateVetor3(entity.Forward, ang);
	//entity.EntityTransfer.forward = dir;
	return Success;
}

AIStatus AITreeImpleted::MoveForwardUpdate(XEntity* entity)
{
	CHECKENTY(entity);
	entity->MoveForward(entity->getTransfer()->forward);
	return Success;
}

AIStatus AITreeImpleted::DetectEnemyInSightUpdate(XEntity* entity)
{
	CHECKENTY(entity);
	if (DetectEnemyInSight(entity))
		return Success;
	return Failure;
}

AIStatus AITreeImpleted::FindTargetByDistanceUpdate(XEntity* entity, float mAIArgDistance, float mAIArgAngle)
{
	CHECKENTY(entity);
	if (entity->GetComponent<XAIComponent>()->FindTargetByDistance(mAIArgDistance, mAIArgAngle))
	{
		return Success;
	}
	return Failure;
}

AIStatus AITreeImpleted::ResetTargetUpdate(XEntity* entity)
{
	CHECKENTY(entity);
	if (entity->GetComponent<XAIComponent>()->ResetTarget())
		return Success;
	return Failure;
}

AIStatus AITreeImpleted::DoSelectNearestUpdate(XEntity* entity)
{
	CHECKENTY(entity);
	if (entity->GetComponent<XAIComponent>()->DoSelectNearest())
		return Success;
	else
		return Failure;
}

AIStatus AITreeImpleted::DoSelectFarthestUpdate(XEntity* entity)
{
	CHECKENTY(entity);
	if (entity->GetComponent<XAIComponent>()->DoSelectFarthest())
		return Success;
	else
		return Failure;
}

AIStatus AITreeImpleted::DoSelectRandomTargetUpdate(XEntity* entity)
{
	CHECKENTY(entity);
	if (entity->GetComponent<XAIComponent>()->DoSelectRandom())
		return Success;
	else
		return Failure;
}

AIStatus AITreeImpleted::CalDistanceUpdate(XEntity* entity, Transform* mAIArgObject, float mAIArgDistance, Vector3 mAIArgDestPoint)
{
	CHECKENTY(entity);
	if (mAIArgObject)
	{
		Vector3 v = entity->getPostion() - mAIArgObject->position;
		mAIArgDistance = v.magnitude();
	}
	else
	{
		mAIArgDistance = (entity->getPostion() - mAIArgDestPoint).magnitude();
	}
	return Success;
}

AIStatus AITreeImpleted::SelectMoveTargetByIdUpdate(XEntity* entity, Transform* mAIArgMoveTarget, int mAIArgObjectId)
{
	CHECKENTY(entity);
	Transform* moveTarget = SelectMoveTargetById(entity, mAIArgObjectId);
	if (moveTarget)
	{
		mAIArgMoveTarget = moveTarget;
		return Success;
	}
	return Failure;
}

AIStatus AITreeImpleted::ValueHPUpdate(XEntity* entity, int mAIArgMaxHP, int mAIArgMinHP)
{
	CHECKENTY(entity);
	double hp = entity->getAttributes()->GetAttr(XAttr_CurrentHP_Basic);
	if (hp >= mAIArgMinHP && hp <= mAIArgMaxHP)
		return Success;
	else
		return Failure;
}

AIStatus AITreeImpleted::ValueMPUpdate(XEntity* entity, int mAIArgMaxMP, int mAIArgMinMP)
{
	CHECKENTY(entity);
	double hp = entity->getAttributes()->GetAttr(XAttr_CurrentMP_Basic);
	if (hp >= mAIArgMinMP && hp <= mAIArgMaxMP)
		return Success;
	else
		return Failure;
}

AIStatus AITreeImpleted::ValueTargetUpdate(XEntity* entity, Transform* mAIArgTarget)
{
	CHECKENTY(entity);
	return Success;
}

AIStatus AITreeImpleted::ValueDistanceUpdate(XEntity* entity, GameObject* mAIArgTarget, float mAIArgMaxDistance)
{
	CHECKENTY(entity);
	float dis = Vector3::sqrtMagnitude(entity->getPostion() - mAIArgTarget->transform->position);
	if (mAIArgTarget &&dis <= mAIArgMaxDistance * mAIArgMaxDistance)
		return Success;
	else
		return Failure;
}

AIStatus AITreeImpleted::StatusRandomUpdate(XEntity* entity, int prob)
{
	CHECKENTY(entity);
	if (prob < 0 || prob > 100)
	{
		return Failure;
	}
	else
	{
		int randx = Random(0, 100);
		if (randx >= prob)
			return Success;
		else return Failure;
	}
}

AIStatus AITreeImpleted::IsOppoCastingSkillUpdate(XEntity* entity)
{
	CHECKENTY(entity);
	if (entity->GetComponent<XAIComponent>()->getIsOppoCastingSkill())
		return Success;
	else
		return Failure;
}

AIStatus AITreeImpleted::IsHurtOppoUpdate(XEntity* entity)
{
	CHECKENTY(entity);
	if (entity->GetComponent<XAIComponent>()->getIsHurtOppo())
		return Success;
	else
		return Failure;
}

AIStatus AITreeImpleted::IsFixedInCdUpdate(XEntity* entity)
{
	CHECKENTY(entity);
	if (entity->GetComponent<XAIComponent>()->getIsFixedInCd())
		return Success;
	else
		return Failure;
}

AIStatus AITreeImpleted::IsCastingSkillUpdate(XEntity* entity)
{
	CHECKENTY(entity);
	if (entity->GetComponent<XAIComponent>()->getIsCastingSkill())
		return Success;
	else
		return Failure;
}

AIStatus AITreeImpleted::StatusIdleUpdate(XEntity* entity)
{
	CHECKENTY(entity);
	if (entity->getState() == XState_Idle)
		return Success;
	else
		return Failure;
}

AIStatus AITreeImpleted::StatusMoveUpdate(XEntity* entity)
{
	CHECKENTY(entity);
	if (entity->getState() == XState_Move)
		return Success;
	else
		return Failure;
}

AIStatus AITreeImpleted::StatusBehitUpdate(XEntity* entity)
{
	CHECKENTY(entity);
	if (entity->getState() == XState_BeHit)
		return Success;
	else
		return Failure;
}

AIStatus AITreeImpleted::StatusDeathUpdate(XEntity* entity)
{
	CHECKENTY(entity);
	if (entity->getState() == XState_Death)
		return Success;
	else
		return Failure;
}

AIStatus AITreeImpleted::StatusFreezeUpdate(XEntity* entity)
{
	CHECKENTY(entity);
	if (entity->getState() == XState_Freeze)
		return Success;
	else
		return Failure;
}

AIStatus AITreeImpleted::StatusSkillUpdate(XEntity* entity)
{
	CHECKENTY(entity);
	if (entity->getState() == XState_Skill)
		return Success;
	else
		return Failure;
}

AIStatus AITreeImpleted::LogUpdate(XEntity* entity, std::string str, bool b)
{
	if (b) ERR(str);
	else LOG(str);
	return Success;
}

AIStatus AITreeImpleted::WaitUpdate(XEntity* entity, float wait, bool rand, float min, float max)
{
	CHECKENTY(entity);
	return Success;

}

AIStatus AITreeImpleted::XAIActionSkillUpdate(XEntity* entity, std::string scr, GameObject* target)
{
	CHECKENTY(entity);
	return Success;
}

AIStatus AITreeImpleted::RandomCompareUpdate(XEntity* entity, int prob)
{
	CHECKENTY(entity);
	int randx = Random(0,100);
	if (prob > randx)
	{
		return Success;
	}
	else
	{
		return Failure;
	}
}

Transform* AITreeImpleted::SelectMoveTargetById(XEntity* entity, int objectid)
{
	std::vector<XEntity*> ens = XEntityMgr::Instance()->GetAllEnemy(entity);
	for (size_t i = 0, max = ens.size(); i < max; i++)
	{
		if (XEntity::Valide(ens[i]) && ens[i]->getAttributes()->getTypeID() == objectid)
		{
			return ens[i]->getTransfer();
		}
	}
	return 0;
}

bool AITreeImpleted::ActionNav(XEntity* entity, Vector3 dest)
{
	XNavigationComponent* nav = entity->GetComponent<XNavigationComponent>();
	if (nav)
	{
		nav->Navigate(dest);
		entity->MoveForward(dest - entity->getPostion());
		return true;
	}
	return false;
}

bool AITreeImpleted::NavToTarget(XEntity* entity, GameObject* target)
{
	if (target)
	{
		XNavigationComponent* nav = entity->GetComponent<XNavigationComponent>();
		if (nav)
		{
			nav->Navigate(target->transform->position);
			entity->MoveForward(target->transform->position - entity->getPostion());
			return true;
		}
	}
	return false;
}

bool AITreeImpleted::StopNavMove(XEntity* entity)
{
	if (entity == NULL) return false;
	XNavigationComponent* nav = entity->GetComponent<XNavigationComponent>();
	if (nav) nav->NavEnd();
	return true;
}

bool AITreeImpleted::RotateToTarget(Transform* src, GameObject* target, float ang)
{
	if (src  && target)
	{
		Vector3 dir = target->transform->position - src->position;
		//dir = XCommon.singleton.HorizontalRotateVetor3(dir, ang);
		src->forward = dir;
		return true;
	}
	return false;
}

bool AITreeImpleted::DetectEnemyInSight(XEntity* e)
{
	std::vector<XEntity*> opponent = XEntityMgr::Instance()->GetAllEnemy(e);
	for (int i = 0; i < opponent.size(); i++)
	{
		if (!XEntity::Valide(opponent[i])) continue;
		Vector3 dir = opponent[i]->getPostion() - e->getPostion();
		float distance = Vector3::sqrtMagnitude(dir);
		//一旦在视野范围，就激活仇恨列表
		if (distance < e->getAttributes()->EnterFightRange * e->getAttributes()->EnterFightRange)
		{
			XAIComponent* ai = opponent[i]->GetComponent<XAIComponent>();
			ai->SetTarget(e);
			return true;
		}
	}
	return false;
}