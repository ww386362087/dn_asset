using UnityEngine;
using System.Collections;

public class XBoss : XEnemy
{

    protected override EnitityType _eEntity_Type
    {
        get { return EnitityType.Entity_Boss; }
    }



}
