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
        AttachComponent<XAnimComponent>();
        AttachComponent<XEquipComponent>();
        AttachComponent<XNavComponent>();
    }


    public void Navigate(Vector3 pos)
    {
        if (nav == null)
        {
            nav = GetComponent<XNavComponent>();
            nav.ActiveNav();
        }
        nav.Navigate(pos);
    }

    public void DrawNavPath()
    {
        if (nav == null)
        {
            nav = GetComponent<XNavComponent>();
        }
        nav.DebugDrawPath();
    }

    public void EnableCC(bool enable)
    {
        if (controller != null)
        {
            controller.enabled = enable;
        }
    }

}
