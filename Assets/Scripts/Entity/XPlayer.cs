using UnityEngine;

public class XPlayer : XRole
{
    public static int PlayerLayer = LayerMask.NameToLayer("Player");

    public Vector3 lastpos = Vector3.zero;
    private XRole _watch_to = null;

    public XRole WatchTo { get { return _watch_to != null && !_watch_to.Deprecated ? _watch_to : null; } }

    public override void OnInitial()
    {
        _eEntity_Type |= EntityType.Player;
        base.OnInitial();
    }

    protected override void EventSubscribe()
    {
        base.EventSubscribe();
        RegisterEvent(XEventDefine.XEvent_JoyStick_Cancel, OnStopJoyStick);
    }


    public override void OnUpdate(float delta)
    {
        base.OnUpdate(delta);
        lastpos = EntityObject.transform.position;
        if (XVirtualTab.singleton.Feeding)
        {
            ApplyJoyStickMove();
        }
    }

    //state在behit、skill、death等状态不能move
    private bool CheckState()
    {
        return _state == XStateDefine.XState_Idle 
            || _state == XStateDefine.XState_Move;
    }

    private void ApplyJoyStickMove()
    {
        XCamera camera = XScene.singleton.GameCamera;
        if (camera != null && XVirtualTab.singleton.Direction != Vector3.zero && CheckState())
        {
            MoveForward(XVirtualTab.singleton.Direction);
        }
    }

    private void OnStopJoyStick(XEventArgs e)
    {
        StopMove();
    }


}
