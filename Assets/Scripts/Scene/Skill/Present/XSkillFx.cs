using System.Collections.Generic;
using UnityEngine;

public class XSkillFx : XSkill
{
    protected List<XFx> _fx = new List<XFx>();
    protected List<XFx> _outer_fx = new List<XFx>();

    public XSkillFx(ISkillHoster _host) : base(_host)
    {
    }

    public override void Execute()
    {
        base.Execute();
        if (current.Fx != null)
        {
            for (int i = 0, max = current.Fx.Count; i < max; i++)
            {
                var data = current.Fx[i];
                AddedTimerToken(XTimerMgr.singleton.SetTimer(data.At, OnTrigger, data), false);
            }
        }
    }

    public override void OnTrigger(object param)
    {
        XFxData data = param as XFxData;
        if (data.Shield || data.Fx == null) return;

        Transform trans = host.Transform;
        Vector3 offset = new Vector3(data.OffsetX, data.OffsetY, data.OffsetZ);
        XFx fx = XFxMgr.singleton.CreateFx(data.Fx);
        fx.DelayDestroy = data.Destroy_Delay;
        if (data.StickToGround)
        {
            switch (data.Type)
            {
                case SkillFxType.FirerBased:
                    break;
                case SkillFxType.TargetBased:
                    if (current.NeedTarget && host.Target != null)
                    {
                        trans = host.Target.transform;
                        offset = new Vector3(data.Target_OffsetX, data.Target_OffsetY, data.Target_OffsetZ);
                    }
                    break;
            }
            Vector3 pos = trans.position + trans.rotation * offset;
            pos.y = 0;
            fx.Play(pos, Quaternion.identity, new Vector3(data.ScaleX, data.ScaleY, data.ScaleZ));
        }
        else
        {
            switch (data.Type)
            {
                case SkillFxType.FirerBased:
                    if (data.Bone != null && data.Bone.Length > 0)
                    {
                        Transform attachPoint = trans.Find(data.Bone);
                        if (attachPoint != null)
                        {
                            trans = attachPoint;
                        }
                        else
                        {
                            int index = data.Bone.LastIndexOf("/");
                            if (index >= 0)
                            {
                                string bone = data.Bone.Substring(index + 1);
                                attachPoint = trans.Find(bone);
                                if (attachPoint != null) trans = attachPoint;
                            }
                        }
                    }
                    break;
                case SkillFxType.TargetBased:
                    if (current.NeedTarget && host.Target != null)
                    {
                        trans = host.Target.transform;
                        offset = new Vector3(data.Target_OffsetX, data.Target_OffsetY, data.Target_OffsetZ);
                    }
                    break;
            }
            fx.Play(trans.gameObject, offset, new Vector3(data.ScaleX, data.ScaleY, data.ScaleZ), 1);
        }
        if (data.Combined)
        {
            if (data.End > 0)
                host.AddedCombinedToken(XTimerMgr.singleton.SetTimer(data.End - data.At, KillFx, fx));
            _outer_fx.Add(fx);
        }
        else
        {
            if (data.End > 0)
                host.AddedTimerToken(XTimerMgr.singleton.SetTimer(data.End - data.At, KillFx, fx), false);
            _fx.Add(fx);
        }
    }


    public override void Clear()
    {
        for (int i = 0; i < _outer_fx.Count; i++)
            XFxMgr.singleton.DestroyFx(_outer_fx[i], false);
        _outer_fx.Clear();

        for (int i = 0; i < _fx.Count; i++)
            XFxMgr.singleton.DestroyFx(_fx[i], false);
        _fx.Clear();
    }


    private void KillFx(object o)
    {
        XFx fx = o as XFx;
        _fx.Remove(fx);
        _outer_fx.Remove(fx);
        XFxMgr.singleton.DestroyFx(fx, false);
    }

}

