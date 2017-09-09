using System;
using System.ComponentModel;
using UnityEngine;


[Serializable]
public class XJAData : XBaseData
{
    //JA Skill
    [SerializeField]
    public string Next_Name = null;
    [SerializeField]
    public string Name = null;
    [SerializeField, DefaultValueAttribute(0.0f)]
    public float At;
    [SerializeField, DefaultValueAttribute(0.0f)]
    public float End;
    [SerializeField, DefaultValueAttribute(0.0f)]
    public float Point;
}
