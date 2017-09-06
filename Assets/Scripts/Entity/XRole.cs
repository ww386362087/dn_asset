using UnityEngine;
using XTable;

public class XRole : XEntity
{

    protected CharacterController controller;
    protected XNavigationComponent nav;

    protected override EnitityType _eEntity_Type
    {
        get { return EnitityType.Entity_Role; }
    }

    public int profession = 1;
    public DefaultEquip.RowData defEquip = null;

    public override void OnInitial()
    {
        base.OnInitial();

        _layer = LayerMask.NameToLayer("Role");
        profession = 1;
        defEquip = DefaultEquip.sington.GetByProfID(profession + 1);
        controller = EntityObject.GetComponent<CharacterController>();
        controller.enabled = false;

        AttachComponent<XAIComponent>();
        AttachComponent<XAnimComponent>();
        AttachComponent<XEquipComponent>();
        AttachComponent<XNavigationComponent>();
    }


    public void Navigate(Vector3 pos)
    {
        if (nav == null)
        {
            nav = GetComponent<XNavigationComponent>();
            nav.ActiveNav();
        }
        nav.Navigate(pos);
    }

    public void DrawNavPath()
    {
        if (nav == null)
        {
            nav = GetComponent<XNavigationComponent>();
        }
        nav.DrawPath();
    }

    public void EnableCC(bool enable)
    {
        if (controller != null)
        {
            controller.enabled = enable;
        }
    }

}
