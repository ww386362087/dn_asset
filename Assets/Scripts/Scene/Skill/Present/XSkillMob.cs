using System.Collections.Generic;
using UnityEngine;
using XTable;


public class XSkillMob : XSkill
{

    List<GameObject> _mob_unit = new List<GameObject>();

    public XSkillMob(ISkillHoster _host) : base(_host) { }

    public override void Execute()
    {
        base.Execute();
        if (current.Mob != null)
        {
            for (int i = 0, max = current.Mob.Count; i < max; i++)
            {
                var data = current.Mob[i];
                AddedTimerToken(XTimerMgr.singleton.SetTimer(data.At, OnTrigger, data), true);
            }
        }
    }

    public override void OnTrigger(object param)
    {
        XMobUnitData mob = param as XMobUnitData;

        uint id = XTableMgr.GetTable<XEntityStatistics>().GetByID(mob.TemplateID).PresentID;
        XEntityPresentation.RowData data = XTableMgr.GetTable<XEntityPresentation>().GetItemID(id);
        GameObject mob_unit = GameObject.Instantiate(Resources.Load("Prefabs/" + data.Prefab)) as GameObject;
        Vector3 offset = host.Transform.rotation * new Vector3(mob.Offset_At_X, mob.Offset_At_Y, mob.Offset_At_Z);
        Vector3 pos = host.Transform.position + offset;
        mob_unit.transform.position = pos;
        mob_unit.transform.forward = host.Transform.forward;
        if (mob.LifewithinSkill) mob_unit.tag = "Finish";
        _mob_unit.Add(mob_unit);
    }


    public override void Clear()
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
