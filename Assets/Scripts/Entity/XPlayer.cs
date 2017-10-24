using UnityEngine;

public class XPlayer : XRole
{
    

    public static int PlayerLayer = LayerMask.NameToLayer("Player");

    public Vector3 lastpos = Vector3.zero;
    private XRole _watch_to = null;

    public XRole WatchTo { get { return _watch_to != null && !_watch_to.Deprecated ? _watch_to : null; } }

    public override void OnInitial()
    {
        base.OnInitial();
        _eEntity_Type |= EntityType.Player;
        OnStopJoyStick(null);
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


    private void ApplyJoyStickMove()
    {
        XCamera camera = XScene.singleton.GameCamera;
        if (camera != null && XVirtualTab.singleton.Direction != Vector3.zero)
        {
            MoveForward(XVirtualTab.singleton.Direction);
        }
    }

    private void OnStopJoyStick(XEventArgs e)
    {
        StopMove();
    }


}
