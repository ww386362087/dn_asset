public class XSkill
{

    protected ISkillHoster host;

    protected XSkillData current
    {
        get { return host.CurrentSkillData; }
    }

    public XSkill(ISkillHoster _host)
    {
        host = _host;
        host.AddSkill(this);
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
