using System.Collections.Generic;
using AI;
using UnityEngine;

public class XAIComponent : XComponent
{
    private bool _is_inited = false;
    private IXBehaviorTree _tree;
    private float _ai_tick = 1.0f;  //AI心跳间隔 
    private float _tick_factor = 1f;
    private bool _enable_runtime = false;
    private uint _cast_skillid = 0;
    private float _tick = 0;
    private float _timer = 0;
    private XEntity _host;
    
    // 行为树相关的变量
    private XEntity _target = null;
    private bool _is_oppo_casting_skill = false;
    private bool _is_hurt_oppo = false;
    private float _target_distance = 0.0f;
    private float _master_distance = 9999.0f;
    private bool _is_fixed_in_cd = false;
    private float _normal_attack_prob = 0.5f;
    private float _enter_fight_range = 10.0f;
    private float _fight_together_dis = 10.0f;
    private float _max_hp = 1000.0f;
    private float _current_hp = 0.0f;
    private float _max_super_armor = 100.0f;
    private float _current_super_armor = 50.0f;
    private float _target_rotation = 0.0f;
    private float _attack_range = 1.0f;
    private float _min_keep_range = 1.0f;
    private bool _is_casting_skill = false;
    private bool _is_fighting = false;
    private bool _is_qte_state = false;
    
    public bool IsCastingSkill { get { return _is_casting_skill; } }
    public bool IsOppoCastingSkill { get { return _is_oppo_casting_skill; } }
    public bool IsFixedInCd { get { return _is_fixed_in_cd; } }
    public bool IsHurtOppo { get { return _is_hurt_oppo; } set { _is_hurt_oppo = value; } }
    public float EnterFightRange { get { return _enter_fight_range; } }

    public override void OnInitial(XObject _obj)
    {
        base.OnInitial(_obj);
        _host = _obj as XEntity;
        InitVariables();
        InitTree();
    }

    public override void OnUninit()
    {
        base.OnUninit();
    }

    protected override void EventSubscribe()
    {
        base.EventSubscribe();
        RegisterEvent(XEventDefine.XEvent_AIStartSkill, OnStartSkill);
        RegisterEvent(XEventDefine.XEvent_AIEndSkill, OnEndSkill);
    }

    public override void OnUpdate(float delta)
    {
        base.OnUpdate(delta);
        if (_tick > 0 && _tree != null)
        {
            _timer += delta;
            if (_timer >= _tick)
            {
                OnTickAI();
                _timer = 0;
            }
        }
    }

    public void SetTarget(XEntity target)
    {
        if (target == null)
            _target = null;
        else
        {
            _target = target;
            if (XEntity.Valide(_target))
            {
                _target_distance = (_host.Position - _target.Position).magnitude;
                _tree.SetVariable("target_distance", _target_distance);
                _tree.SetVariable("target", _target.EntityObject);
            }
        }
    }


    public void InitTree()
    {
        if (_enable_runtime)
        {
            _tree = new AIRunTimeBehaviorTree();
            (_tree as AIRunTimeBehaviorTree).Host = _host;
        }
        else
        {
            _tree = _host.EntityObject.AddComponent<XBehaviorTree>();
        }
        string tree = string.Empty;
        if (_host.IsPlayer)
        {
            tree = "PlayerAutoFight";
        }
        else
        {
            tree = "ai_log";// _host.Attributes.AiBehavior;
        }
        SetBehaviorTree(tree);
    }

    public void SetBehaviorTree(string tree)
    {
        if (!string.IsNullOrEmpty(tree)) _is_inited = true;
        if (_tree != null)
        {
            _tree.SetBehaviorTree(tree);
            _tree.EnableBehaviorTree(true);
            _tree.SetManual(true);
            _tick = _ai_tick * _tick_factor;
        }
        else
        {
            XDebug.LogError("Add behavior error: ", tree, _host.Attributes.Name);
        }
    }


    private void InitVariables()
    {
        XAttributes attr = _host.Attributes;
        _normal_attack_prob = attr.NormalAttackProb;
        _enter_fight_range = attr.EnterFightRange;
        _tick = attr.AIActionGap;
        _is_fixed_in_cd = attr.IsFixedInCD;
        _fight_together_dis = attr.FightTogetherDis;
    }

    protected void OnTickAI()
    {
        if (_is_inited && _tree != null && XEntity.Valide(_host))
        {
            UpdateVariable();
            SetTreeVariable(_tree);
            _tree.TickBehaviorTree();
        }
    }

    private void UpdateVariable()
    {
        if (_host.Attributes != null)
        {
            _max_hp = (float)_host.Attributes.GetAttr(XAttributeDefine.XAttr_MaxHP_Total);
            _current_hp = (float)_host.Attributes.GetAttr(XAttributeDefine.XAttr_CurrentHP_Total);
            _max_super_armor = (float)_host.Attributes.GetAttr(XAttributeDefine.XAttr_MaxSuperArmor_Total);
            _current_super_armor = (float)_host.Attributes.GetAttr(XAttributeDefine.XAttr_CurrentSuperArmor_Total);
        }
        if (XEntity.Valide(XEntityMgr.singleton.Player))
        {
            _master_distance = (_host.Position - XEntityMgr.singleton.Player.Position).magnitude;
        }
        if (_target != null && XEntity.Valide(_target))
        {
            _target_distance = (_host.Position - _target.Position).magnitude;
            _target_distance -= _target.Radius;
            if (_target_distance < 0) _target_distance = 0;
            Vector3 oDirect1 = _host.Position - _target.Position;
            _target_rotation = Mathf.Abs(XCommon.singleton.AngleWithSign(oDirect1, _target.Forward));
        }
        else
        {
            _target_distance = float.MaxValue;
            _target_rotation = 0;
        }
    }

