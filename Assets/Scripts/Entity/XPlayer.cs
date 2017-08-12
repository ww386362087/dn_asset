using UnityEngine;
using System.Collections;

public class XPlayer : XRole
{

    protected override EnitityType _eEntity_Type
    {
        get { return EnitityType.Entity_Player; }
    }

    public static int PlayerLayer = LayerMask.NameToLayer("Player");

    public Vector3 lastpos = Vector3.zero;

    private XRole _watch_to = null;

    public XRole WatchTo { get { return _watch_to != null && !_watch_to.Deprecated ? _watch_to : null; } }


    protected override void EventSubscribe()
    {
        base.EventSubscribe();
        RegisterEvent(XEventDefine.XEvent_JoyStick_Stop, OnStop);
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
            EntityObject.transform.forward = XVirtualTab.singleton.Direction;
            EntityObject.transform.Translate(Vector3.forward * speed);
        }
    }

    private void OnStop(XEventArgs e)
    {
        XAnimComponent anim = GetComponent<XAnimComponent>();
        if(anim!=null)
        {
            anim.SetTrigger("ToMove", false);
            anim.SetTrigger("ToStand");
        }
    }


}
