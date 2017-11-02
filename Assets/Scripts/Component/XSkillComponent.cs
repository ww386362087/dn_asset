using UnityEngine;
using System.Collections.Generic;
using XTable;

public class XSkillComponent : XComponent, ISkillHoster
{
    private XEntity _entity;
    private XEntity _target = null;
    private XSkillData _current = null;
    private XAnimComponent _anim;
    private string trigger = null;
    private XSkillAttributes _attribute;
   
    protected override UpdateState state { get { return UpdateState.FRAME; } }
    
    public Transform Transform { get { return _entity.EntityTransfer; } }

    public GameObject Target { get { return _target.EntityObject; } }

    public XSkillAttributes Attribute { get { return _attribute; } }
    
    public XSkillData CurrentSkillData { get { return _current; } }

    public Transform ShownTransform { get; set; }

    public XEntityPresentation.RowData Present_data { get { return _entity.present; } }

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

        _attribute = new XSkillAttributes(this,_entity.EntityTransfer);
    }


    public override void OnUpdate(float delta)
    {
        base.OnUpdate(delta);
        if (_attribute != null) _attribute.UpdateRotation();
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


    public void Fire()
    {
        if (_current.TypeToken == 0)
            trigger = XSkillData.JA_Command[_current.SkillPosition];
        else if (_current.TypeToken == 1)
            trigger = AnimTriger.ToArtSkill;
        FocusTarget();
    }

    public void StopFire()
    {
        trigger = AnimTriger.EndSkill;
        if (_attribute != null) _attribute.Clear();
    }

    private void Execute()
    {
        if (_current == null) return;
        if (_attribute != null) _attribute.Execute();
    }

    private void FocusTarget()
    {
        if (FindTarget() && _current.IsInAttckField(_entity.Position, _entity.Forward, _entity.EntityObject) && _attribute != null)
        {
            _attribute.PrepareRotation(XCommon.singleton.Horizontal(_target.Position - _entity.Position));
        }
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
    
}

