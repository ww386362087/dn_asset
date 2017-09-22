using System.Collections.Generic;
using UnityEngine;

public class XSkillResult : XSkill
{
    public int nHotID = 0;
    public Vector3 nResultForward = Vector3.zero;
    public List<Vector3>[] WarningPosAt = null;
    private List<HashSet<XSkillHit>> _hurt_target = new List<HashSet<XSkillHit>>();

    public XSkillResult(XSkillHoster _host) : base(_host)
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
                AddedTimerToken(XTimerMgr.singleton.SetTimer(data.At, OnTrigger, data), true);
            }
        }
    }

    public override void OnTrigger(object param)
    {
        if (host.state != XSkillHoster.DummyState.Fire) return;

        XResultData data = param as XResultData;

        if (data.Loop)
        {
            int i = (data.Index << 16) | 0;
            LoopResults(i);
        }
        else
        {
            if (data.Group)
            {
                int i = (data.Index << 16) | 0;
                GroupResults(i);
            }
            else
            {
                if (data.LongAttackEffect)
                {
                    Project(data);
                }
                else
                {
                    InnerResult(data.Index, host.transform.forward, host.transform.position, current);
                }
            }
        }
    }
    
    public override void Clear()
    {
    }
    
    private void AddHurtTarget(XSkillData data, XSkillHit id, int triggerTime)
    {
        if (!data.Result[triggerTime].Loop && /*for multiple trigger end*/!data.Result[triggerTime].LongAttackEffect)
            _hurt_target[triggerTime].Add(id);
    }

    private bool IsHurtEntity(XSkillHit id, int triggerTime)
    {
        /*
         * this section not as same as client shows
         * but in editor mode just using it for simple.
         */
        return triggerTime < _hurt_target.Count ? _hurt_target[triggerTime].Contains(id) : false;
    }

    public void LoopResults(object param)
    {
        int i = (int)param;
        int count = i >> 16;
        int execute_cout = i & 0xFFFF;

        if (!current.Result[count].Loop || current.Result[count].Loop_Count <= execute_cout || current.Result[count].Cycle <= 0)
            return;

        if (current.Result[count].Group)
            GroupResults((count << 16) | (execute_cout << 8) | 0);
        else if (current.Result[count].LongAttackEffect)
            Project(current.Result[count]);
        else
        {
            InnerResult(count, host.transform.forward, host.transform.position, current);
        }

        ++execute_cout;

        if (current.Result[count].Loop_Count > execute_cout)
           host.AddedTimerToken(XTimerMgr.singleton.SetTimer(current.Result[count].Cycle, LoopResults, ((count << 16) | execute_cout)), true);
    }

    private void GroupResults(object param)
    {
        if (host.state != XSkillHoster.DummyState.Fire) return;

        int i = (int)param;
        int count = i >> 16;

        int group_cout = i & 0x00FF;
        int loop_cout = (i & 0xFF00) >> 8;

        if (!host.CurrentSkillData.Result[count].Group || group_cout >= host.CurrentSkillData.Result[count].Group_Count)
            return;

        Vector3 face = host.transform.forward;

        int angle = current.Result[count].Deviation_Angle + current.Result[count].Angle_Step * group_cout;
        angle = current.Result[count].Clockwise ? angle : -angle;

        if (current.Result[count].LongAttackEffect)
            Project(host.CurrentSkillData.Result[count], angle);
        else
            InnerResult(count, XCommon.singleton.HorizontalRotateVetor3(face, angle), host.transform.position, current);

        group_cout++;
        if (group_cout < host.CurrentSkillData.Result[count].Group_Count)
        {
            i = (count << 16) | (loop_cout << 8) | group_cout;
            host.AddedTimerToken(XTimerMgr.singleton.SetTimer(current.Result[count].Time_Step, GroupResults, i), true);
        }
    }

    public void InnerResult(int triggerTime, Vector3 forward, Vector3 pos, XSkillData data, XSkillHit hitted = null)
    {
        nHotID = triggerTime;

        if (hitted == null)
        {
            pos += XCommon.singleton.VectorToQuaternion(host.transform.forward) * new Vector3(data.Result[triggerTime].Offset_X, 0, data.Result[triggerTime].Offset_Z);
            nResultForward = forward;

            XSkillHit[] hits = GameObject.FindObjectsOfType<XSkillHit>();

            foreach (XSkillHit hit in hits)
            {
                if (IsHurtEntity(hit, triggerTime)) continue;

                Vector3 dir = hit.RadiusCenter - pos; dir.y = 0;
                float distance = dir.magnitude;

                if (distance > hit.Radius) distance -= hit.Radius;

                if (dir.sqrMagnitude == 0) dir = forward;
                dir.Normalize();

                if (host.IsInField(data, triggerTime, pos, forward, hit.RadiusCenter, Vector3.Angle(forward, dir), distance))
                {
                    Vector3 vHitDir = data.Result[triggerTime].Affect_Direction == XResultAffectDirection.AttackDir ?
                        (hit.RadiusCenter - pos).normalized :
                        host.GetRotateTo();

                    //_effectual = true;
                    AddHurtTarget(data, hit, triggerTime);
                    hit.Begin(host, data.Hit[triggerTime], vHitDir, data.Logical.AttackOnHitDown);
                }
            }
        }
        else
        {
            Vector3 vHitDir = data.Result[triggerTime].Affect_Direction == XResultAffectDirection.AttackDir ?
                        (hitted.RadiusCenter - pos) :
                        host.GetRotateTo();

            vHitDir.y = 0; vHitDir.Normalize();
            hitted.Begin(host, data.Hit[triggerTime], vHitDir, data.Logical.AttackOnHitDown);
        }
    }


    private void Project(XResultData param, int additionalAngle = 0)
    {
        if (param.Attack_All)
        {
            XSkillHit[] hits = GameObject.FindObjectsOfType<XSkillHit>();

            for (int i = 0; i < hits.Length; i++)
            {
                XBulletMgr.singleton.ShootBullet(GenerateBullet(param, hits[i].gameObject, additionalAngle));
            }
        }
        else if (param.Warning)
        {
            for (int i = 0; i < WarningPosAt[param.Warning_Idx].Count; i++)
            {
                XBulletMgr.singleton.ShootBullet(GenerateBullet(param, null, additionalAngle, i));
            }
        }
        else
            XBulletMgr.singleton.ShootBullet(GenerateBullet(param, host.Target, additionalAngle));
    }

    private XBullet GenerateBullet(XResultData data, GameObject target, int additionalAngle, int wid = -1)
    {
        return new XBullet(new XBulletData(
            host,
            current,
            target,
            data.Index,
            data.LongAttackData.FireAngle + additionalAngle,
            wid));
    }

}
