#ifndef  __XEventDef__
#define  __XEventDef__

#include "XEventArgs.h"

class XEntity;

class XCameraCloseUpEvent :public XEventArgs
{
public:
	XEntity* Target;

	XCameraCloseUpEvent()
	{
		SetEventDef(XEvent_Camera_CloseUp);
	}
};


#endif