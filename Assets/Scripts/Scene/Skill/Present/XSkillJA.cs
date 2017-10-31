using UnityEngine;

public class XSkillJA : XSkill
{

    private int _jaCount = 0;

    public XSkillJA(XSkillHoster _host) : base(_host)
    {
    }

    public override void Execute()
    {
        base.Execute();
        if (current.Ja != null)
        {
            for (int i = 0, max = current.Ja.Count; i < max; i++)
            {
                var data = current.Ja[i];
                AddedTimerToken(XTimerMgr.singleton.SetTimer(data.Point, OnTrigger, data), true);
            }
        }
    }

    public override void OnTrigger(object param)
    {
        int i = (int)param;
        float swype = XGesture.singleton.LastSwypeAt;
        float trigger_at = swype - host.FireTime - Time.deltaTime;

        XJAData jd = current.Ja[_jaCount];
        if (trigger_at < jd.End && trigger_at > jd.At)
        {
            if (host.xOuterData.Ja[i].Name != null && host.xOuterData.Ja[i].Name.Length > 0)
            {
                host.StopFire();
                host.Triger = XSkillData.JA_Command[host.SkillDataExtra.JaEx[i].Ja.SkillPosition];
                host.SetCurrData(host.SkillDataExtra.JaEx[i].Ja);
                host.state = DummyState.Fire;
                host.FireTime = Time.time;
                if (host.Actor != null) host.Actor.speed = 0;
            }
        }
        else if (host.xOuterData.Ja[i].Next_Name != null && host.xOuterData.Ja[i].Next_Name.Length > 0)
        {
            host.StopFire();
            host.Triger = XSkillData.JA_Command[host.SkillDataExtra.JaEx[i].Next.SkillPosition];
            host.SetCurrData(host.SkillDataExtra.JaEx[i].Next);
            host.state = DummyState.Fire;
            host.FireTime = Time.time;
            if (host.Actor != null) host.Actor.speed = 0;
        }
        _jaCount++;
    }


    public override void Clear()
    {
        _jaCount = 0;
    }


}


