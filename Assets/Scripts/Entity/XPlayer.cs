using UnityEngine;

public class XPlayer : XRole
{

    protected override EnitityType _eEntity_Type
    {
        get { return EnitityType.Entity_Player; }
    }

    public static int PlayerLayer = LayerMask.NameToLayer("Player");

    public Vector3 lastpos = Vector3.zero;

    private Vector3 _v3 = Vector3.zero;
    private XRole _watch_to = null;

    public XRole WatchTo { get { return _watch_to != null && !_watch_to.Deprecated ? _watch_to : null; } }


    protected override void EventSubscribe()
    {
        base.EventSubscribe();
        RegisterEvent(XEventDefine.XEvent_JoyStick_Stop, OnStopJoyStick);
    }


    public override void Update(float delta)
    {
        base.Update(delta);
        lastpos = EntityObject.transform.position;
        if(XVirtualTab.singleton.Feeding)
        {
            ApplyJoyStickMove();
        }
    }


    private void ApplyJoyStickMove()
    {
        XCamera camera = XScene.singleton.GameCamera;
        if (camera != null && XVirtualTab.singleton.Direction != Vector3.zero)
        {
            XAnimComponent anim = GetComponent<XAnimComponent>();
            if (anim != null)
            {
                anim.SetTrigger("ToMove");
            }
            //方向
            _transf.forward = XVirtualTab.singleton.Direction;

            //位置
            _v3 = _transf.position + _transf.forward * speed;
            _v3.y = XScene.singleton.TerrainY(Position);
            _transf.position = _v3;
        }
    }

    private void OnStopJoyStick(XEventArgs e)
    {
        XAnimComponent anim = GetComponent<XAnimComponent>();
        if(anim!=null)
        {
            anim.SetTrigger("ToMove", false);
            anim.SetTrigger("ToStand");
        }
    }


}
