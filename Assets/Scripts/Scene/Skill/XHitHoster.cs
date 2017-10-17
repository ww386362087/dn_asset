using System.Collections;
using UnityEngine;
using XTable;

public class XHitHoster : MonoBehaviour {

    [SerializeField]
    public int PresentID = 0;

    private XEntityPresentation.RowData _present_data = null;
    private XHitData _data = null;
    private AnimatorOverrideController _oVerrideController = null;

    IEnumerator Start()
    {
        yield return null;
        _present_data = XTableMgr.GetTable<XEntityPresentation>().GetItemID((uint)PresentID);

        if (_oVerrideController == null) BuildOverride();
        AnimationClip clip = XResources.Load<AnimationClip>("Animation/" + _present_data.AnimLocation + _present_data.AttackIdle, AssetType.Anim);
        XDebug.Log("clip: " + (clip == null), " idle: " + _present_data.AnimLocation + _present_data.AttackIdle + " hitfly: " + (_present_data.HitFly == null));
        _oVerrideController["Idle"] = clip;
        _oVerrideController["HitLanding"] = _present_data.HitFly != null && _present_data.HitFly.Length == 0 ? null : XResources.Load<AnimationClip>("Animation/" + _present_data.AnimLocation + _present_data.HitFly[1], AssetType.Anim);

        _radius = _present_data.BoundRadius;
        _dummy_height = _present_data.BoundHeight;
    }

    private void BuildOverride()
    {
        _oVerrideController = new AnimatorOverrideController();
        _ator = GetComponent<Animator>();
        _oVerrideController.runtimeAnimatorController = _ator.runtimeAnimatorController;
        _ator.runtimeAnimatorController = _oVerrideController;
    }

    private float _radius = 0;
    private float _dummy_height = 0;

    private string _trigger = null;

    private Vector2 _pos = Vector2.zero;
    private Vector2 _des = Vector2.zero;

    private Vector3 _dir = Vector3.zero;

    private float _last_offset = 0;
    private float _last_height = 0;

    private float _delta_x = 0;
    private float _delta_y = 0;
    private float _delta_z = 0;

    private float _deltaH = 0;

    private float _gravity = 0;
    private float _rticalV = 0;

    private float _factor = 0;
    private float _elapsed = 0;

    private float _time_total = 0;

    //private bool _running = false;
    private bool _bcurve = false;
    private bool _loop_hard = true;
    private bool _change_to_fly = false;

    private float _present_straight = 1;
    private float _hard_straight = 1;
    private float _height = 0;
    private float _offset = 0;

    private float _present_animation_factor = 1;

    private float _present_anim_time = 0;
    private float _landing_time = 0;
    private float _hard_straight_time = 0;
    private float _getup_time = 0;

    private XBeHitPhase _phase = XBeHitPhase.Hit_Present;

    private Animator _ator = null;

    private XSkillHoster _hoster = null;

    private GameObject _hit_fx = null;
    private Transform _binded_bone = null;

    private IXCurve _curve_h = null;
    private IXCurve _curve_v = null;

    private float _curve_height_scale = 1;
    private float _curve_offset_scale = 1;
    private float _curve_height_time_scale = 1;
    private float _curve_offset_time_scale = 1;

    public Animator XAnimator { get { return _ator; } }
    public float Height { get { return _dummy_height; } }
    public float Radius { get { return _radius; } }
    public Vector3 RadiusCenter
    {
        get { return transform.position + transform.rotation * ((_present_data.BoundRadiusOffset != null && _present_data.BoundRadiusOffset.Length > 0) ? new Vector3(_present_data.BoundRadiusOffset[0], 0, _present_data.BoundRadiusOffset[1]) : Vector3.zero); }
    }

