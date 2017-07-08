using UnityEngine;
using System.Collections;

public class XRole : XEntity
{
    XAIComponent ai;
    XEquipComponent eq;


    protected override EnitityType _eEntity_Type
    {
        get { return EnitityType.Entity_Role; }
    }


    public override void OnInitial()
    {
        base.OnInitial();
        AttachComponent<XAIComponent>();
        AttachComponent<XAnimComponent>();
        AttachComponent<XEquipComponent>();
    }


}
