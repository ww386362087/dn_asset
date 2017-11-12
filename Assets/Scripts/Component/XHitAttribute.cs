using UnityEngine;
using XTable;

public class XHitAttribute
{
    public XHitData data = null;
    public ISkillHoster hoster = null;

    public float deltaH = 0;
    public float gravity = 0;
    public float rticalV = 0;

    public Vector3 dir = Vector3.zero;
    public float radius = 0;

    public string trigger = null;
    public Vector2 pos = Vector2.zero;
    public Vector2 des = Vector2.zero;

    public float last_offset = 0;
    public float last_height = 0;

    public float delta_x = 0;
    public float delta_z = 0;

    public float factor = 0;
    public float elapsed = 0;
    public float time_total = 0;

    public bool bcurve = false;
    public bool loop_hard = true;
    public bool change_to_fly = false;

    public float present_straight = 1;
    public float hard_straight = 1;
    public float height = 0;
    public float offset = 0;

    public float present_animation_factor = 1;
    public float present_anim_time = 0;
    public float landing_time = 0;
    public float hard_straight_time = 0;
    public float getup_time = 0;

    public IXCurve curve_h = null;
    public IXCurve curve_v = null;

    public float curve_height_scale = 1;
    public float curve_offset_scale = 1;
    public float curve_height_time_scale = 1;
    public float curve_offset_time_scale = 1;

    public XBeHitPhase phase = XBeHitPhase.Hit_Present;

    public XEntityPresentation.RowData present = null;
    public GameObject hit_fx = null;
    public Transform binded_bone = null;
    public Animator ator = null;
    public AnimatorOverrideController controllder = null;

    private Transform transform;

    public XHitAttribute(Transform tr, AnimatorOverrideController oc, Animator at, XEntityPresentation.RowData pres)
    {
        transform = tr;
        controllder = oc;
        ator = at;
        present = pres;
        radius = present.BoundRadius;
    }


    public bool Update()
    {
        if ( hoster == null) return true;

        if (!string.IsNullOrEmpty(trigger) && !ator.IsInTransition(0))
        {
            if (trigger == AnimTriger.ToBeHit)
                ator.Play("Present", 0);
            else
                ator.SetTrigger(trigger);
            trigger = null;
        }
        else
        {
            float last_elapsed = elapsed;
            elapsed += Time.deltaTime;
            if (data.State == XBeHitState.Hit_Freezed)
            {
                float dh = -(deltaH / present_straight) * Time.deltaTime;
                transform.Translate(0, dh, 0, Space.World);
                if (elapsed > time_total)
                {
                    Cancel();
                    return false;
                }
            }
            else
            {
                switch (phase)
                {
                    case XBeHitPhase.Hit_Present:
                        if (elapsed > present_straight)
                        {
                            elapsed = present_straight;
                            ator.speed = 1;

                            if ((change_to_fly || data.State == XBeHitState.Hit_Fly) && present.HitFly != null && present.HitFly.Length > 0)
                            {
                                ator.SetTrigger(AnimTriger.ToBeHit_Landing);
                                phase = XBeHitPhase.Hit_Landing;
                            }
                            else
                            {
                                ator.SetTrigger(AnimTriger.ToBeHit_Hard);
                                phase = XBeHitPhase.Hit_Hard;
                            }
                        }
                        CalcDeltaPos(transform.position, Time.deltaTime, last_elapsed);
                        if (offset < 0)
                        {
                            float move = Mathf.Sqrt(delta_x * delta_x + delta_z * delta_z);
                            float dis = (hoster.Transform.position - transform.position).magnitude;
                            if (move < dis - 0.5)
                            {
                                transform.Translate(delta_x, 0, delta_z, Space.World);
                            }
                        }
                        break;
                    case XBeHitPhase.Hit_Landing:
                        if (elapsed > present_straight + landing_time)
                        {
                            ator.SetTrigger(AnimTriger.ToBeHit_Hard);
                            phase = XBeHitPhase.Hit_Hard;
                        }
                        break;
                    case XBeHitPhase.Hit_Hard:
                        if (elapsed > present_straight + landing_time + hard_straight)
                        {
                            ator.speed = 1;
                            ator.SetTrigger(AnimTriger.ToBeHit_GetUp);
                            phase = XBeHitPhase.Hit_GetUp;
                        }
                        else if (!loop_hard)
                        {
                            ator.speed = hard_straight_time / hard_straight;
                        }
                        break;
                    case XBeHitPhase.Hit_GetUp:
                        if (elapsed > time_total)
                        {
                            Cancel();
                            return false;
                        }
                        break;
                }
            }
        }
        return true;
    }


