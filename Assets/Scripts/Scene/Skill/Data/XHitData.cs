using System;
using System.ComponentModel;
using UnityEngine;

public enum XBeHitState_Animation
{
    Hit_Back_Front = 0,
    Hit_Back_Left,
    Hit_Back_Right
}

public enum XBeHitPhase
{
    Hit_Present = 0,
    Hit_Landing,
    Hit_Hard,
    Hit_GetUp
}
public enum XBeHitState
{
    Hit_Back,
    Hit_Fly,
    Hit_Roll,
    Hit_Freezed,
    Hit_Free
}

[Serializable]
public class XHitData : XBaseData
{
    public XHitData()
    {
        Fx_Follow = true;
        Additional_Using_Default = true;
    }

    [SerializeField, DefaultValueAttribute(0.0f)]
    public float Time_Present_Straight;
    [SerializeField, DefaultValueAttribute(0.0f)]
    public float Time_Hard_Straight;
    [SerializeField, DefaultValueAttribute(0.0f)]
    public float Offset;
    [SerializeField, DefaultValueAttribute(0.0f)]
    public float Height;
    [SerializeField]
    public XBeHitState State = XBeHitState.Hit_Back;
    [SerializeField]
    public XBeHitState_Animation State_Animation = XBeHitState_Animation.Hit_Back_Front;

    [SerializeField, DefaultValueAttribute(0.0f)]
    public float Random_Range;

    [SerializeField]
    public string Fx = null;
    [SerializeField, DefaultValueAttribute(true)]
    public bool Fx_Follow;
    [SerializeField, DefaultValueAttribute(false)]
    public bool Fx_StickToGround;

    [SerializeField, DefaultValueAttribute(false)]
    public bool CurveUsing;

    [SerializeField, DefaultValueAttribute(false)]
    public bool FreezePresent;
    [SerializeField, DefaultValueAttribute(0.0f)]
    public float FreezeDuration;

    [SerializeField, DefaultValueAttribute(true)]
    public bool Additional_Using_Default;
    [SerializeField, DefaultValueAttribute(0.0f)]
    public float Additional_Hit_Time_Present_Straight;
    [SerializeField, DefaultValueAttribute(0.0f)]
    public float Additional_Hit_Time_Hard_Straight;
    [SerializeField, DefaultValueAttribute(0.0f)]
    public float Additional_Hit_Offset;
    [SerializeField, DefaultValueAttribute(0.0f)]
    public float Additional_Hit_Height;
}

