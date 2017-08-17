using UnityEngine;

public enum XEventDefine
{
    XEvent_Invalid = -1,
    XEvent_JoyStick_Cancel = 0,
    XEvent_Gesture_Cancel,
    XEvent_Camera_CloseUp,
    XEvent_Camera_CloseUpEnd,

    XEvent_Num
}


public class XJoyStickCancelEvent : XEventArgs
{
    public XJoyStickCancelEvent() : base()
    {
        _eDefine = XEventDefine.XEvent_JoyStick_Cancel;
    }
}


public class XGestureCancelEvent : XEventArgs
{
    public XGestureCancelEvent() : base()
    {
        _eDefine = XEventDefine.XEvent_Gesture_Cancel;
    }
}


public class XCameraCloseUpEvent : XEventArgs
{
    public XCameraCloseUpEvent() : base()
    {
        _eDefine = XEventDefine.XEvent_Camera_CloseUp;
    }
}

public class XCameraCloseUpEndEvent : XEventArgs
{
    public XCameraCloseUpEndEvent() : base()
    {
        _eDefine = XEventDefine.XEvent_Camera_CloseUpEnd;
    }

    public override void Recycle()
    {
        base.Recycle();
    }

}