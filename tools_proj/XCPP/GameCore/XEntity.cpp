#include "XEntity.h"
#include "XBeHitComponent.h"
#include "XAIComponent.h"
#include "XSkillComponent.h"
#include "Transform.h"

EntyCallBack eCallback;
CompCallBack cCallback;

void XEntity::Update(float delta)
{
}

void XEntity::LateUpdate()
{
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

	XObject::AttachComponent<XSkillComponent>();
	XObject::AttachComponent<XAIComponent>();
	XAIComponent* ai = XObject::GetComponnet<XAIComponent>();
	XBeHitComponent* hit = XObject::GetComponnet<XBeHitComponent>();
	LOG("AI:" + tostring(ai == 0) + " hit: " + tostring(hit == NULL));
	XObject::DetachComponent<XAIComponent>();
	ai = XObject::GetComponnet<XAIComponent>();
	LOG("AI:" + tostring(ai == 0));

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
	XSkillComponent* pskill = GetComponnet<XSkillComponent>();
	return pskill ? pskill->SkillManager() : NULL;
}

void XEntity::UnloadEntity()
{
	_attr = 0;
	_object = 0;
	OnUnintial();
	Unload();
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

XAttributes* XEntity::getAttributes()
{
	return _attr;
}