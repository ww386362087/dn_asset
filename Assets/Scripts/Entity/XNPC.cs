using UnityEngine;
using System.Collections;

public class XNPC : XEntity
{
    protected override EnitityType _eEntity_Type
    {
        get { return EnitityType.Entity_Npc; }
    }

    public static int NpcLayer = LayerMask.NameToLayer("Npc");


    public override void OnInitial()
    {
        base.OnInitial();
        _layer = LayerMask.NameToLayer("Npc");
    }

}
