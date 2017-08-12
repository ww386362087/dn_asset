using UnityEngine;

public enum XEventDefine
{
    XEvent_Invalid = -1,

    //move event
    XEvent_Idle = 0,
    XEvent_Move = 1,
    XEvent_Jump = 2,
    XEvent_Fall = 3,

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

public class XIdleEventArgs : XEventArgs
{
    public XIdleEventArgs()
    {
        _eDefine = XEventDefine.XEvent_Idle;
        Token = XCommon.singleton.UniqueToken;
    }

    public override void Recycle()
    {
        base.Recycle();
    }
}

public class XMoveEventArgs : XEventArgs
{
    public XMoveEventArgs()
    {
        _eDefine = XEventDefine.XEvent_Move;
        Token = XCommon.singleton.UniqueToken;
    }

    public override void Recycle()
    {
        base.Recycle();

        Destination = Vector3.zero;
        Speed = 0;
    }

    public Vector3 Destination = Vector3.zero;
    public float Speed = 0;
}