    void Update()
    {
        if (_hoster == null) return;

        //trigger and present
        if (null != _trigger && !_ator.IsInTransition(0))
        {
            if (_trigger == "ToBeHit")
            {
                _ator.Play("Present", 0);
            }
            else
                _ator.SetTrigger(_trigger);

            _trigger = null;
        }
        else
        {
            float last_elapsed = _elapsed;
            _elapsed += Time.deltaTime;

            if (_data.State == XBeHitState.Hit_Freezed)
            {
                float deltaH = -(_deltaH / _present_straight) * Time.deltaTime;
                transform.Translate(0, deltaH, 0, Space.World);

                if (_elapsed > _time_total)
                {
                    Cancel();
                }
            }
            else
            {
                switch (_phase)
                {
                    case XBeHitPhase.Hit_Present:
                        if (_elapsed > _present_straight)
                        {
                            _elapsed = _present_straight;
                            _ator.speed = 1;

                            if ((_change_to_fly || _data.State == XBeHitState.Hit_Fly) && _present_data.HitFly != null && _present_data.HitFly.Length > 0)
                            {
                                _ator.SetTrigger("ToBeHit_Landing");
                                _phase = XBeHitPhase.Hit_Landing;
                            }
                            else
                            {
                                _ator.SetTrigger("ToBeHit_Hard");
                                _phase = XBeHitPhase.Hit_Hard;
                            }
                        }

                        CalcDeltaPos(Time.deltaTime, last_elapsed);
                        float deltaH = -(_deltaH / _present_straight) * Time.deltaTime;

                        if (_offset < 0)
                        {
                            float move = Mathf.Sqrt(_delta_x * _delta_x + _delta_z * _delta_z);
                            float dis = (_hoster.gameObject.transform.position - gameObject.transform.position).magnitude;

                            if (move > dis - 0.5)
                            {
                                _delta_x = 0;
                                _delta_z = 0;
                            }
                        }
                        transform.Translate(_delta_x, _delta_y + deltaH, _delta_z, Space.World);
                        break;
                    case XBeHitPhase.Hit_Landing:
                        if (_elapsed > _present_straight + _landing_time)
                        {
                            _ator.SetTrigger("ToBeHit_Hard");
                            _phase = XBeHitPhase.Hit_Hard;
                        }
                        break;
                    case XBeHitPhase.Hit_Hard:
                        if (_elapsed > _present_straight + _landing_time + _hard_straight)
                        {
                            _ator.speed = 1;
                            _ator.SetTrigger("ToBeHit_GetUp");
                            _phase = XBeHitPhase.Hit_GetUp;
                        }
                        else
                        {
                            if (!_loop_hard) _ator.speed = _hard_straight_time / _hard_straight;
                        }
                        break;
                    case XBeHitPhase.Hit_GetUp:
                        if (_elapsed > _time_total)
                        {
                            Cancel();
                        }
                        break;
                }
            }
        }
    }

    protected void Cancel()
    {
        _elapsed = 0;
        _rticalV = 0;
        _gravity = 0;
        _deltaH = 0;
        _ator.speed = 1;
        DestroyFx();

        _data = null;
        _hoster = null;

        _ator.SetTrigger("ToStand");
    }