    private void SetTreeVariable(IXBehaviorTree tree)
    {
        tree.SetVariable(AITreeArg.TARGET, _target == null ? null : _target.EntityObject);
        tree.SetVariable(AITreeArg.MASTER, XEntityMgr.singleton.Player == null ? null : XEntityMgr.singleton.Player.EntityObject);
        tree.SetVariable(AITreeArg.IsOppoCastingSkill, _is_oppo_casting_skill);
        tree.SetVariable(AITreeArg.IsHurtOppo, _is_hurt_oppo);
        tree.SetVariable(AITreeArg.TargetDistance, _target_distance);
        tree.SetVariable(AITreeArg.MasterDistance, _master_distance);
        tree.SetVariable(AITreeArg.IsFixedInCD, _is_fixed_in_cd);
        tree.SetVariable(AITreeArg.NormalAttackProb, _normal_attack_prob);
        tree.SetVariable(AITreeArg.EnterFightRange, _enter_fight_range);
        tree.SetVariable(AITreeArg.FightTogetherDis, _fight_together_dis);
        tree.SetVariable(AITreeArg.MaxHP, _max_hp);
        tree.SetVariable(AITreeArg.CurrHP, _current_hp);
        tree.SetVariable(AITreeArg.MaxSupperArmor, _max_super_armor);
        tree.SetVariable(AITreeArg.CurrSuperArmor, _current_super_armor);
        tree.SetVariable(AITreeArg.EntityType, _host.Type);
        tree.SetVariable(AITreeArg.TargetRot, _target_rotation);
        tree.SetVariable(AITreeArg.AttackRange, _attack_range);
        tree.SetVariable(AITreeArg.MinKeepRange, _min_keep_range);
        tree.SetVariable(AITreeArg.IsCastingSkill, _is_casting_skill);
        tree.SetVariable(AITreeArg.IsFighting, _is_fighting);
        tree.SetVariable(AITreeArg.IsQteState, _is_qte_state);
        tree.SetVariable(AITreeArg.MoveDir, Vector3.zero);
        tree.SetVariable(AITreeArg.MoveDest, Vector3.zero);
        tree.SetVariable(AITreeArg.MoveSpeed, _host.Speed);
        tree.SetVariable(AITreeArg.BornPos, _host.Position);
    }


    private void OnStartSkill(XEventArgs e)
    {
        XAIStartSkillEventArgs skillarg = e as XAIStartSkillEventArgs;
        if (skillarg.IsCaster)//自己放技能
        {
            _is_hurt_oppo = false;
            _is_casting_skill = true;
            _cast_skillid = skillarg.SkillId;
            if (_tree != null) _tree.SetVariable(AITreeArg.SkillID, (int)_cast_skillid);
        }
        else  // 别人放技能
        {
            //XDebug.Log("Other cast skill");
            _is_oppo_casting_skill = true;
        }
    }

    private void OnEndSkill(XEventArgs e)
    {
        XAIEndSkillEventArgs skillarg = e as XAIEndSkillEventArgs;
        if (skillarg.IsCaster)
        {
            _is_casting_skill = false;
            _cast_skillid = 0;
            _is_hurt_oppo = false;
        }
        else  // 别人放技能
        {
            //XDebug.Log("Other cast skill");
            _is_oppo_casting_skill = false;
        }
    }

    public bool FindTargetByDistance(float dist, float angle)
    { 
        List<XEntity> list = XEntityMgr.singleton.GetAllEnemy();
        if (list == null) return false;
        list.Sort(SortEntity);
        for (int i = 0; i < list.Count; i++)
        {
            XEntity enemy = list[i];
            if (XEntity.Valide(enemy))
            {
                Vector3 oTargetPos = enemy.Position;
                Vector3 oSrcPos = _host.Position;
                float distance = (oTargetPos - oSrcPos).magnitude;
                if (distance < dist)
                {
                    Vector3 dir = enemy.Position - _host.Position;
                    float targetangle = Vector3.Angle(dir, _host.Forward);
                    if (targetangle < angle)
                    {
                        _target = enemy;
                        return true;
                    }
                }
            }
        }
        return false;
    }
    
    public bool DoSelectNearest()
    {
        SetTarget(_target);
        return true;
    }

    public int SortEntity(XEntity a, XEntity b)
    {
        if (a == null || b == null)
            return 0;
        if (a.Attributes == null || b.Attributes == null)
            return 0;
        return b.Attributes.AiHit.CompareTo(a.Attributes.AiHit);
    }

}
