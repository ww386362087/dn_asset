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


    public override void Update(float delta)
    {
        base.Update(delta);
        lastpos = EntityObject.transform.position;
    }
}
