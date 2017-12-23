#ifndef  __XEventArgs__
#define  __XEventArgs__

#include "XEventDef.h"
#include "XDelegate.h"
#include "Common.h"
#include "SkillReader.h"

class XEventArgs :public IArgs
{
public:
	XEventDefine getEventDef();
	void setEventDef(XEventDefine def);
	long Token = 0;

protected:
	XEventDefine _eDefine = XEvent_Invalid;
	XEventDefine _argsDefine;
};


class XAIStartSkillEventArgs : public XEventArgs
{
public:
	XAIStartSkillEventArgs() : XEventArgs()
	{
		SkillId = 0;
		IsCaster = false;
		_eDefine = XEvent_AIStartSkill;
	}

	int SkillId;
	bool IsCaster;
};


class XAIEndSkillEventArgs : public XEventArgs
{
public:
	XAIEndSkillEventArgs() : XEventArgs()
	{
		SkillId = 0;
		IsCaster = false;
		_eDefine = XEvent_AIEndSkill;
	}

	int SkillId;
	bool IsCaster;
};


class XAttackArgs : public XEventArgs
{
public:
	XAttackArgs()
		:Identify(0u), 
		Target(0u), 
		Slot(0u), 
		HasManualFace(false), 
		ManualFace(0.0f), 
		TimeScale(1.0f)
	{
		_eDefine = XEvent_Attack;
	}
	virtual ~XAttackArgs() { }

	uint Identify;
	uint Target;
	uint Slot;
	bool HasManualFace;
	float ManualFace;
	float TimeScale;
};

class XIdleArgs : public XEventArgs
{
public:
	XIdleArgs()
	{
		_eDefine = XEvent_Idle;
	}
};


class XChargeActionArgs : public XEventArgs
{
public:
	XChargeActionArgs()
	{
		new(this) XChargeActionArgs(NULL);
	}

	XChargeActionArgs(const XChargeData* data)
		:XEventArgs(),
		Height(0),
		TimeSpan(0),
		Target(0),
		TimeGone(0),
		TimeScale(1.0f)
	{
		_eDefine = XAction_Charge;
		_data = data;
	}

	float Height;
	float TimeSpan;
	uint Target;
	float TimeGone;
	float TimeScale;

public:
	inline const XChargeData* Data() const { return _data; }

private:
	const XChargeData* _data;
};

#endif