#ifndef  __XEventArgs__
#define  __XEventArgs__

#include "XEventDef.h"
#include "XDelegate.h"

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



#endif