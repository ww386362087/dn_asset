using UnityEngine;
using XTable;

public enum DummyState { Idle, Move, Fire };

public interface ISkillHoster
{
    Transform Transform { get; }

    GameObject Target { get; }
    
    XSkillAttributes Attribute { get; }

    XEntityPresentation.RowData Present_data { get; }
    
    XSkillData CurrentSkillData { get; }
    
    Transform ShownTransform { get; set; }

    IHitHoster[] Hits { get; }

    void Fire();

    void StopFire();

}
