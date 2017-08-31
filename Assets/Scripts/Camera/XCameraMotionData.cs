using System;
using UnityEngine;
using System.ComponentModel;

#if DEBUG
using System.Xml.Serialization;
#endif


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

    [SerializeField, DefaultValueAttribute(false)]
    public bool LookAt_Target;

    [SerializeField, DefaultValueAttribute(true)]
    public bool Follow_Position;

    [SerializeField, DefaultValueAttribute(false)]
    public bool AutoSync_At_Begin;

    public object Clone()
    {
        return MemberwiseClone();
    }

}


public enum CameraMotionType
{
    AnchorBased = 0,
    CameraBased = 1,
}
