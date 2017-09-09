using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class XSkillDataExtra
{
    [SerializeField]
    public AnimationClip SkillClip;
    [SerializeField]
    public float SkillClip_Frame;
    [SerializeField]
    public string ScriptPath;
    [SerializeField]
    public string ScriptFile;
    [SerializeField]
    public GameObject Dummy;

    [SerializeField]
    public List<XResultDataExtraEx> ResultEx = new List<XResultDataExtraEx>();
    [SerializeField]
    public List<XFxDataExtra> Fx = new List<XFxDataExtra>();
    [SerializeField]
    public List<XWarningDataExtra> Warning = new List<XWarningDataExtra>();
    [SerializeField]
    public List<XMobUnitDataExtra> Mob = new List<XMobUnitDataExtra>();
    [SerializeField]
    public List<XJADataExtraEx> JaEx = new List<XJADataExtraEx>();
    [SerializeField]
    public List<XHitDataExtraEx> HitEx = new List<XHitDataExtraEx>();

    public void Add<T>(T data) where T : XBaseDataExtra
    {
        Type t = typeof(T);

        if (t == typeof(XFxDataExtra)) Fx.Add(data as XFxDataExtra);
        else if (t == typeof(XWarningDataExtra)) Warning.Add(data as XWarningDataExtra);
        else if (t == typeof(XMobUnitDataExtra)) Mob.Add(data as XMobUnitDataExtra);
        else if (t == typeof(XJADataExtraEx)) JaEx.Add(data as XJADataExtraEx);
        else if (t == typeof(XHitDataExtraEx)) HitEx.Add(data as XHitDataExtraEx);
        else if (t == typeof(XResultDataExtraEx)) ResultEx.Add(data as XResultDataExtraEx);
    }
}


[Serializable]
public class XBaseDataExtra { }

[Serializable]
public class XResultDataExtra : XBaseDataExtra
{
    [SerializeField]
    public float Result_Ratio = 0;
}

[Serializable]
public class XResultDataExtraEx : XBaseDataExtra
{
    [SerializeField]
    public GameObject BulletPrefab = null;
    [SerializeField]
    public GameObject BulletEndFx = null;
    [SerializeField]
    public GameObject BulletHitGroundFx = null;
}

[Serializable]
public class XJADataExtraEx : XBaseDataExtra
{
    [SerializeField]
    public XSkillData Next = null;
    [SerializeField]
    public XSkillData Ja = null;
}

[Serializable]
public class XJADataExtra : XBaseDataExtra
{
    [SerializeField]
    public float JA_Begin_Ratio = 0;
    [SerializeField]
    public float JA_End_Ratio = 0;
    [SerializeField]
    public float JA_Point_Ratio = 0;
    [SerializeField]
    public string JA_Skill_PathWithName = null;
    [SerializeField]
    public string Next_Skill_PathWithName = null;
}


[Serializable]
public class XMobUnitDataExtra : XBaseDataExtra
{
    [SerializeField]
    public float Ratio = 0;
}

[Serializable]
public class XFxDataExtra : XBaseDataExtra
{
    [SerializeField]
    public GameObject Fx = null;
    [SerializeField]
    public GameObject BindTo = null;
    [SerializeField]
    public float Ratio = 0;
    [SerializeField]
    public float End_Ratio = -1;
}

[Serializable]
public class XWarningDataExtra : XBaseDataExtra
{
    [SerializeField]
    public GameObject Fx = null;
    [SerializeField]
    public float Ratio = 0;
}

[Serializable]
public class XHitDataExtraEx : XBaseDataExtra
{
    [SerializeField]
    public GameObject Fx = null;
}