#include "XStateMachine.h"



XStateMachine::XStateMachine()
{
}


XStateMachine::~XStateMachine()
{
}


bool XStateMachine::CanAct(IXStateTransform* next)
{
	return CanAct(next->SelfState());
}

bool XStateMachine::CanAct(XStateDefine state)
{
	return (state == XState_Death) || (InnerPermission(state) && _current->IsPermitted(state));
}


void XStateMachine::Update(float fDeltaT)
{
	_current->StateUpdate(fDeltaT);
	if (_current->IsFinished()) TransferToDefaultState();
}

void XStateMachine::ForceToDefaultState(bool ignoredeath)
{
	if (ignoredeath || _current->SelfState() != XState_Death)
	{
		if (_current != _default)
		{
			_current->Stop(_default->SelfState());
			TransferToDefaultState();
		}
	}
}

void XStateMachine::TransferToDefaultState()
{
	XIdleArgs args;
	GetHost()->DispatchEvent(&args);
}

bool XStateMachine::TransferToState(IXStateTransform* next)
{
	if (CanAct(next))
	{
		if (_current->SelfState() != next->SelfState() || next->SelfState() != XState_Move)
			_current->Stop(next->SelfState());

		_pre = _current;
		_current = next;
		_current->OnGetPermission();
		return true;
	}
	else
	{
		next->OnRejected(_current->SelfState());
		return false;
	}
}

void XStateMachine::SetDefaultState(IXStateTransform* def)
{
	_default = def;
	_current = _default;
	_pre = _default;
}