using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// 受此技能打击范围内所有对手被动位移
/// </summary>
public class XSkillManipulate : XSkill
{

    private Dictionary<long, XManipulationData> _item = new Dictionary<long, XManipulationData>();

    public XSkillManipulate(ISkillHoster _host) : base(_host) { }

    public Dictionary<long, XManipulationData> Set { get { return _item; } }

    public override void Execute()
    {
        base.Execute();
        if (current.Manipulation != null)
        {
            for (int i = 0, max = current.Manipulation.Count; i < max; i++)
            {
                var data = current.Manipulation[i];
                AddedTimerToken(XTimerMgr.singleton.SetTimer(data.At, OnTrigger, data), true);
            }
        }
    }

    public override void OnTrigger(object param)
    {
        XManipulationData data = param as XManipulationData;
        long token = XCommon.singleton.UniqueToken;
        Add(token, data);
        host.AddedTimerToken(XTimerMgr.singleton.SetTimer(data.End - data.At, KillManipulate, token), true);
    }

    public override void Clear()
    {
        Remove(0);
    }

    public void KillManipulate(object param)
    {
        Remove((long)param);
    }

    public void Add(long token, XManipulationData data)
    {
        if (!_item.ContainsKey(token))
        {
            _item.Add(token, data);
        }
    }

    public void Remove(long token)
    {
        if (token == 0) _item.Clear();
        else _item.Remove(token);
    }

    public void Update(float deltaTime)
    {
        XHitHoster[] hits = GameObject.FindObjectsOfType<XHitHoster>();
        foreach (XManipulationData data in _item.Values)
        {
            Vector3 center = host.Transform.position + host.Transform.rotation * new Vector3(data.OffsetX, 0, data.OffsetZ);
            foreach (XHitHoster hit in hits)
            {
                Vector3 gap = center - hit.transform.position; gap.y = 0;
                float dis = gap.magnitude;

                if (dis < data.Radius && (dis == 0 || Vector3.Angle(-gap, host.Transform.forward) <= data.Degree * 0.5f))
                {
                    float len = data.Force * deltaTime;
                    Vector3 dir = gap.normalized;
                    Vector3 move = dir * Mathf.Min(dis, len);
                    hit.transform.Translate(move, Space.World);
                }
            }
        }
    }
}
