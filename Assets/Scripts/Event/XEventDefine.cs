using UnityEngine;

public enum XEventDefine
{
    XEvent_Invalid = -1,
    XEvent_JoyStick_Cancel = 0,
    XEvent_Gesture_Cancel = 1,

    XEvent_Num
}



public abstract class XEventArgs
{
    protected long _token = 0;

    protected XEventDefine _eDefine = XEventDefine.XEvent_Invalid;

    protected XObject _firer = null;

    public XObject Firer
    {
        get { return _firer; }
        set { _firer = value; }
    }

    public bool ManualRecycle { get; set; }

    public XEventDefine ArgsDefine
    {
        get { return _eDefine; }
    }
    public long Token
    {
        get { return _token; }
        set { _token = value; }
    }

    public virtual void Recycle()
    {
        _eDefine = XEventDefine.XEvent_Invalid;
        _firer = null;
        _token = 0;
    } 
}



public class XJoyStickCancelEvent : XEventArgs
{
    public XJoyStickCancelEvent()
    {
        _eDefine = XEventDefine.XEvent_JoyStick_Cancel;
        Token = XCommon.singleton.UniqueToken;
    }

    public override void Recycle()
    {
        base.Recycle();
    }
}



public class XGestureCancelEvent : XEventArgs
{
    public XGestureCancelEvent()
    {
        _eDefine = XEventDefine.XEvent_Gesture_Cancel;
        Token = XCommon.singleton.UniqueToken;
    }

    public override void Recycle()
    {
        base.Recycle();
    }
}