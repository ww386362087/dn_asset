using System;
using System.ComponentModel;
using System.Xml.Serialization;
using UnityEngine;

public enum CameraMotionType
{
    AnchorBased = 0,
    CameraBased = 1,
}

public enum CameraMotionSpace
{
    World,
    Self,
    Camera
}

[Serializable]
public class XCameraMotionData : ICloneable
{
    public XCameraMotionData()
    {
        Follow_Position = true;
    }

    [SerializeField, DefaultValueAttribute(0.0f)]
    public float At;

#if DEBUG
    [SerializeField, XmlIgnore]
#endif
    public string Motion = null;
#if DEBUG
    [SerializeField, XmlIgnore]
#endif
    public CameraMotionType MotionType = CameraMotionType.CameraBased;

    [SerializeField]
    public string Motion3D = null;
    [SerializeField]
    public CameraMotionType Motion3DType = CameraMotionType.CameraBased;
    [SerializeField]
    public string Motion2_5D = null;
    [SerializeField]
    public CameraMotionType Motion2_5DType = CameraMotionType.CameraBased;

    [SerializeField, DefaultValueAttribute(false)]
    public bool LookAt_Target;

    [SerializeField, DefaultValueAttribute(true)]
    public bool Follow_Position;
    [SerializeField, DefaultValueAttribute(false)]
    public bool AutoSync_At_Begin;
    [SerializeField]
    public CameraMotionSpace Coordinate = CameraMotionSpace.World;

    public object Clone()
    {
        return MemberwiseClone();
    }
}