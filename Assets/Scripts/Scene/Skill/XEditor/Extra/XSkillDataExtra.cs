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
    public List<XManipulationDataExtra> ManipulationEx = new List<XManipulationDataExtra>();
    [SerializeField]
    public List<XWarningDataExtra> WarningEx = new List<XWarningDataExtra>();
    [SerializeField]
    public List<XMobUnitDataExtra> MobEx = new List<XMobUnitDataExtra>();
    [SerializeField]
    public List<XHitDataExtraEx> HitEx = new List<XHitDataExtraEx>();

    public void Add<T>() where T : XBaseDataExtra, new()
    {
        XBaseDataExtra data = new T();

        if (data is XFxDataExtra) Fx.Add(data as XFxDataExtra);
        else if (data is XWarningDataExtra) WarningEx.Add(data as XWarningDataExtra);
        else if (data is XMobUnitDataExtra) MobEx.Add(data as XMobUnitDataExtra);
        else if (data is XHitDataExtraEx) HitEx.Add(data as XHitDataExtraEx);
        else if (data is XResultDataExtraEx) ResultEx.Add(data as XResultDataExtraEx);
    }
}


[Serializable]
public class XBaseDataExtra{}

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
public class XManipulationDataExtra : XBaseDataExtra
{
    [SerializeField]
    public float At_Ratio = 0;
    [SerializeField]
    public float End_Ratio = 0;
    [SerializeField]
    public bool Present = true;
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


[Serializable]
public class XQTEDataExtra
{
    [SerializeField]
    public float QTE_At_Ratio = 0;
    [SerializeField]
    public float QTE_End_Ratio = 0;
}

[Serializable]
public class XLogicalDataExtra : XBaseDataExtra
{
    [SerializeField]
    public float Not_Move_At_Ratio = 0;
    [SerializeField]
    public float Not_Move_End_Ratio = 0;
    [SerializeField]
    public float Not_Dash_At_Ratio = 0;
    [SerializeField]
    public float Not_Dash_End_Ratio = 0;
    [SerializeField]
    public float Rotate_At_Ratio = 0;
    [SerializeField]
    public float Rotate_End_Ratio = 0;
    [SerializeField]
    public List<XQTEDataExtra> QTEDataEx = new List<XQTEDataExtra>();
    [SerializeField]
    public float Cancel_At_Ratio = 0;
    [SerializeField]
    public float Preserved_Ratio = 0;
    [SerializeField]
    public float Preserved_End_Ratio = 0;
    [SerializeField]
    public float ExString_Ratio = 0;
    [SerializeField]
    public float Not_Selected_At_Ratio = 0;
    [SerializeField]
    public float Not_Selected_End_Ratio = 0;
}