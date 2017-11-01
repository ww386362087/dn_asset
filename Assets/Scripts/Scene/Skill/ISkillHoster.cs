using System.Collections.Generic;
using UnityEngine;

public enum DummyState { Idle, Move, Fire };

public interface ISkillHoster
{
    Transform Transform { get; }

    GameObject Target { get; }

    XConfigData ConfigData { get; set; }

    XSkillResult skillResult { get; set; }
    
    XSkillData xOuterData { get; set; }

    List<Vector3>[] warningPosAt { get; set; }

    XSkillData CurrentSkillData { get; }

    float ir { get; set; }

    float or { get; set; }

    XSkillDataExtra SkillDataExtra { get; }

    Transform ShownTransform { get; set; }
    
    void AddedTimerToken(uint token, bool logical);
    
    void StopFire();

    void AddedCombinedToken(uint token);

    Vector3 GetRotateTo();
}
