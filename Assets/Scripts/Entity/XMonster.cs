using UnityEngine;


public class XMonster : XEntity
{

    private XAnimComponent anim;

    public override void OnInitial()
    {
        _eEntity_Type |= EntityType.Monster;
        base.OnInitial();
        _layer = LayerMask.NameToLayer("Enemy");
        _speed = 0.01f;

        InitAnim();
        AttachComponent<XAIComponent>();
        AttachComponent<XNavComponent>();
    }



    private void InitAnim()
    {
        anim = AttachComponent<XAnimComponent>();
        OverrideAnim("Idle", _present.AnimLocation + _present.AttackIdle);
        OverrideAnim("Death", present.AnimLocation + present.Death);
        OverrideAnim("Run", present.AnimLocation + present.Run);
        OverrideAnim("RunLeft", present.AnimLocation + present.RunLeft);
        OverrideAnim("RunRight", present.AnimLocation + present.RunRight);
        OverrideAnim("Freezed", present.AnimLocation + present.Freeze);


        string[] hits = _present.HitFly;
        string hit = hits == null || hits.Length == 0 ? null : hits[1];
        OverrideAnim("HitLanding", _present.AnimLocation + hit);
    }



    private void OverrideAnim(string key, string clippath)
    {
        if (anim != null)
            anim.OverrideAnim(key, clippath);
    }

}

