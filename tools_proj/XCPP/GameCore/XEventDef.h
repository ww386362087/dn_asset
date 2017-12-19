#ifndef  __XEventDef__
#define  __XEventDef__


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




#endif