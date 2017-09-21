using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class XSkill
{

   protected  XSkillHoster host;

    protected XSkillData current
    {
        get { return host.CurrentSkillData; }
    }

    public void Attach(XSkillHoster _host)
    {
        host = _host;
    }


    protected virtual void OnTrigger(object param)
    {

    }


    protected virtual void Clear()
    {

    }
}
