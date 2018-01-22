using UnityEngine;

public class NativeMonster : NativeEntity
{

    protected CharacterController controller;

    protected override void OnInitial()
    {
        base.OnInitial();
        //controller = EntityObject.GetComponent<CharacterController>();
        //controller.enabled = false;
    }


    protected override void InitAnim()
    {
        OverrideAnim(Clip.Idle, _present.AttackIdle);
        OverrideAnim(Clip.Death, present.Death);
        OverrideAnim(Clip.Run, present.Run);
        OverrideAnim(Clip.RunLeft, present.RunLeft);
        OverrideAnim(Clip.RunRight, present.RunRight);
        OverrideAnim(Clip.Freezed, present.Freeze);
        OverrideAnim(Clip.Walk, present.AttackWalk);

        string[] hits = _present.HitFly;
        string hit = hits == null || hits.Length == 0 ? null : hits[1];
        OverrideAnim(Clip.HitLanding, hit);
    }


}

