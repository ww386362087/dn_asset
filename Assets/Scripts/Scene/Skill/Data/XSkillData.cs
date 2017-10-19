using System;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

[Serializable]
public class XBaseData
{
    [SerializeField, DefaultValueAttribute(0)]
    public int Index = 0;
}

[Serializable]
public class XSkillData
{
    public static readonly string[] Skills =
        {
            "XJAComboSkill", 
            "XArtsSkill",
            "XCombinedSkill"
        };

    public static readonly string[] JaOverrideMap =
      {
            "A",
            "AA", "AAA", "AAAA", "AAAAA",
            "AB", "ABB",
            "AAB", "AABB",
            "AAAB", "AAABB",
            "AAAAB", "AAAABB",
            "AAAAAB", "AAAAABB",
            "QTE"
        };

    public static readonly string[] JA_Command = new string[]
       {
            "ToSkill",
            "ToJA_1_0", "ToJA_2_0", "ToJA_3_0", "ToJA_4_0",
            "ToJA_0_1", "ToJA_0_2",
            "ToJA_1_1", "ToJA_1_2",
            "ToJA_2_1", "ToJA_2_2",
            "ToJA_3_1", "ToJA_3_2",
            "ToJA_4_1", "ToJA_4_2",
            "ToJA_QTE"
       };

    public static readonly string[] Combined_Command = new string[]
      {
            "ToPhase",
            "ToPhase1", "ToPhase2", "ToPhase3", "ToPhase4",
            "ToPhase5", "ToPhase6",
            "ToPhase7", "ToPhase8",
            "ToPhase9"
      };

    public static readonly string[] CombinedOverrideMap =
       {
            "Phase0",
            "Phase1", "Phase2", "Phase3", "Phase4",
            "Phase5", "Phase6",
            "Phase7", "Phase8",
            "Phase9"
        };

    public XSkillData()
    {
        TypeToken = 1;
        NeedTarget = true;
        BackTowardsDecline = 0.75f;
        CameraTurnBack = 1.0f;
        CoolDown = 1.0f;
    }

    [SerializeField]
    public string Name;
    [SerializeField, DefaultValueAttribute(1)]
    public int TypeToken;
    [SerializeField]
    public string ClipName;

    [SerializeField, DefaultValueAttribute(0)]
    public int SkillPosition;
    [SerializeField, DefaultValueAttribute(false)]
    public bool IgnoreCollision;
    [SerializeField, DefaultValueAttribute(true)]
    public bool NeedTarget;
    [SerializeField, DefaultValueAttribute(false)]
    public bool OnceOnly;
    [SerializeField, DefaultValueAttribute(false)]
    public bool MultipleAttackSupported;
    [SerializeField, DefaultValueAttribute(0.75f)]
    public float BackTowardsDecline;

    [SerializeField]
    public List<XResultData> Result;
    [SerializeField]
    public List<XJAData> Ja;
    [SerializeField]
    public List<XHitData> Hit;
    [SerializeField]
    public List<XManipulationData> Manipulation;
    [SerializeField]
    public List<XFxData> Fx;
    [SerializeField]
    public List<XWarningData> Warning;
    [SerializeField]
    public List<XMobUnitData> Mob;

    [SerializeField]
    public XLogicalData Logical;

    [SerializeField, DefaultValueAttribute(1.0f)]
    public float CoolDown;
    [SerializeField, DefaultValueAttribute(0.0f)]
    public float Time;

    [SerializeField, DefaultValueAttribute(false)]
    public bool Cast_Range_Rect;
    [SerializeField, DefaultValueAttribute(0.0f)]
    public float Cast_Range_Upper;
    [SerializeField, DefaultValueAttribute(0.0f)]
    public float Cast_Range_Lower;
    [SerializeField, DefaultValueAttribute(0.0f)]
    public float Cast_Offset_X;
    [SerializeField, DefaultValueAttribute(0.0f)]
    public float Cast_Offset_Z;
    [SerializeField, DefaultValueAttribute(0.0f)]
    public float Cast_Scope;
    [SerializeField, DefaultValueAttribute(0.0f)]
    public float Cast_Scope_Shift;
    [SerializeField, DefaultValueAttribute(1.0f)]
    public float CameraTurnBack;


}

