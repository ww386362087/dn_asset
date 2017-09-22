using System.Collections.Generic;
using UnityEngine;

public class XSkillWarning : XSkill
{
    public List<Vector3>[] WarningPosAt = null;

    public XSkillWarning(XSkillHoster _host) : base(_host)
    {
        
    }

    public override void Execute()
    {
        base.Execute();

        if (current.Warning != null)
        {
            if (current.Warning.Count > 0)
                WarningPosAt = new List<Vector3>[current.Warning.Count];

            for (int i = 0, max = current.Warning.Count; i < max; i++)
            {
                var data = current.Warning[i];
                AddedTimerToken(XTimerMgr.singleton.SetTimer(data.At, OnTrigger, data), false);
            }
        }
    }

    public override void OnTrigger(object param)
    {
        XWarningData data = param as XWarningData;
        WarningPosAt[data.Index].Clear();

        if (data.RandomWarningPos || data.Type == XWarningType.Warning_Multiple)
        {
            if (data.RandomWarningPos)
            {
                List<GameObject> item = new List<GameObject>();
                switch (data.Type)
                {
                    case XWarningType.Warning_All:
                    case XWarningType.Warning_Multiple:
                        XSkillHit[] hits = GameObject.FindObjectsOfType<XSkillHit>();
                        int n = (data.Type == XWarningType.Warning_All) ? hits.Length : data.MaxRandomTarget;
                        for (int i = 0; i < hits.Length; i++)
                        {
                            bool counted = (data.Type == XWarningType.Warning_All) ? true : host.IsPickedInRange(n, hits.Length - i);
                            if (counted)
                            {
                                n--;
                                item.Add(hits[i].gameObject);
                            }
                        }
                        break;
                    case XWarningType.Warning_Target:
                        if (host.Target != null) item.Add(host.Target);
                        break;
                }

                for (int i = 0; i < item.Count; i++)
                {
                    for (int n = 0; n < data.PosRandomCount; n++)
                    {
                        int d = Random.Range(0, 360);
                        float r = Random.Range(0, data.PosRandomRange);
                        Vector3 v = r * XCommon.singleton.HorizontalRotateVetor3(Vector3.forward, d);
                        if (!string.IsNullOrEmpty(data.Fx))
                        {
                            XFxMgr.singleton.CreateAndPlay(
                                    data.Fx,
                                    item[i].gameObject,
                                    new Vector3(v.x, 0.05f - item[i].transform.position.y, v.z),
                                    data.Scale * Vector3.one,
                                    1,
                                    data.FxDuration);
                        }
                        WarningPosAt[data.Index].Add(item[i].transform.position + v);
                    }
                }
            }
            else if (data.Type == XWarningType.Warning_Multiple)
            {
                XSkillHit[] hits = GameObject.FindObjectsOfType<XSkillHit>();
                int n = data.MaxRandomTarget;
                for (int i = 0; i < hits.Length; i++)
                {
                    if (host.IsPickedInRange(n, hits.Length - i))
                    {
                        n--;
                        if (!string.IsNullOrEmpty(data.Fx))
                        {
                            XFxMgr.singleton.CreateAndPlay(
                                    data.Fx,
                                    hits[i].gameObject,
                                    new Vector3(0, 0.05f - hits[i].transform.position.y, 0),
                                    data.Scale * Vector3.one,
                                    1,
                                    data.FxDuration);
                        }
                        WarningPosAt[data.Index].Add(hits[i].transform.position);
                    }
                }
            }
        }
        else
        {
            switch (data.Type)
            {
                case XWarningType.Warning_None:
                    {
                        Vector3 offset = host.transform.rotation * new Vector3(data.OffsetX, data.OffsetY, data.OffsetZ);

                        XFxMgr.singleton.CreateAndPlay(
                                data.Fx,
                                host.gameObject,
                                offset,
                                data.Scale * Vector3.one,
                                1,
                                data.FxDuration);

                        WarningPosAt[data.Index].Add(host.transform.position + offset);
                    }
                    break;
                case XWarningType.Warning_Target:
                    if (host.Target != null)
                    {
                        if (!string.IsNullOrEmpty(data.Fx))
                        {
                            XFxMgr.singleton.CreateAndPlay(
                                    data.Fx,
                                    host.Target.gameObject,
                                    new Vector3(0, 0.05f - host.Target.transform.position.y, 0),
                                    data.Scale * Vector3.one,
                                    1,
                                    data.FxDuration);
                        }
                        WarningPosAt[data.Index].Add(host.Target.transform.position);
                    }
                    else
                    {

                        Vector3 offset = host.transform.rotation * new Vector3(data.OffsetX, data.OffsetY, data.OffsetZ);

                        XFxMgr.singleton.CreateAndPlay(
                                data.Fx,
                                host.gameObject,
                                offset,
                                data.Scale * Vector3.one,
                                1,
                                data.FxDuration);
                        WarningPosAt[data.Index].Add(host.transform.position + offset);
                    }
                    break;
                case XWarningType.Warning_All:
                    XSkillHit[] hits = GameObject.FindObjectsOfType<XSkillHit>();

                    for (int i = 0; i < hits.Length; i++)
                    {
                        if (!string.IsNullOrEmpty(data.Fx))
                        {
                            XFxMgr.singleton.CreateAndPlay(
                                    data.Fx,
                                    hits[i].gameObject,
                                     new Vector3(0, 0.05f - hits[i].transform.position.y, 0),
                                    data.Scale * Vector3.one,
                                    1,
                                    data.FxDuration);
                        }
                        WarningPosAt[data.Index].Add(hits[i].transform.position);
                    }
                    break;
            }
        }
    }

    public override void Clear()
    {
    }

    

}

