using UnityEngine;
using XTable;

public class XRole : XEntity
{

    protected CharacterController controller;
    protected XNavComponent nav;
    protected XAnimComponent ani;
    
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
        ani = AttachComponent<XAnimComponent>();
        nav = AttachComponent<XNavComponent>();

        InitAnim();
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


    private void InitAnim()
    {
        OverrideAnim("A", present.A);
        OverrideAnim("AA", present.AA);
        OverrideAnim("AAA", "Player_archer_attack_run");
        OverrideAnim("AAAA", "Player_archer_attack_run");
        OverrideAnim("AAAAA", present.AAAAA);
        OverrideAnim("Walk", present.Walk);
        OverrideAnim("Idle", present.Idle);
        OverrideAnim("Death", present.Death);
        OverrideAnim("Run", present.Run);
        OverrideAnim("RunLeft", present.RunLeft);
        OverrideAnim("RunRight", present.RunRight);
        OverrideAnim("Freezed", present.Freeze);
        OverrideAnim("HitLanding", present.Hit_l[0]);
        OverrideAnim("Phase0", "Player_archer_jump");
        OverrideAnim("Phase1", "Player_archer_attack_lifttwinshot");
        OverrideAnim("Phase2", "Player_archer_attack_aerialchainshot");
        OverrideAnim("Art", "Player_archer_victory");
    }


    private void OverrideAnim(string key,string clip)
    {
        string path = present.AnimLocation+clip;
        ani.OverrideAnim(key, path);
    }

}
