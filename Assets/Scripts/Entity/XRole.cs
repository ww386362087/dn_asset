using UnityEngine;
using XTable;

public class XRole : XEntity
{
    protected CharacterController controller;
    protected XNavComponent nav;

    public int profession = 1;
    public DefaultEquip.RowData defEquip = null;

    public override void OnInitial()
    {
        _eEntity_Type |= EntityType.Role;
        base.OnInitial();
        _layer = LayerMask.NameToLayer("Role");
        profession = 1;
        defEquip = XTableMgr.GetTable<DefaultEquip>().GetByUID(profession + 1);
        controller = EntityObject.GetComponent<CharacterController>();
        controller.enabled = false;

        AttachComponent<XAIComponent>();
        AttachComponent<XEquipComponent>();
        nav = AttachComponent<XNavComponent>();
        AttachComponent<XSkillComponent>();
        AttachComponent<XBeHitComponent>();
    }

    protected override void OnUnintial()
    {
        DetachComponent<XEquipComponent>();
        base.OnUnintial();
    }

    public void Navigate(Vector3 pos)
    {
        nav.Navigate(pos);
    }

    public void DrawNavPath()
    {
        nav.DebugDrawPath();
    }

    public void EnableCC(bool enable)
    {
        if (controller != null)
        {
            controller.enabled = enable;
        }
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
