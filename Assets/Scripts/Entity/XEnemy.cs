using UnityEngine;
using System.Collections;

public class XEnemy : XEntity
{
    protected override EnitityType _eEntity_Type
    {
        get { return EnitityType.Entity_Enemy; }
    }


    public override void OnInitial()
    {
        base.OnInitial();
        _layer = LayerMask.NameToLayer("Enemy");
    }


    public bool IsFighting
    {
        get
        {
            XAIComponent ai = GetComponent<XAIComponent>();
            if (ai == null) return false;
            return ai.IsFighting;
        }
    }


}