    public void Begin(XSkillHoster hoster, XHitData data, Vector3 dir, bool bAttackOnHitDown)
    {
        if (data.State == XBeHitState.Hit_Free) return;

        _hoster = hoster;
        _deltaH = transform.position.y;
        _data = data;
        _change_to_fly = (_data.State == XBeHitState.Hit_Back || _data.State == XBeHitState.Hit_Roll) && _deltaH > 0.1f;

        DestroyFx();
        BuildAnimation(data);

        if (_data.State == XBeHitState.Hit_Freezed)
        {
            _time_total = _data.FreezeDuration;
            _present_animation_factor = 1;
        }
        else
        {
            float OffsetTimeScale_Offset = 1;
            float OffsetTimeScale_Height = 1;
            float OffsetTimeScale_Present = 1;
            float OffsetTimeScale_Hard = 1;

            switch (_data.State)
            {
                case XBeHitState.Hit_Back:
                    if (_change_to_fly)
                    {
                        OffsetTimeScale_Offset = _present_data.HitFlyOffsetTimeScale[0] == 0 ? 1 : _present_data.HitFlyOffsetTimeScale[0];
                        OffsetTimeScale_Height = _present_data.HitFlyOffsetTimeScale[1] == 0 ? 1 : _present_data.HitFlyOffsetTimeScale[1];
                        OffsetTimeScale_Present = _present_data.HitFlyOffsetTimeScale[2] == 0 ? 1 : _present_data.HitFlyOffsetTimeScale[2];
                        OffsetTimeScale_Hard = _present_data.HitFlyOffsetTimeScale[3] == 0 ? 1 : _present_data.HitFlyOffsetTimeScale[3];
                    }
                    else
                    {
                        OffsetTimeScale_Offset = _present_data.HitBackOffsetTimeScale[0] == 0 ? 1 : _present_data.HitBackOffsetTimeScale[0];
                        OffsetTimeScale_Present = _present_data.HitBackOffsetTimeScale[1] == 0 ? 1 : _present_data.HitBackOffsetTimeScale[1];
                        OffsetTimeScale_Hard = _present_data.HitBackOffsetTimeScale[2] == 0 ? 1 : _present_data.HitBackOffsetTimeScale[2];
                    }
                    break;
                case XBeHitState.Hit_Fly:
                    OffsetTimeScale_Offset = _present_data.HitFlyOffsetTimeScale[0] == 0 ? 1 : _present_data.HitFlyOffsetTimeScale[0];
                    OffsetTimeScale_Height = _present_data.HitFlyOffsetTimeScale[1] == 0 ? 1 : _present_data.HitFlyOffsetTimeScale[1];
                    OffsetTimeScale_Present = _present_data.HitFlyOffsetTimeScale[2] == 0 ? 1 : _present_data.HitFlyOffsetTimeScale[2];
                    OffsetTimeScale_Hard = _present_data.HitFlyOffsetTimeScale[3] == 0 ? 1 : _present_data.HitFlyOffsetTimeScale[3];
                    break;
                case XBeHitState.Hit_Roll:
                    if (_change_to_fly)
                    {
                        OffsetTimeScale_Offset = _present_data.HitFlyOffsetTimeScale[0] == 0 ? 1 : _present_data.HitFlyOffsetTimeScale[0];
                        OffsetTimeScale_Height = _present_data.HitFlyOffsetTimeScale[1] == 0 ? 1 : _present_data.HitFlyOffsetTimeScale[1];
                        OffsetTimeScale_Present = _present_data.HitFlyOffsetTimeScale[2] == 0 ? 1 : _present_data.HitFlyOffsetTimeScale[2];
                        OffsetTimeScale_Hard = _present_data.HitFlyOffsetTimeScale[3] == 0 ? 1 : _present_data.HitFlyOffsetTimeScale[3];
                    }
                    else
                    {
                        OffsetTimeScale_Offset = _present_data.HitRollOffsetTimeScale[0] == 0 ? 1 : _present_data.HitRollOffsetTimeScale[0];
                        OffsetTimeScale_Present = _present_data.HitRollOffsetTimeScale[1] == 0 ? 1 : _present_data.HitRollOffsetTimeScale[1];
                        OffsetTimeScale_Hard = _present_data.HitRollOffsetTimeScale[2] == 0 ? 1 : _present_data.HitRollOffsetTimeScale[2];
                    }
                    break;
            }

            _present_straight = (_change_to_fly ? (data.Additional_Using_Default ? XHitConfLibrary.Hit_PresentStraight : data.Additional_Hit_Time_Present_Straight) : data.Time_Present_Straight) * OffsetTimeScale_Present;
            _hard_straight = (_change_to_fly ? (data.Additional_Using_Default ? XHitConfLibrary.Hit_HardStraight : data.Additional_Hit_Time_Hard_Straight) : data.Time_Hard_Straight) * OffsetTimeScale_Hard;
            _height = (_change_to_fly ? (data.Additional_Using_Default ? XHitConfLibrary.Hit_Height : data.Additional_Hit_Height) : data.Height) * OffsetTimeScale_Height;
            _offset = (_change_to_fly ? (data.Additional_Using_Default ? XHitConfLibrary.Hit_Offset : data.Additional_Hit_Offset) : data.Offset) * OffsetTimeScale_Offset;

            _dir = dir;
            _time_total = _present_straight + _landing_time + _hard_straight + _getup_time;
            _bcurve = data.CurveUsing;

            _present_animation_factor = _present_anim_time / _present_straight;

            //need re-calculate between hurt and broken
            if (_bcurve)
            {
                if (_present_data.HitFly != null && _present_data.HitCurves != null)
                {
                    IXCurve raw_h = ((_change_to_fly || _data.State == XBeHitState.Hit_Fly) && _present_data.HitFly.Length > 0) ?
                        XResources.Load<XCurve>("Curve/" + _present_data.CurveLocation + _present_data.HitCurves[4], AssetType.Prefab) : null;
                    IXCurve raw_v = ((_change_to_fly || _data.State == XBeHitState.Hit_Fly) && _present_data.HitFly.Length > 0) ?
                                     XResources.Load<XCurve>("Curve/" + _present_data.CurveLocation + _present_data.HitCurves[3], AssetType.Prefab) :
                                     ((_data.State == XBeHitState.Hit_Roll && _present_data.Hit_Roll.Length > 0) ?
                                       XResources.Load<XCurve>("Curve/" + _present_data.CurveLocation + _present_data.HitCurves[5], AssetType.Prefab) :
                                       (_data.State == XBeHitState.Hit_Back ? XResources.Load<XCurve>("Curve/" + _present_data.CurveLocation + _present_data.HitCurves[(int)data.State_Animation], AssetType.Prefab) :
                                        XResources.Load<XCurve>("Curve/" + _present_data.CurveLocation + _present_data.HitCurves[0], AssetType.Prefab)));

                    _curve_h = raw_h != null ? raw_h : null;
                    _curve_v = raw_v;

                    _curve_height_scale = (raw_h == null || raw_h.GetMaxValue() == 0) ? 1 : _height / raw_h.GetMaxValue();
                    _curve_offset_scale = raw_v.GetMaxValue() == 0 ? 1 : _offset / raw_v.GetMaxValue();
                }
            }
        }

        //play fx here
        if (data.Fx != null) PlayHitFx(data.Fx, data.Fx_Follow);
        _elapsed = 0;
        ReadyToGo(data);
        _trigger = _data.State == XBeHitState.Hit_Freezed ? (_data.FreezePresent ? "ToFreezed" : null) : "ToBeHit";
        _ator.speed = _trigger == null ? 0 : _present_animation_factor;
        _phase = XBeHitPhase.Hit_Present;
    }

