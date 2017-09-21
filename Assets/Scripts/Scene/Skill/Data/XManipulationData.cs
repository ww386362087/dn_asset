using System;
using System.ComponentModel;
using UnityEngine;

[Serializable]
public class XManipulationData : XBaseData
{
    [SerializeField, DefaultValueAttribute(0.0f)]
    public float At;
    [SerializeField, DefaultValueAttribute(0.0f)]
    public float End;
    [SerializeField, DefaultValueAttribute(0.0f)]
    public float OffsetX;
    [SerializeField, DefaultValueAttribute(0.0f)]
    public float OffsetZ;
    [SerializeField, DefaultValueAttribute(0.0f)]
    public float Degree;
    [SerializeField, DefaultValueAttribute(0.0f)]
    public float Radius;
    [SerializeField, DefaultValueAttribute(0.0f)]
    public float Force;
    [SerializeField, DefaultValueAttribute(true)]
    public bool TargetIsOpponent;
}