    public void Begin(ISkillHoster host, XHitData da, Vector3 d, bool bAttackOnHitDown)
    {
        if (da.State == XBeHitState.Hit_Free) return;
        hoster = host;
        deltaH = transform.position.y;
        data = da;
        change_to_fly = (data.State == XBeHitState.Hit_Back || data.State == XBeHitState.Hit_Roll) && deltaH > 0.1f;

        DestroyFx();
        BuildAnimation(data);

        if (data.State == XBeHitState.Hit_Freezed)
        {
            time_total = data.FreezeDuration;
            present_animation_factor = 1;
        }
        else
        {
            float OffsetTimeScale_Offset = 1;
            float OffsetTimeScale_Height = 1;
            float OffsetTimeScale_Present = 1;
            float OffsetTimeScale_Hard = 1;

            switch (data.State)
            {
                case XBeHitState.Hit_Back:
                    if (change_to_fly)
                    {
                        OffsetTimeScale_Offset = present.HitFlyOffsetTimeScale[0] == 0 ? 1 : present.HitFlyOffsetTimeScale[0];
                        OffsetTimeScale_Height = present.HitFlyOffsetTimeScale[1] == 0 ? 1 : present.HitFlyOffsetTimeScale[1];
                        OffsetTimeScale_Present = present.HitFlyOffsetTimeScale[2] == 0 ? 1 : present.HitFlyOffsetTimeScale[2];
                        OffsetTimeScale_Hard = present.HitFlyOffsetTimeScale[3] == 0 ? 1 : present.HitFlyOffsetTimeScale[3];
                    }
                    else
                    {
                        OffsetTimeScale_Offset = present.HitBackOffsetTimeScale[0] == 0 ? 1 : present.HitBackOffsetTimeScale[0];
                        OffsetTimeScale_Present = present.HitBackOffsetTimeScale[1] == 0 ? 1 : present.HitBackOffsetTimeScale[1];
                        OffsetTimeScale_Hard = present.HitBackOffsetTimeScale[2] == 0 ? 1 : present.HitBackOffsetTimeScale[2];
                    }
                    break;
                case XBeHitState.Hit_Fly:
                    OffsetTimeScale_Offset = present.HitFlyOffsetTimeScale[0] == 0 ? 1 : present.HitFlyOffsetTimeScale[0];
                    OffsetTimeScale_Height = present.HitFlyOffsetTimeScale[1] == 0 ? 1 : present.HitFlyOffsetTimeScale[1];
                    OffsetTimeScale_Present = present.HitFlyOffsetTimeScale[2] == 0 ? 1 : present.HitFlyOffsetTimeScale[2];
                    OffsetTimeScale_Hard = present.HitFlyOffsetTimeScale[3] == 0 ? 1 : present.HitFlyOffsetTimeScale[3];
                    break;
                case XBeHitState.Hit_Roll:
                    if (change_to_fly)
                    {
                        OffsetTimeScale_Offset = present.HitFlyOffsetTimeScale[0] == 0 ? 1 : present.HitFlyOffsetTimeScale[0];
                        OffsetTimeScale_Height = present.HitFlyOffsetTimeScale[1] == 0 ? 1 : present.HitFlyOffsetTimeScale[1];
                        OffsetTimeScale_Present = present.HitFlyOffsetTimeScale[2] == 0 ? 1 : present.HitFlyOffsetTimeScale[2];
                        OffsetTimeScale_Hard = present.HitFlyOffsetTimeScale[3] == 0 ? 1 : present.HitFlyOffsetTimeScale[3];
                    }
                    else
                    {
                        OffsetTimeScale_Offset = present.HitRollOffsetTimeScale[0] == 0 ? 1 : present.HitRollOffsetTimeScale[0];
                        OffsetTimeScale_Present = present.HitRollOffsetTimeScale[1] == 0 ? 1 : present.HitRollOffsetTimeScale[1];
                        OffsetTimeScale_Hard = present.HitRollOffsetTimeScale[2] == 0 ? 1 : present.HitRollOffsetTimeScale[2];
                    }
                    break;
            }

            present_straight = (change_to_fly ? data.Additional_Hit_Time_Present_Straight : data.Time_Present_Straight) * OffsetTimeScale_Present;
            hard_straight = (change_to_fly ? data.Additional_Hit_Time_Hard_Straight : data.Time_Hard_Straight) * OffsetTimeScale_Hard;
            height = (change_to_fly ? data.Additional_Hit_Height : data.Height) * OffsetTimeScale_Height;
            offset = (change_to_fly ? data.Additional_Hit_Offset : data.Offset) * OffsetTimeScale_Offset;

            dir = d;
            time_total = present_straight + landing_time + hard_straight + getup_time;
            bcurve = data.CurveUsing;
            present_animation_factor = present_anim_time / present_straight;

            //need re-calculate between hurt and broken
            if (bcurve)
            {
                if (present.HitFly != null && present.HitCurves != null)
                {
                    IXCurve raw_h = ((change_to_fly || data.State == XBeHitState.Hit_Fly) && present.HitFly.Length > 0) ?
                        XResources.Load<XCurve>("Curve/" + present.CurveLocation + present.HitCurves[4], AssetType.Prefab) : null;
                    IXCurve raw_v = ((change_to_fly || data.State == XBeHitState.Hit_Fly) && present.HitFly.Length > 0) ?
                                     XResources.Load<XCurve>("Curve/" + present.CurveLocation + present.HitCurves[3], AssetType.Prefab) :
                                     ((data.State == XBeHitState.Hit_Roll && present.Hit_Roll.Length > 0) ?
                                       XResources.Load<XCurve>("Curve/" + present.CurveLocation + present.HitCurves[5], AssetType.Prefab) :
                                       (data.State == XBeHitState.Hit_Back ? XResources.Load<XCurve>("Curve/" + present.CurveLocation + present.HitCurves[(int)data.State_Animation], AssetType.Prefab) :
                                        XResources.Load<XCurve>("Curve/" + present.CurveLocation + present.HitCurves[0], AssetType.Prefab)));

                    curve_h = raw_h != null ? raw_h : null;
                    curve_v = raw_v;
                    curve_height_scale = (raw_h == null || raw_h.GetMaxValue() == 0) ? 1 : height / raw_h.GetMaxValue();
                    curve_offset_scale = (raw_v == null || raw_v.GetMaxValue() == 0) ? 1 : offset / raw_v.GetMaxValue();
                }
            }
        }

        if (data.Fx != null) PlayHitFx(transform, data.Fx, data.Fx_Follow);
        ReadyToGo();
        ator.speed = trigger == null ? 0 : present_animation_factor;
    }

