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
        OverrideAnim("Idle", _present.AttackIdle);
        OverrideAnim("Death", present.Death);
        OverrideAnim("Run", present.Run);
        OverrideAnim("RunLeft", present.RunLeft);
        OverrideAnim("RunRight", present.RunRight);
        OverrideAnim("Freezed", present.Freeze);
        OverrideAnim("Walk", present.AttackWalk);

        string[] hits = _present.HitFly;
        string hit = hits == null || hits.Length == 0 ? null : hits[1];
        OverrideAnim("HitLanding", hit);
    }



    private void OverrideAnim(string key, string clip)
    {

        if (anim != null)
        {
            string path = present.AnimLocation + clip;
            anim.OverrideAnim(key, path);
        }
    }

}