    protected void ReadyToGo(XHitData data)
    {
        if (_data.State == XBeHitState.Hit_Freezed) return;

        _pos.x = transform.position.x;
        _pos.y = transform.position.z;
        Vector3 destination = transform.position + _dir * _offset;
        _des.x = destination.x;
        _des.y = destination.z;

        if (_bcurve)
        {
            _curve_height_time_scale = _curve_h == null ? 1 : _present_straight / _curve_h.GetTime(_curve_h.length - 1);
            _curve_offset_time_scale = _present_straight / _curve_v.GetTime(_curve_v.length - 1);

            _last_offset = 0;
            _last_height = 0;
        }
        else
        {
            _factor = XCommon.singleton.GetSmoothFactor((_pos - _des).magnitude, _present_straight, 0.01f);
            _rticalV = ((!_change_to_fly && _data.State != XBeHitState.Hit_Fly)) ? 0 : (_height * 4.0f) / _present_straight;
            _gravity = _rticalV / _present_straight * 2.0f;
        }
    }

    private void BuildAnimation(XHitData data)
    {
        string[] anims = null;
        switch (data.State)
        {
            case XBeHitState.Hit_Back:
                {
                    if (_change_to_fly)
                    {
                        anims = _present_data.HitFly != null && _present_data.HitFly.Length > 0 ? _present_data.HitFly : _present_data.Hit_f;
                    }
                    else
                    {
                        switch (data.State_Animation)
                        {
                            case XBeHitState_Animation.Hit_Back_Front: anims = _present_data.Hit_f; break;
                            case XBeHitState_Animation.Hit_Back_Left: anims = _present_data.Hit_l; break;
                            case XBeHitState_Animation.Hit_Back_Right: anims = _present_data.Hit_r; break;
                        }
                    }
                }
                break;
            case XBeHitState.Hit_Roll:
                {
                    anims = _change_to_fly ?
                        _present_data.HitFly != null && _present_data.HitFly.Length > 0 ? _present_data.HitFly : _present_data.Hit_f :
                        _present_data.HitFly != null && _present_data.HitFly.Length > 0 ? _present_data.Hit_Roll : _present_data.Hit_f;
                }
                break;
            case XBeHitState.Hit_Fly:
                {
                    anims = _present_data.HitFly != null && _present_data.HitFly.Length > 0 ? _present_data.HitFly : _present_data.Hit_f;
                }
                break;
            case XBeHitState.Hit_Freezed:
                {
                    if (_data.FreezePresent)
                    {
                        string freeze = "Animation/" + _present_data.AnimLocation + _present_data.Freeze;
                        AnimationClip freeze_clip = XResources.Load<AnimationClip>(freeze, AssetType.Anim);
                        _present_anim_time = freeze_clip.length;
                        _oVerrideController["Freezed"] = freeze_clip;
                    }
                    return;
                }
        }
        if (anims == null)
            return;
        int idx = 0;

        string clipname = "Animation/" + _present_data.AnimLocation + anims[idx++];
        AnimationClip clip = XResources.Load<AnimationClip>(clipname, AssetType.Anim);
        _present_anim_time = clip.length;
        _oVerrideController["PresentStraight"] = clip;

        if ((_change_to_fly || data.State == XBeHitState.Hit_Fly) && _present_data.HitFly != null && _present_data.HitFly.Length > 0)
        {
            clipname = "Animation/" + _present_data.AnimLocation + anims[idx++];
            clip = XResources.Load<AnimationClip>(clipname, AssetType.Anim);
            _landing_time = clip.length;
        }
        else
        {
            _landing_time = 0;
        }

        clipname = "Animation/" + _present_data.AnimLocation + anims[idx++];
        clip = XResources.Load<AnimationClip>(clipname, AssetType.Anim);
        _oVerrideController["HardStraight"] = clip;
        _loop_hard = (clip.wrapMode == WrapMode.Loop);
        _hard_straight_time = clip.length;

        clipname = "Animation/" + _present_data.AnimLocation + anims[idx++];
        clip = XResources.Load<AnimationClip>(clipname, AssetType.Anim);
        _getup_time = clip.length;
        _oVerrideController["GetUp"] = clip;
    }

