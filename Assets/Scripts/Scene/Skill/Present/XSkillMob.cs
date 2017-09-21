using System.Collections.Generic;
using UnityEngine;
using XTable;


public class XSkillMob : XSkill
{

    List<GameObject> _mob_unit = new List<GameObject>();

    protected override void OnTrigger(object param)
    {
        XMobUnitData mob = param as XMobUnitData;

        uint id = XTableMgr.GetTable<XEntityStatistics>().GetByID(mob.TemplateID).PresentID;
        XEntityPresentation.RowData data = XTableMgr.GetTable<XEntityPresentation>().GetItemID(id);

        GameObject mob_unit = GameObject.Instantiate(Resources.Load("Prefabs/" + data.Prefab)) as GameObject;

        Vector3 offset = host.transform.rotation * new Vector3(mob.Offset_At_X, mob.Offset_At_Y, mob.Offset_At_Z);
        Vector3 pos = host.transform.position + offset;
        mob_unit.transform.position = pos;
        mob_unit.transform.forward = host.transform.forward;
        if (mob.LifewithinSkill) mob_unit.tag = "Finish";
        _mob_unit.Add(mob_unit);
    }


    protected override void Clear()
    {
        if (_mob_unit.Count > 0)
        {
            for (int i = 0; i < _mob_unit.Count; i++)
            {
                if (_mob_unit[i].CompareTag("Finish")) GameObject.DestroyImmediate(_mob_unit[i]);
            }
        }
        _mob_unit.Clear();
    }

}
