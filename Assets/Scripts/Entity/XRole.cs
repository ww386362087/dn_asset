using UnityEngine;
using XTable;

public class XRole : XEntity
{


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
        DefaultEquip defaultEquip = new DefaultEquip();
        defEquip = defaultEquip.GetByProfID(profession + 1);


        AttachComponent<XAIComponent>();
        AttachComponent<XAnimComponent>();
        AttachComponent<XEquipComponent>();
    }




}
