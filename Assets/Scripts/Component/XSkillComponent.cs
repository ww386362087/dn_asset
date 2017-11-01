using UnityEngine;
using System.Collections.Generic;

public class XSkillComponent : XComponent, ISkillHoster
{
    private XEntity _entity;
    private XEntity _target = null;
    private XSkillData _current = null;
    private XAnimComponent _anim;
    private float fire_time = 0;
    private string trigger = null;
    private float _to = 0;
    private float _from = 0;
    
    public XSkillResult skillResult { get; set; }
    public XSkillMob skillMob { get; set; }
    public XSkillFx skillFx { get; set; }
    public XSkillManipulate skillManip { get; set; }
    public XSkillWarning skillWarning { get; set; }

    public List<XSkill> skills = new List<XSkill>();

    protected override UpdateState state { get { return UpdateState.FRAME; } }

    public Transform Transform { get { return _entity.EntityTransfer; } }

    public GameObject Target { get { return _target.EntityObject; } }

    public XConfigData ConfigData { get; set; }
    
    public List<Vector3>[] warningPosAt { get; set; }

    public XSkillData CurrentSkillData { get { return _current; } }

    public Transform ShownTransform { get; set; }

    public override void OnInitial(XObject _obj)
    {
        base.OnInitial(_obj);
        _entity = _obj as XEntity;

        _anim = _entity.GetComponent<XAnimComponent>();
        if (_anim == null)
        {
            _anim = _entity.AttachComponent<XAnimComponent>();
        }
        RebuildSkillAniamtion();

        InitHost();
    }

    private void InitHost()
    {
        skills.Clear();
        skillResult = new XSkillResult(this);
        skillMob = new XSkillMob(this);
        skillFx = new XSkillFx(this);
        skillManip = new XSkillManipulate(this);
        skillWarning = new XSkillWarning(this);
        skills.Add(skillResult);
        skills.Add(skillMob);
        skills.Add(skillFx);
        skills.Add(skillManip);
        skills.Add(skillWarning);
    }

    public override void OnUpdate(float delta)
    {
        base.OnUpdate(delta);

        UpdateRotation();
        if (!string.IsNullOrEmpty(trigger) && _anim != null && !_anim.Ator.IsInTransition(0))
        {
            if (trigger != AnimTriger.ToStand &&
                trigger != AnimTriger.ToMove &&
                trigger != AnimTriger.EndSkill)
                Execute();

            _anim.SetTrigger(trigger);
            trigger = null;
        }
    }

    private float rotate_speed = 0;
    private void UpdateRotation()
    {
        if (_from != _to)
        {
            _from += (_to - _from) * Mathf.Min(1.0f, Time.deltaTime * rotate_speed);
            _entity.EntityTransfer.rotation = Quaternion.Euler(0, _from, 0);
        }
    }

    public void Fire()
    {
        fire_time = Time.time;
        if (_current.TypeToken == 0)
            trigger = XSkillData.JA_Command[_current.SkillPosition];
        else if (_current.TypeToken == 1)
            trigger = AnimTriger.ToArtSkill;
        FocusTarget();
    }

    public void StopFire()
    {
        trigger = AnimTriger.EndSkill;
        for (int i = 0, max = skills.Count; i < max; i++)
        {
            skills[i].Clear();
        }
    }

    private void Execute()
    {
        if (_current == null) return;
        for (int i = 0, max = skills.Count; i < max; i++)
        {
            skills[i].Execute();
        }
    }

    private void FocusTarget()
    {
        if (FindTarget() && _current.IsInAttckField(_entity.Position, _entity.Forward, _entity.EntityObject))
        {
            PrepareRotation(XCommon.singleton.Horizontal(_target.Position - _entity.Position));
        }
    }

    public void PrepareRotation(Vector3 targetDir)
    {
        Vector3 from = _entity.Forward;
        _from = YRotation(from);
        float angle = Vector3.Angle(from, targetDir);
        bool clockwise = XCommon.singleton.Clockwise(from, targetDir);
        _to = clockwise ? _from + angle : _from - angle;
    }

    private float YRotation(Vector3 dir)
    {
        float r = Vector3.Angle(Vector3.forward, dir);
        bool clockwise = XCommon.singleton.Clockwise(Vector3.forward, dir);
        return clockwise ? r : 360.0f - r;
    }

    private void RebuildSkillAniamtion()
    {
        AnimationClip clip = XResources.Load<AnimationClip>(_current.ClipName, AssetType.Anim);
        if (_current.TypeToken == 0)
        {
            string motion = XSkillData.JaOverrideMap[_current.SkillPosition];
            _anim.OverrideAnim(motion, clip);
        }
        else if (_current.TypeToken == 1)
        {
            _anim.OverrideAnim("Art", clip);
        }
    }
    
    private bool FindTarget()
    {
        List<XEntity> enemys = XEntityMgr.singleton.GetAllEnemy(_entity);
        if (enemys != null && enemys.Count > 0)
        {
            _target = enemys[0];
            return true;
        }
        return false;
    }
    
    public Vector3 GetRotateTo()
    {
        return XCommon.singleton.FloatToAngle(_to);
    }
}

