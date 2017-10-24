using UnityEngine;

public class XBoss : XEntity
{

    public override void OnInitial()
    {
        base.OnInitial();
        _layer = LayerMask.NameToLayer("Boss");
        _eEntity_Type |= EntityType.Boss;
    }
    

}
