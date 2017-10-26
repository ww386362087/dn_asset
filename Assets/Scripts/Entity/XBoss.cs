using UnityEngine;

public class XBoss : XMonster
{

    public override void OnInitial()
    {
        _layer = LayerMask.NameToLayer("Boss");
        base.OnInitial();
        _eEntity_Type |= EntityType.Boss;
    }

    /// <summary>
    /// Boss狂暴
    /// </summary>
    public void MakeCrazy()
    {

    }

    /// <summary>
    /// Boss放大招
    /// </summary>
    public void CastSkill(int skillid)
    {

    }

}
