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
        if (!host.EditorData.XAutoJA) return;

        int i = (int)param;
        float swype = XGesture.singleton.LastSwypeAt;
        float trigger_at = swype - host.fire_time - Time.deltaTime;

        XJAData jd = current.Ja[_jaCount];
        if (trigger_at < jd.End && trigger_at > jd.At)
        {
            if (host.xOuterData.Ja[i].Name != null && host.xOuterData.Ja[i].Name.Length > 0)
            {
                host.StopFire();
                host.trigger = XSkillData.JA_Command[host.xDataExtra.JaEx[i].Ja.SkillPosition];

                host.SetCurrData(host.xDataExtra.JaEx[i].Ja);

                host.state = XSkillHoster.DummyState.Fire;
                host.fire_time = Time.time;
                if (host.ator != null) host.ator.speed = 0;
            }
        }
        else if (host.xOuterData.Ja[i].Next_Name != null && host.xOuterData.Ja[i].Next_Name.Length > 0)
        {
            host.StopFire();
            host.trigger = XSkillData.JA_Command[host.xDataExtra.JaEx[i].Next.SkillPosition];

            host.SetCurrData(host.xDataExtra.JaEx[i].Next);
            host.state = XSkillHoster.DummyState.Fire;
            host.fire_time = Time.time;
            if (host.ator != null) host.ator.speed = 0;
        }
        _jaCount++;
    }


    public override void Clear()
    {
        _jaCount = 0;
    }


}


