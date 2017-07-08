using UnityEngine;
using System.Collections;

public class XEnemy : XEntity
{
    protected override EnitityType _eEntity_Type
    {
        get { return EnitityType.Entity_Enemy; }
    }


}
