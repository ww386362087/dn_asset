#include "XEntity.h"
#include "XBeHitComponent.h"
#include "XAIComponent.h"
#include "XSkillComponent.h"
#include "Transform.h"
#include "XAttributes.h"
#include "XStateMachine.h"

EntyCallBack eCallback;
CompCallBack cCallback;


XEntity::~XEntity()
{
	Unload();
	OnUnintial();
	delete _pmachine;
	_attr = NULL;
	_object = NULL;
	_transf = NULL;
	_pmachine = NULL;
}

void XEntity::Update(float delta)
{
	for (std::unordered_map<uint, XComponent*>::iterator it = components.begin();it!=components.end();it++)
	{
		it->second->Update(delta);
	}
}


void XEntity::AttachToHost()
{
}

void XEntity::DetachFromHost()
{
}

void XEntity::Initilize(GameObject* go, XAttributes* attr)
{
	XObject::Initilize();
	_object = go;
	_transf = go->transform;
	_attr = attr;
	XObject::AttachComponent<XAIComponent>();
	OnInitial();
}

void XEntity::OnInitial() {}

void XEntity::OnUnintial() {}

bool XEntity::IsBoss()
{
	return (_eEntity_Type & Boss) != 0;
}

bool XEntity::IsNpc()
{
	return (_eEntity_Type & Npc) != 0;
}

bool XEntity::IsRole()
{
	return (_eEntity_Type & Role) != 0;
}

bool XEntity::IsPlayer()
{
	return (_eEntity_Type & Player) != 0;
}

void XEntity::MoveForward(Vector3 forward)
{
	_forward = forward;
	_force_move = true;
}

void XEntity::StopMove()
{
	_force_move = false;
	_forward = Vector3::zero;
}

void XEntity::OnDied()
{
	_state = XState_Death;
}

bool XEntity::IsDead()
{
	return _state == XState_Death;
}

EntityType XEntity::GetType()
{
	return _eEntity_Type;
}

XSkillMgr* XEntity::SkillManager()
{
	XSkillComponent* pskill = GetComponent<XSkillComponent>();
	return pskill ? pskill->SkillManager() : NULL;
}


bool XEntity::Valide(XEntity* e)
{
	return e && e->_attr && !e->IsDead();
}

XEntity* XEntity::ValidEntity(uint id)
{
	if (id == 0) return NULL;
	return XEntityMgr::Instance()->GetEntity(id);
}

Vector3 XEntity::getPostion()
{
	if (_transf)
	{
		return _transf->position;
	}
	else
	{
		return Vector3::zero;
	}
}

GameObject* XEntity::getEntityObject()
{
	return _object;
}

Transform* XEntity::getTransfer()
{
	return _transf;
}

Vector3 XEntity::getForward()
{
	return _transf->forward;
}

XAttributes* XEntity::getAttributes()
{
	return _attr;
}

void XEntity::setAttributes(XAttributes* attr)
{
	_attr = attr;
}

XStateDefine XEntity::getState()
{
	return _state;
}