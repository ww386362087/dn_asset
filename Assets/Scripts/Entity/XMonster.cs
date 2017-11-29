using UnityEngine;


public class XMonster : XEntity
{
    private XAnimComponent anim;
    private int hitCnt = 0;
    private int maxHit = 4;

    public override void OnInitial()
    {
        _eEntity_Type |= EntityType.Monster;
        base.OnInitial();
        _layer = LayerMask.NameToLayer("Enemy");
        _speed = 0.001f;

        anim = AttachComponent<XAnimComponent>();
        AttachComponent<XAIComponent>();
        AttachComponent<XNavComponent>();
        AttachComponent<XSkillComponent>();
        AttachComponent<XBeHitComponent>();

        InitAnim();
    }

    
    private void InitAnim()
    {
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


    public override void OnHit(bool hit)
    {
        base.OnHit(hit);
        hitCnt++;
        if (hitCnt >= maxHit)
        {
           // OnDied();
        }
    }

}

