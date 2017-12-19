#ifndef  __XEventArgs__
#define  __XEventArgs__

#include "ITimerCallback.h"

enum  XEventDefine
{
	XEvent_Invalid = -1,
	XEvent_JoyStick_Cancel = 0,
	XEvent_Gesture_Cancel,
	XEvent_Camera_CloseUp,
	XEvent_Camera_CloseUpEnd,
	XEvent_Camera_Action,
	XEvent_Attach_Host,
	XEvent_Detach_Host,
	XEvent_AIStartSkill,
	XEvent_AIEndSkill,
	XEvent_Num
};

class XEventArgs :public ITimerArg
{
public:
	XEventArgs();
	~XEventArgs();
	XEventDefine GetEventDef();
	void SetEventDef(XEventDefine def);
	void Debug();
	long Token = 0;

protected:
	XEventDefine _eDefine = XEvent_Invalid;
	bool _ManualRecycle;
	XEventDefine _argsDefine;
};

#endif