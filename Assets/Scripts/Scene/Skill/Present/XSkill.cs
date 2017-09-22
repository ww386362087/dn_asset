public class XSkill
{

    protected XSkillHoster host;

    protected XSkillData current
    {
        get { return host.CurrentSkillData; }
    }

    public XSkill(XSkillHoster _host)
    {
        host = _host;
        host.skills.Add(this);
    }


    protected void AddedTimerToken(uint token, bool logical)
    {
        host.AddedTimerToken(token, logical);
    }

    public virtual void Execute()
    {

    }

    public virtual void OnTrigger(object param)
    {

    }


    public virtual void Clear()
    {

    }

}
