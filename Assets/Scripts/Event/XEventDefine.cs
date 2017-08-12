using UnityEngine;

public enum XEventDefine
{
    XEvent_Invalid = -1,

    //move event
    XEvent_JoyStick_Stop = 0,
    XEvent_JoySick_Move = 1,

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


public class XJoyStickDirectionEvent : XEventArgs
{
    public XJoyStickDirectionEvent()
    {
        _eDefine = XEventDefine.XEvent_JoySick_Move;
        Token = XCommon.singleton.UniqueToken;
    }

    public override void Recycle()
    {
        base.Recycle();
        x = y = 0;
    }

    public float x, y;
}


public class XJoyStickStopEvent : XEventArgs
{
    public XJoyStickStopEvent()
    {
        _eDefine = XEventDefine.XEvent_JoyStick_Stop;
        Token = XCommon.singleton.UniqueToken;
    }

    public override void Recycle()
    {
        base.Recycle();
        
    }
    
}