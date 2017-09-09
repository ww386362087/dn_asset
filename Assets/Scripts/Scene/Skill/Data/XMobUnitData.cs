using System;
using System.ComponentModel;
using UnityEngine;


[Serializable]
public class XMobUnitData : XBaseData
{
    [SerializeField, DefaultValueAttribute(0.0f)]
    public float At;
    [SerializeField, DefaultValueAttribute(0)]
    public int TemplateID;
    [SerializeField, DefaultValueAttribute(false)]
    public bool LifewithinSkill;

    [SerializeField, DefaultValueAttribute(0.0f)]
    public float Offset_At_X;
    [SerializeField, DefaultValueAttribute(0.0f)]
    public float Offset_At_Y;
    [SerializeField, DefaultValueAttribute(0.0f)]
    public float Offset_At_Z;
    [SerializeField, DefaultValueAttribute(false)]
    public bool Shield;
}
