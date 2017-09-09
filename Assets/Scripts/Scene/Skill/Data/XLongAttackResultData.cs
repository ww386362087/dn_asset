using System;
using System.ComponentModel;
using UnityEngine;

[Serializable]
public class XLongAttackResultData
{
    public XLongAttackResultData()
    {
        WithCollision = true;
        TriggerOnce = true;
        EndFx_Ground = true;
        FlyWithTerrain = true;
        AimTargetCenter = true;
        StaticCollider = true;
    }

    [SerializeField]
    public XResultBulletType Type = XResultBulletType.Sphere;
    [SerializeField, DefaultValueAttribute(true)]
    public bool WithCollision;
    [SerializeField, DefaultValueAttribute(false)]
    public bool Follow;
    [SerializeField, DefaultValueAttribute(0.0f)]
    public float Runningtime;
    [SerializeField, DefaultValueAttribute(0.0f)]
    public float Stickytime;
    [SerializeField, DefaultValueAttribute(0.0f)]
    public float Velocity;
    [SerializeField, DefaultValueAttribute(0.0f)]
    public float Radius;
    [SerializeField, DefaultValueAttribute(0.0f)]
    public float Palstance;
    [SerializeField, DefaultValueAttribute(0.0f)]
    public float RingVelocity;
    [SerializeField, DefaultValueAttribute(0.0f)]
    public float RingRadius;
    [SerializeField, DefaultValueAttribute(false)]
    public bool RingFull;
    [SerializeField]
    public string Prefab = null;
    [SerializeField, DefaultValueAttribute(true)]
    public bool TriggerOnce;
    [SerializeField, DefaultValueAttribute(false)]
    public bool TriggerAtEnd;
    [SerializeField, DefaultValueAttribute(0.0f)]
    public float TriggerAtEnd_Cycle;
    [SerializeField, DefaultValueAttribute(0)]
    public int TriggerAtEnd_Count;
    [SerializeField, DefaultValueAttribute(0)]
    public int FireAngle;
    [SerializeField]
    public string HitGround_Fx = null;
    [SerializeField, DefaultValueAttribute(0.0f)]
    public float HitGroundFx_LifeTime;
    [SerializeField]
    public string End_Fx = null;
    [SerializeField, DefaultValueAttribute(0.0f)]
    public float EndFx_LifeTime;
    [SerializeField, DefaultValueAttribute(true)]
    public bool EndFx_Ground;
    [SerializeField]
    public string Audio = null;
    [SerializeField]
    public AudioChannel Audio_Channel = AudioChannel.Skill;
    [SerializeField]
    public string End_Audio = null;
    [SerializeField]
    public AudioChannel End_Audio_Channel = AudioChannel.Skill;
    [SerializeField, DefaultValueAttribute(true)]
    public bool FlyWithTerrain;
    [SerializeField, DefaultValueAttribute(false)]
    public bool IsPingPong;
    [SerializeField, DefaultValueAttribute(true)]
    public bool AimTargetCenter;
    [SerializeField, DefaultValueAttribute(0.0f)]
    public float At_X;
    [SerializeField, DefaultValueAttribute(0.0f)]
    public float At_Y;
    [SerializeField, DefaultValueAttribute(0.0f)]
    public float At_Z;
    [SerializeField, DefaultValueAttribute(0.0f)]
    public float Refine_Cycle;
    [SerializeField, DefaultValueAttribute(0)]
    public int Refine_Count;
    [SerializeField, DefaultValueAttribute(false)]
    public bool AutoRefine_at_Half;
    [SerializeField, DefaultValueAttribute(true)]
    public bool StaticCollider;
    [SerializeField, DefaultValueAttribute(false)]
    public bool DynamicCollider;
    [SerializeField, DefaultValueAttribute(false)]
    public bool Manipulation;
    [SerializeField, DefaultValueAttribute(0.0f)]
    public float ManipulationRadius;
    [SerializeField, DefaultValueAttribute(0.0f)]
    public float ManipulationForce;
}