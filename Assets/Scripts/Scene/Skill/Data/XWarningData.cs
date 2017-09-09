using System;
using System.ComponentModel;
using UnityEngine;

public enum XWarningType
{
    Warning_None,
    Warning_Target,
    Warning_Multiple,
    Warning_All
}

[Serializable]
public class XWarningData : XBaseData
{
    public XWarningData()
    {
        Scale = 1.0f;
    }

    [SerializeField]
    public XWarningType Type = XWarningType.Warning_None;
    [SerializeField, DefaultValueAttribute(0.0f)]
    public float At;
    [SerializeField, DefaultValueAttribute(0.0f)]
    public float FxDuration;
    [SerializeField, DefaultValueAttribute(0.0f)]
    public float OffsetX;
    [SerializeField, DefaultValueAttribute(0.0f)]
    public float OffsetY;
    [SerializeField, DefaultValueAttribute(0.0f)]
    public float OffsetZ;
    [SerializeField]
    public string Fx = null;
    [SerializeField, DefaultValueAttribute(1.0f)]
    public float Scale = 1;
    [SerializeField, DefaultValueAttribute(false)]
    public bool Mobs_Inclusived;
    [SerializeField, DefaultValueAttribute(0)]
    public int MaxRandomTarget;
    [SerializeField, DefaultValueAttribute(false)]
    public bool RandomWarningPos;
    [SerializeField, DefaultValueAttribute(0)]
    public float PosRandomRange;
    [SerializeField, DefaultValueAttribute(0)]
    public int PosRandomCount;
}