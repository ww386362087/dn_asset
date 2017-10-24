using UnityEngine;

public enum XEventDefine
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
    public XEntity Target;

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

public class XCameraActionEvent : XEventArgs
{
    public float To_Rot_X, To_Rot_Y = 0;

    public XCameraActionEvent() : base()
    {
        _eDefine = XEventDefine.XEvent_Camera_Action;
    }

    public override void Recycle()
    {
        To_Rot_X = 0;
        To_Rot_Y = 0;
        base.Recycle();
    }
}


public class XAIEventArgs : XEventArgs
{
    public bool DepracatedPass;
    public int EventType;
    public string EventArg;

    public XAIEventArgs() : base()
    {
        _eDefine = XEventDefine.XEvent_Camera_Action;
    }

    public override void Recycle()
    {
        DepracatedPass = false;
        EventType = 1;
        EventArg = string.Empty;
        base.Recycle();
    }
}

public class XAttachEventArgs : XEventArgs
{
    public XAttachEventArgs() : base()
    {
        _eDefine = XEventDefine.XEvent_Attach_Host;
    }

    public override void Recycle()
    {
    }
}

public class XDetachEventArgs:XEventArgs
{
    public XDetachEventArgs() : base()
    {
        _eDefine = XEventDefine.XEvent_Detach_Host;
    }

    public override void Recycle()
    {
    }
}


public class XAIStartSkillEventArgs : XEventArgs
{
    public uint SkillId = 0;
    public bool IsCaster = false;

    public XAIStartSkillEventArgs()
    {
        _eDefine = XEventDefine.XEvent_AIStartSkill;
    }

    public override void Recycle()
    {
        SkillId = 0;
        IsCaster = false;
        base.Recycle();
    }
}

internal class XAIEndSkillEventArgs : XEventArgs
{
    public uint SkillId = 0;
    public bool IsCaster = false;

    public XAIEndSkillEventArgs()
    {
        _eDefine = XEventDefine.XEvent_AIEndSkill;
    }

    public override void Recycle()
    {
        SkillId = 0;
        IsCaster = false;
        base.Recycle();
    }
   
}