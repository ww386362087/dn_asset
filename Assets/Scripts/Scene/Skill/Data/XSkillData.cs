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
    public static readonly string[] Skills = {
            "XJAComboSkill",
            "XArtsSkill",
            "XCombinedSkill"
        };


    public static readonly string[] JaOverrideMap = {
            Clip.A,
            Clip.AA,
            Clip.AAA,
            Clip.AAAA,
            Clip.AAAAA,
            Clip.AB,
            Clip.QTE
    };

    public static readonly string[] JA_Command = {
           AnimTriger.ToSkill,  //A
           AnimTriger.ToJA_1_0, //AA
           AnimTriger.ToJA_2_0, //AAA
           AnimTriger.ToJA_3_0, //AAAA
           AnimTriger.ToJA_4_0, //AAAAA
           AnimTriger.ToJA_0_1, //AB
           AnimTriger.ToJA_QTE  //QTE
       };

    public static readonly string[] Combined_Command = {
            AnimTriger.ToPhase,
            AnimTriger.ToPhase1, AnimTriger.ToPhase2, AnimTriger.ToPhase3, AnimTriger.ToPhase4,
            AnimTriger.ToPhase5, AnimTriger.ToPhase6,
            AnimTriger.ToPhase7, AnimTriger.ToPhase8,
            AnimTriger.ToPhase9
      };

    public static readonly string[] CombinedOverrideMap = {
            Clip.Phase0,
            Clip.Phase1, Clip.Phase2, Clip.Phase3, Clip.Phase4,
            Clip.Phase5, Clip.Phase6,
            Clip.Phase7, Clip.Phase8,
            Clip.Phase9
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


    public bool IsInField(int triggerTime, Vector3 pos, Vector3 forward, Vector3 target, float angle, float distance)
    {
        if (Result[triggerTime].Sector_Type)
        {
            return (distance >= Result[triggerTime].Low_Range &&
                   distance < Result[triggerTime].Range &&
                    angle <= Result[triggerTime].Scope * 0.5f);
        }
        else
        {
            return IsInAttackRect(target, pos, forward, Result[triggerTime].Range, Result[triggerTime].Scope, Result[triggerTime].Rect_HalfEffect, Result[triggerTime].None_Sector_Angle_Shift);
        }
    }

    public bool IsInAttckField(Vector3 pos, Vector3 forward, GameObject target)
    {
        forward = XCommon.singleton.HorizontalRotateVetor3(forward, Cast_Scope_Shift);
        Vector3 targetPos = target.transform.position;
        if (Cast_Range_Rect)
        {
            pos.x += Cast_Offset_X;
            pos.z += Cast_Offset_Z;
            return IsInAttackRect(targetPos, pos, forward, Cast_Range_Upper, Cast_Scope, false, 0);
        }
        else
        {
            Vector3 dir = targetPos - pos;
            dir.y = 0;
            float distance = dir.magnitude;
            dir.Normalize();
            float angle = (distance == 0) ? 0 : Vector3.Angle(forward, dir);
            return distance <= Cast_Range_Upper &&
                distance >= Cast_Range_Lower &&
                angle <= Cast_Scope * 0.5f;
        }
    }


    private bool IsInAttackRect(Vector3 target, Vector3 anchor, Vector3 forward, float d, float w, bool half, float shift)
    {
        Quaternion rotation = XCommon.singleton.VectorToQuaternion(XCommon.singleton.HorizontalRotateVetor3(forward, shift));
        Rect rect = new Rect();
        rect.xMin = -w / 2.0f;
        rect.xMax = w / 2.0f;
        rect.yMin = half ? 0 : (-d / 2.0f);
        rect.yMax = d / 2.0f;
        return XCommon.singleton.IsInRect(target - anchor, rect, Vector3.zero, rotation);
    }

}

