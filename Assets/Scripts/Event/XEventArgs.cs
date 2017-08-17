

public abstract class XEventArgs
{

    protected XEventDefine _eDefine = XEventDefine.XEvent_Invalid;

    public virtual bool ManualRecycle { get { return false; } }

    public XEventDefine ArgsDefine
    {
        get { return _eDefine; }
    }

    public long Token = 0;

    public virtual void Recycle()
    {
        _eDefine = XEventDefine.XEvent_Invalid;
        Token = 0;
    }


    public XEventArgs()
    {
        Token = XCommon.singleton.UniqueToken;
    }


}