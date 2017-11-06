using UnityEngine;

public class XBeHitComponent : XComponent, IHitHoster
{
    private XEntity _entity;
    private XAnimComponent _anim;
    private XHitAttribute _attr = null;

    public GameObject HitObject { get { return _entity.EntityObject; } }

    public Vector3 Pos { get { return _entity.Position; } }

    public Vector3 RadiusCenter
    {
        get { return _entity.Position + _entity.Rotation * ((_entity.present.BoundRadiusOffset != null && _entity.present.BoundRadiusOffset.Length > 0) ? new Vector3(_entity.present.BoundRadiusOffset[0], 0, _entity.present.BoundRadiusOffset[1]) : Vector3.zero); }
    }

    public XHitAttribute Attr { get { return _attr; } }

    protected override UpdateState state { get { return UpdateState.FRAME; } }

    public override void OnInitial(XObject _obj)
    {
        base.OnInitial(_obj);
        _entity = _obj as XEntity;
        InitHitAnim();
        InitAttr();
    }

    private void InitHitAnim()
    {
        _anim = _entity.GetComponent<XAnimComponent>();
        if (_anim == null)
        {
            _anim = _entity.AttachComponent<XAnimComponent>();
        }

        var present = _entity.present;
        string path = present.HitFly != null && present.HitFly.Length == 0 ? null : present.AnimLocation + present.HitFly[1];
        _anim.OverrideAnim(Clip.HitLanding, path);
    }


    private void InitAttr()
    {
        _attr = new XHitAttribute(_entity.EntityTransfer,
            _anim.OverideControllder,
            _anim.Ator,
            _entity.present);
    }


    public override void OnUpdate(float delta)
    {
        base.OnUpdate(delta);
        if (_attr != null)
        {
            if (!_attr.Update())
            {
                _entity.OnHit(false);
            }
        }
    }



    public void Begin(ISkillHoster hoster, XHitData data, Vector3 dir, bool bAttackOnHitDown)
    {
        if (_attr != null && _entity.CurState != XStateDefine.XState_Death)
        {
            _entity.OnHit(true);
            _attr.Begin(hoster, data, dir, bAttackOnHitDown);
        }
    }

}
