using System.Collections.Generic;
using UnityEngine;

public class XSkillAttributes
{
    private float _to = 0;
    private float _from = 0;
    public float rotate_speed = 1;
    private Transform _tranform;
    private ISkillHoster _host;

    public XSkillResult skillResult;
    private XSkillMob skillMob;
    private XSkillFx skillFx;
    private XSkillManipulate skillManip;
    public XSkillWarning skillWarning;
    public List<XSkill> skills = new List<XSkill>();


    public XSkillAttributes(ISkillHoster host, Transform tran)
    {
        _tranform = tran;
        skills.Clear();
        skillResult = new XSkillResult(host);
        skillMob = new XSkillMob(host);
        skillFx = new XSkillFx(host);
        skillManip = new XSkillManipulate(host);
        skillWarning = new XSkillWarning(host);
        skills.Add(skillResult);
        skills.Add(skillMob);
        skills.Add(skillFx);
        skills.Add(skillManip);
        skills.Add(skillWarning);
    }


    public void Execute()
    {
        for (int i = 0, max = skills.Count; i < max; i++)
        {
            skills[i].Execute();
        }
    }


    public void Clear()
    {
        for (int i = 0, max = skills.Count; i < max; i++)
        {
            skills[i].Clear();
        }
    }


    public void PrepareRotation(Vector3 targetDir)
    {
        Vector3 from = _tranform.forward;
        _from = YRotation(from);
        float angle = Vector3.Angle(from, targetDir);
        bool clockwise = XCommon.singleton.Clockwise(from, targetDir);
        _to = clockwise ? _from + angle : _from - angle;
    }


    private float YRotation(Vector3 dir)
    {
        float r = Vector3.Angle(Vector3.forward, dir);
        bool clockwise = XCommon.singleton.Clockwise(Vector3.forward, dir);
        return clockwise ? r : 360.0f - r;
    }

    public Vector3 GetRotateTo()
    {
        return XCommon.singleton.FloatToAngle(_to);
    }


    public void UpdateRotation()
    {
        if (_from != _to)
        {
            _from += (_to - _from) * Mathf.Min(1.0f, Time.deltaTime * rotate_speed);
            _tranform.rotation = Quaternion.Euler(0, _from, 0);
        }
    }

}

