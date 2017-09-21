using System;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;


public enum XStrickenResponse
{
    Invincible,
    Half_Endure,
    Cease,
    Full_Endure
}

[Serializable]
public class XQTEData
{
    [SerializeField]
    public int QTE = 0;
    [SerializeField, DefaultValueAttribute(0.0f)]
    public float At = 0.0f;
    [SerializeField, DefaultValueAttribute(0.0f)]
    public float End = 0.0f;
}

[Serializable]
public class XLogicalData
{
    public XLogicalData()
    {
        AttackOnHitDown = true;
    }

    [SerializeField]
    public XStrickenResponse StrickenMask = XStrickenResponse.Cease;
    [SerializeField, DefaultValueAttribute(0)]
    public int CanReplacedby;

    [SerializeField, DefaultValueAttribute(0.0f)]
    public float Not_Move_At;
    [SerializeField, DefaultValueAttribute(0.0f)]
    public float Not_Move_End;
    [SerializeField, DefaultValueAttribute(0.0f)]
    public float Rotate_At;
    [SerializeField, DefaultValueAttribute(0.0f)]
    public float Rotate_End;
    [SerializeField, DefaultValueAttribute(0.0f)]
    public float Rotate_Speed;
    [SerializeField, DefaultValueAttribute(false)]
    public bool Rotate_Server_Sync;
    [SerializeField, DefaultValueAttribute(0)]
    public int CanCastAt_QTE;
    [SerializeField, DefaultValueAttribute(0)]
    public int QTE_Key;
    [SerializeField]
    public List<XQTEData> QTEData = null;
    [SerializeField, DefaultValueAttribute(0.0f)]
    public float CanCancelAt;  //not canceled

    [SerializeField, DefaultValueAttribute(false)]
    public bool SuppressPlayer;
    [SerializeField, DefaultValueAttribute(true)]
    public bool AttackOnHitDown;
    [SerializeField, DefaultValueAttribute(false)]
    public bool Association;
    [SerializeField, DefaultValueAttribute(false)]
    public bool MoveType;
    [SerializeField]
    public string Association_Skill = null;

    [SerializeField, DefaultValueAttribute(0)]
    public int PreservedStrength;
    [SerializeField, DefaultValueAttribute(0.0f)]
    public float PreservedAt;
    [SerializeField, DefaultValueAttribute(0.0f)]
    public float PreservedEndAt;

    [SerializeField]
    public string Exstring = null;
    [SerializeField, DefaultValueAttribute(0.0f)]
    public float Exstring_At;

    [SerializeField, DefaultValueAttribute(0.0f)]
    public float Not_Selected_At;
    [SerializeField, DefaultValueAttribute(0.0f)]
    public float Not_Selected_End;
}