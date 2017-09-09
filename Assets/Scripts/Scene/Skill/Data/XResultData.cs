using System;
using System.ComponentModel;
using UnityEngine;


public enum XResultAffectDirection
{
    AttackDir, ChargeDir
}

public enum XResultBulletType
{
    Sphere, Plane, Satellite, Ring
}

[Serializable]
public class XResultData : XBaseData
{
    public XResultData()
    {
        Sector_Type = true;
    }

    [SerializeField, DefaultValueAttribute(false)]
    public bool LongAttackEffect;

    [SerializeField, DefaultValueAttribute(false)]
    public bool Attack_Only_Target;
    [SerializeField, DefaultValueAttribute(false)]
    public bool Attack_All;
    [SerializeField, DefaultValueAttribute(false)]
    public bool Mobs_Inclusived;
    [SerializeField, DefaultValueAttribute(true)]
    public bool Sector_Type;
    [SerializeField, DefaultValueAttribute(false)]
    public bool Rect_HalfEffect;
    [SerializeField, DefaultValueAttribute(0)]
    public int None_Sector_Angle_Shift;
    [SerializeField, DefaultValueAttribute(0.0f)]
    public float At;
    [SerializeField, DefaultValueAttribute(0.0f)]
    public float Low_Range;
    [SerializeField, DefaultValueAttribute(0.0f)]
    public float Range;
    [SerializeField, DefaultValueAttribute(0.0f)]
    public float Scope;
    [SerializeField, DefaultValueAttribute(0.0f)]
    public float Offset_X;
    [SerializeField, DefaultValueAttribute(0.0f)]
    public float Offset_Z;
    [SerializeField, DefaultValueAttribute(false)]
    public bool Loop;
    [SerializeField, DefaultValueAttribute(false)]
    public bool Group;
    [SerializeField, DefaultValueAttribute(0.0f)]
    public float Cycle;
    [SerializeField, DefaultValueAttribute(0)]
    public int Loop_Count;
    [SerializeField, DefaultValueAttribute(0)]
    public int Deviation_Angle;
    [SerializeField, DefaultValueAttribute(0)]
    public int Angle_Step;
    [SerializeField, DefaultValueAttribute(0.0f)]
    public float Time_Step;
    [SerializeField, DefaultValueAttribute(0)]
    public int Group_Count;
    [SerializeField, DefaultValueAttribute(0)]
    public int Token;
    [SerializeField, DefaultValueAttribute(false)]
    public bool Clockwise;
    //////////////////////////////////////////
    [SerializeField]
    public XLongAttackResultData LongAttackData;
    //////////////////////////////////////////
    [SerializeField, DefaultValueAttribute(false)]
    public bool Warning;
    [SerializeField, DefaultValueAttribute(0)]
    public int Warning_Idx;
    //////////////////////////////////////////
    [SerializeField]
    public XResultAffectDirection Affect_Direction = XResultAffectDirection.AttackDir;
}


