using System.Collections;
using UnityEngine;
using XTable;

public class XHitHoster : MonoBehaviour, IHitHoster
{
    [SerializeField]
    public int PresentID = 0;

    private XHitAttribute _attr = null;


    public GameObject HitObject { get { return gameObject; } }

    public Vector3 Pos { get { return transform.position; } }

    public XHitAttribute Attr { get { return _attr; } }

    public Vector3 RadiusCenter
    {
        get { return transform.position + transform.rotation * ((_attr.present.BoundRadiusOffset != null && _attr.present.BoundRadiusOffset.Length > 0) ? new Vector3(_attr.present.BoundRadiusOffset[0], 0, _attr.present.BoundRadiusOffset[1]) : Vector3.zero); }
    }

    IEnumerator Start()
    {
        yield return new WaitForSeconds(0.4f);

        var present = XTableMgr.GetTable<XEntityPresentation>().GetItemID((uint)PresentID);

        var contr = new AnimatorOverrideController();
        var ator = GetComponent<Animator>();
        contr.runtimeAnimatorController = ator.runtimeAnimatorController;
        ator.runtimeAnimatorController = contr;

        AnimationClip clip = XResources.Load<AnimationClip>("Animation/" + present.AnimLocation + present.AttackIdle, AssetType.Anim);
        contr[Clip.Idle] = clip;
        contr[Clip.HitLanding] = present.HitFly != null && present.HitFly.Length == 0 ? null : XResources.Load<AnimationClip>("Animation/" + present.AnimLocation + present.HitFly[1], AssetType.Anim);

        _attr = new XHitAttribute(transform, contr, ator, present);
    }

    private void BuildOverride()
    {
        var controllder = new AnimatorOverrideController();
        var ator = GetComponent<Animator>();
        controllder.runtimeAnimatorController = _attr.ator.runtimeAnimatorController;
        _attr.ator.runtimeAnimatorController = _attr.controllder;
    }


    void Update()
    {
        if (_attr != null)
        {
            _attr.Update();
        }
    }


    public void Begin(ISkillHoster hoster, XHitData data, Vector3 dir, bool bAttackOnHitDown)
    {
        if (_attr != null)
        {
            _attr.Begin(hoster, data, dir, bAttackOnHitDown);
        }
    }

}
