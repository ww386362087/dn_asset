using UnityEngine;
using XTable;

public class NativeRole : NativeEntity
{
    public DefaultEquip.RowData defEquip = null;
    protected CharacterController controller;
    protected NativeAnimComponent ani;

    protected override void OnInitial()
    {
        base.OnInitial();
        defEquip = XTableMgr.GetTable<DefaultEquip>().GetByUID(2);
        controller = EntityObject.GetComponent<CharacterController>();
        controller.enabled = false;
        AttachComponent<NativeEquipComponent>();
    }


    protected override void OnUnintial()
    {
        DetachComponent<NativeEquipComponent>();
        DetachComponent<NativeAnimComponent>();
        ani = null;
        base.OnUnintial();
    }

    protected override void InitAnim()
    {
        OverrideAnim(Clip.AAA, "Player_archer_attack_run");
        OverrideAnim(Clip.AAAA, "Player_archer_attack_run");
        OverrideAnim(Clip.Walk, present.Walk);
        OverrideAnim(Clip.Idle, present.Idle);
        OverrideAnim(Clip.Death, present.Death);
        OverrideAnim(Clip.Run, present.Run);
        OverrideAnim(Clip.RunLeft, present.RunLeft);
        OverrideAnim(Clip.RunRight, present.RunRight);
        OverrideAnim(Clip.Freezed, present.Freeze);
        OverrideAnim(Clip.HitLanding, present.Hit_l[0]);
        OverrideAnim(Clip.PresentStraight, present.HitFly[0]);
        OverrideAnim(Clip.GetUp, present.HitFly[2]);
        OverrideAnim(Clip.Phase0, "Player_archer_jump");
        OverrideAnim(Clip.Phase1, "Player_archer_attack_lifttwinshot");
        OverrideAnim(Clip.Phase2, "Player_archer_attack_aerialchainshot");
        OverrideAnim(Clip.Art, "Player_archer_victory");
    }

   

}