    private void PlayHitFx(string fx, bool follow)
    {
        if (fx.Length == 0) return;

        GameObject o = Resources.Load(fx) as GameObject;
        _hit_fx = GameObject.Instantiate(o) as GameObject;

        _binded_bone = transform.Find("Bip001/Bip001 Pelvis/Bip001 Spine");
        Transform parent = (_binded_bone == null) ? gameObject.transform : _binded_bone;

        if (follow)
        {
            _hit_fx.transform.parent = parent;
            _hit_fx.transform.localPosition = Vector3.zero;
            _hit_fx.transform.localRotation = Quaternion.identity;
            _hit_fx.transform.localScale = Vector3.one;
        }
        else
        {
            _hit_fx.transform.position = parent.position;
            _hit_fx.transform.rotation = parent.rotation;
        }

        ParticleSystem[] systems = _hit_fx.GetComponentsInChildren<ParticleSystem>();
        foreach (ParticleSystem system in systems)
        {
            system.Play();
        }
    }

    private void DestroyFx()
    {
        if (_hit_fx != null)
        {
            ParticleSystem[] systems = _hit_fx.GetComponentsInChildren<ParticleSystem>();
            foreach (ParticleSystem system in systems)
            {
                system.Stop();
            }

            _hit_fx.transform.parent = null;
            GameObject.Destroy(_hit_fx);
        }
        _hit_fx = null;
    }

    private void CalcDeltaPos(float deltaTime, float last_elapsed)
    {
        Vector2 delta = Vector2.zero;
        float h = 0;

        if (_bcurve)
        {
            float ev = (_elapsed) / _curve_offset_time_scale;
            float eh = (_elapsed) / _curve_height_time_scale;

            float c_v = _curve_v.Evaluate(ev) * _curve_offset_scale;
            float c_h = _curve_h == null ? 0 : _curve_h.Evaluate(eh) * _curve_height_scale;

            Vector3 v = _dir * (c_v - _last_offset);
            delta.x = v.x; delta.y = v.z;

            h = c_h - _last_height;

            _last_height = c_h;
            _last_offset = c_v;
        }
        else
        {
            float v1 = _rticalV - _gravity * (last_elapsed);
            float v2 = _rticalV - _gravity * (_elapsed);

            h = (v1 + v2) / 2.0f * deltaTime;

            _pos.x = transform.position.x;
            _pos.y = transform.position.z;

            delta = (_des - _pos) * Mathf.Min(1.0f, _factor * deltaTime);
        }

        _delta_x = delta.x;
        _delta_y = h;
        _delta_z = delta.y;
    }
}