    public void BuildAnimation(XHitData data)
    {
        string[] anims = null;
        switch (data.State)
        {
            case XBeHitState.Hit_Back:
                if (change_to_fly)
                {
                    anims = present.HitFly != null && present.HitFly.Length > 0 ? present.HitFly : present.Hit_f;
                }
                else
                {
                    switch (data.State_Animation)
                    {
                        case XBeHitState_Animation.Hit_Back_Front: anims = present.Hit_f; break;
                        case XBeHitState_Animation.Hit_Back_Left: anims = present.Hit_l; break;
                        case XBeHitState_Animation.Hit_Back_Right: anims = present.Hit_r; break;
                    }
                }
                break;
            case XBeHitState.Hit_Roll:
                anims = change_to_fly ?
                    present.HitFly != null && present.HitFly.Length > 0 ? present.HitFly : present.Hit_f :
                    present.HitFly != null && present.HitFly.Length > 0 ? present.Hit_Roll : present.Hit_f;
                break;
            case XBeHitState.Hit_Fly:
                anims = present.HitFly != null && present.HitFly.Length > 0 ? present.HitFly : present.Hit_f;
                break;
            case XBeHitState.Hit_Freezed:
                if (data.FreezePresent)
                {
                    string freeze = "Animation/" + present.AnimLocation + present.Freeze;
                    AnimationClip freeze_clip = XResources.Load<AnimationClip>(freeze, AssetType.Anim);
                    present_anim_time = freeze_clip.length;
                    controllder[Clip.Freezed] = freeze_clip;
                }
                return;
        }
        if (anims == null) return;
        int idx = 0;

        string clipname = "Animation/" + present.AnimLocation + anims[idx++];
        AnimationClip clip = XResources.Load<AnimationClip>(clipname, AssetType.Anim);
        present_anim_time = clip.length;
        controllder[Clip.PresentStraight] = clip;

        if ((change_to_fly || data.State == XBeHitState.Hit_Fly) && present.HitFly != null && present.HitFly.Length > 0)
        {
            clipname = "Animation/" + present.AnimLocation + anims[idx++];
            clip = XResources.Load<AnimationClip>(clipname, AssetType.Anim);
            landing_time = clip.length;
        }
        else
        {
            landing_time = 0;
        }

        clipname = "Animation/" + present.AnimLocation + anims[idx++];
        clip = XResources.Load<AnimationClip>(clipname, AssetType.Anim);
        controllder[Clip.HardStraight] = clip;
        loop_hard = (clip.wrapMode == WrapMode.Loop);
        hard_straight_time = clip.length;

        clipname = "Animation/" + present.AnimLocation + anims[idx++];
        clip = XResources.Load<AnimationClip>(clipname, AssetType.Anim);
        getup_time = clip.length;
        controllder[Clip.GetUp] = clip;
    }

