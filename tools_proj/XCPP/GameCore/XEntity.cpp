#include "XEntity.h"
#include "XAudioComponent.h"
#include "XBeHitComponent.h"
#include "XAIComponent.h"
#include "XSkillComponent.h"

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
	XObject::TAttachComponent<XAudioComponent>();
	XObject::TAttachComponent<XSkillComponent>();
	XObject::TAttachComponent<XAIComponent>();
	XObject::TAttachComponent<XBeHitComponent>();
	XObject::TGetComponnet<XAudioComponent>();
	XObject::TGetComponnet<XBeHitComponent>();
	XObject::TDetachComponent<XAudioComponent>();
	_object = go;
	_transf = go->transform;
	_attr = attr;
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

void XEntity::UnloadEntity()
{
	_attr = 0;
	_object = 0;
	OnUnintial();
	Unload();
}