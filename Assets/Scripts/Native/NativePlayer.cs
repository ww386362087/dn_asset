using UnityEngine;

public class NativePlayer : NativeRole
{
    public Vector3 lastpos = Vector3.zero;

    protected override void OnInitial()
    {
        base.OnInitial();
    }

    protected override void EventSubscribe()
    {
        base.EventSubscribe();
        RegisterEvent(XEventDefine.XEvent_JoyStick_Cancel, OnStopJoyStick);
    }

    protected override void OnUpdate(float delta)
    {
        base.OnUpdate(delta);
        if (XVirtualTab.singleton.Feeding)
        {
            ApplyJoyStickMove();
        }
    }


    private void ApplyJoyStickMove()
    {
        if (XVirtualTab.singleton.Direction != Vector3.zero)
        {
            MoveForward(XVirtualTab.singleton.Direction);
        }
    }

    
    private void OnStopJoyStick(XEventArgs e)
    {
        StopMove();
    }
}