    public void Cancel()
    {
        elapsed = 0;
        rticalV = 0;
        gravity = 0;
        data = null;
        hoster = null;

        ator.speed = 1;
        DestroyFx();
        ator.SetTrigger(AnimTriger.ToStand);
    }

    public void ReadyToGo()
    {
        if (data.State == XBeHitState.Hit_Freezed) return;
        pos.x = transform.position.x;
        pos.y = transform.position.z;
        elapsed = 0;
        Vector3 destination = transform.position + dir * offset;
        des.x = destination.x;
        des.y = destination.z;
        phase = XBeHitPhase.Hit_Present;
        trigger = data.State == XBeHitState.Hit_Freezed ? (data.FreezePresent ? AnimTriger.ToFreezed : null) : AnimTriger.ToBeHit;
        if (bcurve)
        {
            curve_height_time_scale = curve_h == null ? 1 : present_straight / curve_h.GetTime(curve_h.length - 1);
            curve_offset_time_scale = curve_v == null ? 1 : present_straight / curve_v.GetTime(curve_v.length - 1);
            last_offset = 0;
            last_height = 0;
        }
        else
        {
            factor = XCommon.singleton.GetSmoothFactor((pos - des).magnitude, present_straight, 0.01f);
            rticalV = ((!change_to_fly && data.State != XBeHitState.Hit_Fly)) ? 0 : (height * 4.0f) / present_straight;
            gravity = rticalV / present_straight * 2.0f;
        }
    }

    public void CalcDeltaPos(Vector3 position, float deltaTime, float last_elapsed)
    {
        Vector2 delta = Vector2.zero;

        if (bcurve)
        {
            float ev = elapsed / curve_offset_time_scale;
            float eh = elapsed / curve_height_time_scale;

            float c_v = curve_v == null ? 0 : curve_v.Evaluate(ev) * curve_offset_scale;
            float c_h = curve_h == null ? 0 : curve_h.Evaluate(eh) * curve_height_scale;

            Vector3 v = dir * (c_v - last_offset);
            delta.x = v.x; delta.y = v.z;
            last_height = c_h;
            last_offset = c_v;
        }
        else
        {
            pos = position;
            delta = (des - pos) * Mathf.Min(1.0f, factor * deltaTime);
        }

        delta_x = delta.x;
        delta_z = delta.y;
    }


    public void PlayHitFx(Transform trans, string fx, bool follow)
    {
        if (string.IsNullOrEmpty(fx)) return;

        hit_fx = XResources.Load<GameObject>(fx, AssetType.Prefab);
        binded_bone = trans.Find("Bip001/Bip001 Pelvis/Bip001 Spine");
        Transform parent = (binded_bone == null) ? trans : binded_bone;

        if (follow)
        {
            hit_fx.transform.parent = parent;
            hit_fx.transform.localPosition = Vector3.zero;
            hit_fx.transform.localRotation = Quaternion.identity;
            hit_fx.transform.localScale = Vector3.one;
        }
        else
        {
            hit_fx.transform.position = parent.position;
            hit_fx.transform.rotation = parent.rotation;
        }

        ParticleSystem[] systems = hit_fx.GetComponentsInChildren<ParticleSystem>();
        foreach (ParticleSystem system in systems)
        {
            system.Play();
        }
    }

    public void DestroyFx()
    {
        if (hit_fx != null)
        {
            ParticleSystem[] systems = hit_fx.GetComponentsInChildren<ParticleSystem>();
            foreach (ParticleSystem system in systems)
            {
                system.Stop();
            }
            hit_fx.transform.parent = null;
            GameObject.Destroy(hit_fx);
        }
        hit_fx = null;
    }

}
