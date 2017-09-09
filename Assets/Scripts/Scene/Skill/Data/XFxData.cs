using System;
using System.ComponentModel;
using UnityEngine;


public enum SkillFxType
{
    FirerBased = 0,
    TargetBased = 1
}


[Serializable]
public class XFxData : XBaseData
{
    public XFxData()
    {
        Follow = true;
        ScaleX = 1.0f;
        ScaleY = 1.0f;
        ScaleZ = 1.0f;
    }

    [SerializeField]
    public SkillFxType Type = SkillFxType.FirerBased;
    [SerializeField, DefaultValueAttribute(0.0f)]
    public float At;
    [SerializeField, DefaultValueAttribute(0.0f)]
    public float End;
    [SerializeField]
    public string Fx = null;
    [SerializeField]
    public string Bone = null;
    [SerializeField, DefaultValueAttribute(1.0f)]
    public float ScaleX;
    [SerializeField, DefaultValueAttribute(1.0f)]
    public float ScaleY;
    [SerializeField, DefaultValueAttribute(1.0f)]
    public float ScaleZ;
    [SerializeField, DefaultValueAttribute(0.0f)]
    public float OffsetX;
    [SerializeField, DefaultValueAttribute(0.0f)]
    public float OffsetY;
    [SerializeField, DefaultValueAttribute(0.0f)]
    public float OffsetZ;
    [SerializeField, DefaultValueAttribute(0.0f)]
    public float Target_OffsetX;
    [SerializeField, DefaultValueAttribute(0.0f)]
    public float Target_OffsetY;
    [SerializeField, DefaultValueAttribute(0.0f)]
    public float Target_OffsetZ;
    [SerializeField, DefaultValueAttribute(true)]
    public bool Follow;
    [SerializeField, DefaultValueAttribute(false)]
    public bool StickToGround;
    [SerializeField, DefaultValueAttribute(0.0f)]
    public float Destroy_Delay;
    [SerializeField, DefaultValueAttribute(false)]
    public bool Combined;
    [SerializeField, DefaultValueAttribute(false)]
    public bool Shield;
}