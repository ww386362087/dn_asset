using System.Collections.Generic;

public class XSkill
{

    protected ISkillHoster host;

    private List<uint> _timers = new List<uint>();

    protected XSkillData current
    {
        get { return host.CurrentSkillData; }
    }

    public XSkill(ISkillHoster _host)
    {
        host = _host;
    }

    protected void AddedTimerToken(uint token)
    {
        _timers.Add(token);
    }

    public virtual void Execute() { }

    public virtual void OnTrigger(object param) { }


    public virtual void Clear()
    {
        for (int i = 0, max = _timers.Count; i < max; i++)
        {
            XTimerMgr.singleton.RemoveTimer(_timers[i]);
        }
        _timers.Clear();
    }

}
