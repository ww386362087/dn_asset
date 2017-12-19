#include "XEntity.h"

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