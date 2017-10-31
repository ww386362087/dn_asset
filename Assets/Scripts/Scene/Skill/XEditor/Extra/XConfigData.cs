using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class XConfigData
{
    [SerializeField]
    public string SkillName;
    [SerializeField]
    public float Speed = 2.0f;
    [SerializeField]
    public float RotateSpeed = 12.0f;

    [SerializeField]
    public string SkillClip;
    [SerializeField]
    public string SkillClipName;

    [SerializeField]
    public string Directory = null;
    [SerializeField]
    public int Player = 0;
    [SerializeField]
    public int Dummy = 0;

    [SerializeField]
    public List<XResultDataExtra> Result = new List<XResultDataExtra>();
    [SerializeField]
    public List<XJADataExtra> Ja = new List<XJADataExtra>();
    [SerializeField]
    public XLogicalDataExtra Logical = new XLogicalDataExtra();


    public void Add<T>() where T : XBaseDataExtra, new()
    {
        XBaseDataExtra data = new T();
        if (data is XResultDataExtra) Result.Add(data as XResultDataExtra);
        else if (data is XJADataExtra) Ja.Add(data as XJADataExtra);
    }
}