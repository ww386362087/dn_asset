using System.Collections.Generic;
using UnityEngine;

public class XSkillResult : XSkill
{
    public int nHotID = 0;
    public Vector3 nResultForward = Vector3.zero;
    private Dictionary<int, HashSet<IHitHoster>> _hurt_target = new Dictionary<int, HashSet<IHitHoster>>();


    public XSkillResult(ISkillHoster _host) : base(_host)
    {
    }

    public override void Execute()
    {
        base.Execute();
        if (current.Result != null)
        {
            int index = 0;
            for (int i = 0, max = current.Result.Count; i < max; i++)
            {
                var data = current.Result[i];
                data.Token = index++;
                AddedTimerToken(XTimerMgr.singleton.SetTimer(data.At, OnTrigger, data));
            }
        }
    }

    public override void OnTrigger(object param)
    {
        XResultData data = param as XResultData;
        if (data.Loop)
        {
            int i = data.Index << 16;
            LoopResults(i);
        }
        else if (data.Group)
        {
            int i = data.Index << 16;
            GroupResults(i);
        }
        else if (data.LongAttackEffect)
        {
            Project(data);
        }
        else
        {
            InnerResult(data.Index, host.Transform.forward, host.Transform.position, current);
        }
    }

    public override void Clear()
    {
        base.Clear();
        _hurt_target.Clear();
    }

    private void AddHurtTarget(XSkillData data, IHitHoster hit, int triggerTime)
    {
        if (!data.Result[triggerTime].Loop && !data.Result[triggerTime].LongAttackEffect)
        {
            if (!_hurt_target.ContainsKey(triggerTime))
                _hurt_target[triggerTime] = new HashSet<IHitHoster>();
            _hurt_target[triggerTime].Add(hit);
        }
    }

    private bool IsHurtEntity(IHitHoster id, int triggerTime)
    {
        return _hurt_target.ContainsKey(triggerTime) ? _hurt_target[triggerTime].Contains(id) : false;
    }

    public void LoopResults(object param)
    {
        int i = (int)param;
        int index = i >> 16;
        int execute_cout = i & 0xFFFF;

        if (!current.Result[index].Loop || current.Result[index].Loop_Count <= execute_cout || current.Result[index].Cycle <= 0) return;

        if (current.Result[index].Group)
            GroupResults((index << 16) | (execute_cout << 8) | 0);
        else if (current.Result[index].LongAttackEffect)
            Project(current.Result[index]);
        else
            InnerResult(index, host.Transform.forward, host.Transform.position, current);

        ++execute_cout;
        if (current.Result[index].Loop_Count > execute_cout)
        {
            AddedTimerToken(XTimerMgr.singleton.SetTimer(current.Result[index].Cycle, LoopResults, ((index << 16) | execute_cout)));
        }
    }

    private void GroupResults(object param)
    {
        int i = (int)param;
        int index = i >> 16;
        int group_cout = i & 0x00FF;
        int loop_cout = (i & 0xFF00) >> 8;

        if (!host.CurrentSkillData.Result[index].Group || group_cout >= host.CurrentSkillData.Result[index].Group_Count) return;

        Vector3 face = host.Transform.forward;
        int angle = current.Result[index].Deviation_Angle + current.Result[index].Angle_Step * group_cout;
        angle = current.Result[index].Clockwise ? angle : -angle;

        if (current.Result[index].LongAttackEffect)
            Project(host.CurrentSkillData.Result[index], angle);
        else
            InnerResult(index, XCommon.singleton.HorizontalRotateVetor3(face, angle), host.Transform.position, current);

        group_cout++;
        if (group_cout < host.CurrentSkillData.Result[index].Group_Count)
        {
            i = (index << 16) | (loop_cout << 8) | group_cout;
            uint timer = XTimerMgr.singleton.SetTimer(current.Result[index].Time_Step, GroupResults, i);
            AddedTimerToken(timer);
        }
    }

    public void InnerResult(int triggerTime, Vector3 forward, Vector3 pos, XSkillData data, IHitHoster hitted = null)
    {
        nHotID = triggerTime;

        if (hitted == null)
        {
            pos += XCommon.singleton.VectorToQuaternion(host.Transform.forward) * new Vector3(data.Result[triggerTime].Offset_X, 0, data.Result[triggerTime].Offset_Z);
            nResultForward = forward;
            for (int i = 0, max = host.Hits.Length; i < max; i++)
            {
                var hit = host.Hits[i];
                if (IsHurtEntity(hit, triggerTime)) continue;
                Vector3 dir = hit.RadiusCenter - pos;
                dir.y = 0;
                float distance = dir.magnitude;
                if (distance > hit.Attr.radius) distance -= hit.Attr.radius;
                if (dir.sqrMagnitude == 0) dir = forward;
                dir.Normalize();

                if (data.IsInField(triggerTime, pos, forward, hit.RadiusCenter, Vector3.Angle(forward, dir), distance))
                {
                    Vector3 vHitDir = data.Result[triggerTime].Affect_Direction == XResultAffectDirection.AttackDir ? (hit.RadiusCenter - pos).normalized : host.Attribute.GetRotateTo();
                    AddHurtTarget(data, hit, triggerTime);
                    hit.Begin(host, data.Hit[triggerTime], vHitDir, data.Logical.AttackOnHitDown);
                }
            }
        }
        else
        {
            Vector3 vHitDir = data.Result[triggerTime].Affect_Direction == XResultAffectDirection.AttackDir ? hitted.RadiusCenter - pos : host.Attribute.GetRotateTo();
            vHitDir.y = 0;
            vHitDir.Normalize();
            hitted.Begin(host, data.Hit[triggerTime], vHitDir, data.Logical.AttackOnHitDown);
        }
    }


    private void Project(XResultData param, int additionalAngle = 0)
    {
        if (param.Attack_All)
        {
            IHitHoster[] hits = host.Hits;
            for (int i = 0; i < hits.Length; i++)
            {
                XBulletMgr.singleton.ShootBullet(GenerateBullet(param, hits[i].HitObject, additionalAngle));
            }
        }
        else if (param.Warning)
        {
            for (int i = 0; i < host.Attribute.skillWarning.warningPosAt[param.Warning_Idx].Count; i++)
            {
                XBulletMgr.singleton.ShootBullet(GenerateBullet(param, null, additionalAngle, i));
            }
        }
        else
        {
            XBulletMgr.singleton.ShootBullet(GenerateBullet(param, host.Target, additionalAngle));
        }
    }

    private XBullet GenerateBullet(XResultData data, GameObject target, int additionalAngle, int wid = -1)
    {
        return new XBullet(new XBulletData(
            host,
            current,
            target,
            data.Index,
            data.LongAttackData.FireAngle + additionalAngle,
            wid,
            host.Hits));
    }

}
