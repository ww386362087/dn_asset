using System.Collections.Generic;
using UnityEngine;

public enum DummyState { Idle, Move, Fire };

public interface ISkillHoster
{
    Transform Transform { get; }

    GameObject Target { get; }

    XConfigData ConfigData { get; set; }

    XSkillResult skillResult { get; set; }
    
    List<Vector3>[] warningPosAt { get; set; }

    XSkillData CurrentSkillData { get; }
    
    Transform ShownTransform { get; set; }

    void StopFire();
    
    Vector3 GetRotateTo();
}
