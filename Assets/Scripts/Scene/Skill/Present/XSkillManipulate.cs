using UnityEngine;
using System.Collections.Generic;

public class XSkillManipulate
{
    private XSkillHoster _hoster = null;

    private Dictionary<long, XManipulationData> _item = new Dictionary<long, XManipulationData>();
    public Dictionary<long, XManipulationData> Set { get { return _item; } }

    public XSkillManipulate(XSkillHoster hoster)
    {
        _hoster = hoster;
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
        else
        {
            _item.Remove(token);
        }
    }

    public void Update(float deltaTime)
    {
        XSkillHit[] hits = GameObject.FindObjectsOfType<XSkillHit>();

        foreach (XManipulationData data in _item.Values)
        {
            Vector3 center = _hoster.transform.position + _hoster.transform.rotation * new Vector3(data.OffsetX, 0, data.OffsetZ);

            foreach (XSkillHit hit in hits)
            {
                Vector3 gap = center - hit.transform.position; gap.y = 0;
                float dis = gap.magnitude;

                if (dis < data.Radius && (dis == 0 || Vector3.Angle(-gap, _hoster.transform.forward) <= data.Degree * 0.5f))
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
