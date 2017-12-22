#ifndef __STATEMACHINE_H__
#define __STATEMACHINE_H__

#include "XObject.h"
#include "XStateDefine.h"
#include "IXStateTransform.h"

class XSkillComponent;

class XStateMachine:public XComponent
{
public:
	XStateMachine();
	~XStateMachine();

	inline IXStateTransform* GetCurrent() { return _current; }
	inline IXStateTransform* GetDefault() { return _default; }
	inline XStateDefine GetCurrentState() { return _current->SelfState(); }
	inline XStateDefine GetDefaultState() { return _default->SelfState(); }
	inline XStateDefine GetPretState() { return _pre->SelfState(); }
	inline uint ActionToken() { return _current->Token(); }
	inline bool StatePermitted(XStateDefine src, XStateDefine des) const { return _bStateMap[src][des]; }

	bool CanAct(XStateDefine state);
	bool CanAct(IXStateTransform* next);
	void ForceToDefaultState(bool ignoredeath);
	bool TransferToState(IXStateTransform* next);
	void SetDefaultState(IXStateTransform* def);
	virtual void Update(float fDeltaT);


protected:
	void TransferToDefaultState();
	virtual bool InnerPermission(XStateDefine next) { return true; }

private:
	IXStateTransform* _current;
	IXStateTransform* _pre;
	IXStateTransform* _default;


	static const bool _bStateMap[][XState_Max];
	XSkillComponent* _skill;
};

#endif