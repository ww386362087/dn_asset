#ifndef  __XEventArgs__
#define  __XEventArgs__

#include "XEventDef.h"
#include "ITimerCallback.h"

class XEventArgs :public ITimerArg
{
public:
	XEventArgs();
	~XEventArgs();
	XEventDefine GetEventDef();
	void Debug();
	long Token = 0;

protected:
	XEventDefine _eDefine = XEvent_Invalid;
	bool _ManualRecycle;
	XEventDefine _argsDefine;
};

#endif