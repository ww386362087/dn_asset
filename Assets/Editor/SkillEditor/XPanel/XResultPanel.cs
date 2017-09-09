using UnityEditor;
using UnityEngine;

public class XVector3
{
    private Vector3 _inner_pos;

    public XVector3(float x, float y, float z)
    {
        _inner_pos = new Vector3(x, y, z);
    }

    public Vector3 FirePos
    {
        get { return _inner_pos; }
        set { _inner_pos = value; }
    }
}


class XResultPanel : XPanel
{
   
 
    GUIStyle _myLabelStyle2 = null;

    XVector3 _xv = new XVector3(0, 0, 0);
    
    protected override int Count
    {
        get { return Hoster.SkillData.Result == null ? -1 : Hoster.SkillData.Result.Count; }
    }

    protected override string PanelName
    {
        get { return "Result"; }
    }

    protected override bool FoldOut
    {
        get { return Hoster.EditorData.XResult_foldout; }
        set { Hoster.EditorData.XResult_foldout = value; }
    }

    protected override void OnInnerGUI()
    {
        if (_myLabelStyle2 == null)
        {
            _myLabelStyle2 = new GUIStyle(GUI.skin.label);
            _myLabelStyle2.fontStyle = FontStyle.Italic;
        }

        if (Hoster.SkillData.Result == null) return;

        for (int i = 0; i < Hoster.SkillData.Result.Count; i++)
        {
            Hoster.SkillData.Result[i].Index = i;
            float result_at = (Hoster.SkillData.Result[i].At / XSkillPanel.frame);

            EditorGUILayout.BeginHorizontal();
            Hoster.SkillData.Result[i].LongAttackEffect = EditorGUILayout.Toggle("Long Range Attack", Hoster.SkillData.Result[i].LongAttackEffect);
            if (GUILayout.Button(_content_remove, GUILayout.MaxWidth(30), GUILayout.MinWidth(30)))
            {
                Hoster.SkillData.Result.RemoveAt(i);
                Hoster.ConfigData.Result.RemoveAt(i);
                EditorGUILayout.EndHorizontal();
                continue;
            }
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.Space();

            if (Hoster.SkillData.Result[i].LongAttackEffect && Hoster.SkillData.Result[i].LongAttackData == null) Hoster.SkillData.Result[i].LongAttackData = new XLongAttackResultData();

            EditorGUILayout.LabelField("Affect basic configuration.", _myLabelStyle2);
            if (Hoster.SkillData.Result[i].LongAttackEffect)
                Hoster.SkillData.Result[i].LongAttackData.Type = (XResultBulletType)EditorGUILayout.EnumPopup("Type", Hoster.SkillData.Result[i].LongAttackData.Type);
            EditorGUILayout.BeginHorizontal();
            result_at = EditorGUILayout.FloatField(Hoster.SkillData.Result[i].LongAttackEffect ? "Fire At" : "Triggered At", result_at);
            GUILayout.Label("(frame)");
            EditorGUILayout.EndHorizontal();
            Hoster.ConfigData.Result[i].Result_Ratio = result_at / Hoster.SkillDataExtra.SkillClip_Frame;
            if (Hoster.ConfigData.Result[i].Result_Ratio > 1) Hoster.ConfigData.Result[i].Result_Ratio = 1;

            Hoster.ConfigData.Result[i].Result_Ratio = EditorGUILayout.Slider("Ratio", Hoster.ConfigData.Result[i].Result_Ratio, 0, 1);
            Hoster.SkillData.Result[i].At = (Hoster.ConfigData.Result[i].Result_Ratio * Hoster.SkillDataExtra.SkillClip_Frame) * XSkillPanel.frame;
            if (Hoster.SkillData.Result[i].LongAttackEffect &&
                (Hoster.SkillData.Result[i].LongAttackData.Type != XResultBulletType.Satellite && Hoster.SkillData.Result[i].LongAttackData.Type != XResultBulletType.Ring))
            {
                _xv.FirePos = new Vector3(Hoster.SkillData.Result[i].LongAttackData.At_X, Hoster.SkillData.Result[i].LongAttackData.At_Y, Hoster.SkillData.Result[i].LongAttackData.At_Z);
                PropertyField[] field = new PropertyField[1];
                field[0] = new PropertyField(_xv, typeof(XVector3).GetProperty("FirePos"), SerializedPropertyType.Vector3);
                ExposeProperties.Expose(field);

                Hoster.SkillData.Result[i].LongAttackData.At_X = _xv.FirePos.x;
                Hoster.SkillData.Result[i].LongAttackData.At_Y = _xv.FirePos.y;
                Hoster.SkillData.Result[i].LongAttackData.At_Z = _xv.FirePos.z;
            }
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Affect logical configuration.", _myLabelStyle2);
            Hoster.SkillData.Result[i].Loop = EditorGUILayout.Toggle("Loop", Hoster.SkillData.Result[i].Loop);
            if (Hoster.SkillData.Result[i].Loop)
            {
                float loop = (Hoster.SkillData.Result[i].Cycle / XSkillPanel.frame);
                EditorGUILayout.BeginHorizontal();
                loop = EditorGUILayout.FloatField("Repeated Cycle", loop);
                GUILayout.Label("(frame)");
                GUILayout.Label("", GUILayout.MaxWidth(30));
                EditorGUILayout.EndHorizontal();
                Hoster.SkillData.Result[i].Cycle = loop * XSkillPanel.frame;
                int count = EditorGUILayout.IntField("Repeated Count", Hoster.SkillData.Result[i].Loop_Count);
                if (count < 100 && count >= 0) Hoster.SkillData.Result[i].Loop_Count = count;
            }

            if (Hoster.SkillData.Result[i].Loop || Hoster.SkillData.Result[i].Group) EditorGUILayout.Space();

            Hoster.SkillData.Result[i].Group = EditorGUILayout.Toggle("Group", Hoster.SkillData.Result[i].Group);
            if (Hoster.SkillData.Result[i].Group)
            {
                Hoster.SkillData.Result[i].Clockwise = EditorGUILayout.Toggle("ClockWise", Hoster.SkillData.Result[i].Clockwise);
                Hoster.SkillData.Result[i].Deviation_Angle = EditorGUILayout.IntField("Deviation Angle", Hoster.SkillData.Result[i].Deviation_Angle);
                Hoster.SkillData.Result[i].Angle_Step = EditorGUILayout.IntField("Angle Step", Hoster.SkillData.Result[i].Angle_Step);

                float step = (Hoster.SkillData.Result[i].Time_Step / XSkillPanel.frame);
                EditorGUILayout.BeginHorizontal();
                step = EditorGUILayout.FloatField("Time Step", step);
                GUILayout.Label("(frame)");
                GUILayout.Label("", GUILayout.MaxWidth(30));
                EditorGUILayout.EndHorizontal();
                Hoster.SkillData.Result[i].Time_Step = step * XSkillPanel.frame;

                int count = EditorGUILayout.IntField("Count", Hoster.SkillData.Result[i].Group_Count);
                if (count < 100 && count >= 0) Hoster.SkillData.Result[i].Group_Count = count;
            }

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Affect area configuration.", _myLabelStyle2);

            if (Hoster.SkillData.NeedTarget)
                Hoster.SkillData.Result[i].Attack_Only_Target = EditorGUILayout.Toggle("Attack Only Target", Hoster.SkillData.Result[i].Attack_Only_Target);
            else
                Hoster.SkillData.Result[i].Attack_Only_Target = false;

            if (Hoster.SkillData.Result[i].LongAttackEffect && !Hoster.SkillData.Result[i].Attack_Only_Target)
                Hoster.SkillData.Result[i].Attack_All = EditorGUILayout.Toggle("Attack All", Hoster.SkillData.Result[i].Attack_All);
            else
                Hoster.SkillData.Result[i].Attack_All = false;
            if (Hoster.SkillData.Result[i].Attack_All)
            {
                Hoster.SkillData.Result[i].Mobs_Inclusived = EditorGUILayout.Toggle("Include Mobs", Hoster.SkillData.Result[i].Mobs_Inclusived);
            }
            EditorGUILayout.Space();
            if (!Hoster.SkillData.Result[i].LongAttackEffect || Hoster.SkillData.Result[i].LongAttackData.TriggerAtEnd)
            {
                Hoster.SkillData.Result[i].Sector_Type = EditorGUILayout.Toggle("Sector Damage", Hoster.SkillData.Result[i].Sector_Type);
                if (Hoster.SkillData.Result[i].Sector_Type)
                {
                    Hoster.SkillData.Result[i].Low_Range = EditorGUILayout.FloatField("Range (↓)", Hoster.SkillData.Result[i].Low_Range);
                    Hoster.SkillData.Result[i].Range = EditorGUILayout.FloatField("Range (↑)", Hoster.SkillData.Result[i].Range);
                    Hoster.SkillData.Result[i].Scope = EditorGUILayout.FloatField("Scope", Hoster.SkillData.Result[i].Scope);
                }
                else
                {
                    Hoster.SkillData.Result[i].Rect_HalfEffect = EditorGUILayout.Toggle("Half Effect", Hoster.SkillData.Result[i].Rect_HalfEffect);
                    Hoster.SkillData.Result[i].None_Sector_Angle_Shift = EditorGUILayout.IntField("Angle Shift", Hoster.SkillData.Result[i].None_Sector_Angle_Shift);
                    Hoster.SkillData.Result[i].Range = EditorGUILayout.FloatField("Depth", Hoster.SkillData.Result[i].Range);
                    Hoster.SkillData.Result[i].Scope = EditorGUILayout.FloatField("Width", Hoster.SkillData.Result[i].Scope);
                }
                if (Hoster.SkillData.Result[i].Low_Range > Hoster.SkillData.Result[i].Range)
                {
                    Hoster.SkillData.Result[i].Low_Range = Hoster.SkillData.Result[i].Range;
                }
                Hoster.SkillData.Result[i].Offset_X = EditorGUILayout.FloatField("OffsetX", Hoster.SkillData.Result[i].Offset_X);
                Hoster.SkillData.Result[i].Offset_Z = EditorGUILayout.FloatField("OffsetZ", Hoster.SkillData.Result[i].Offset_Z);

                EditorGUILayout.BeginHorizontal();
                GUILayout.FlexibleSpace();
                if (GUILayout.Button("view"))
                {
                    Hoster.nHotID = i;
                }
                EditorGUILayout.EndHorizontal();
            }
            else
                EditorGUILayout.Space();

            if (Hoster.SkillData.Result[i].LongAttackEffect)
            {
                EditorGUILayout.LabelField("Collision setting", _myLabelStyle2);
                Hoster.SkillData.Result[i].LongAttackData.StaticCollider = EditorGUILayout.Toggle("Static Enabled", Hoster.SkillData.Result[i].LongAttackData.StaticCollider);
                Hoster.SkillData.Result[i].LongAttackData.DynamicCollider = EditorGUILayout.Toggle("Dynamic Enabled", Hoster.SkillData.Result[i].LongAttackData.DynamicCollider);

                EditorGUILayout.Space();
                EditorGUILayout.LabelField("Bullet's setting", _myLabelStyle2);
                if (Hoster.SkillData.Result[i].LongAttackData.Type != XResultBulletType.Satellite && Hoster.SkillData.Result[i].LongAttackData.Type != XResultBulletType.Ring)
                    Hoster.SkillData.Result[i].LongAttackData.IsPingPong = EditorGUILayout.Toggle("Is PingPong", Hoster.SkillData.Result[i].LongAttackData.IsPingPong);

                if (Hoster.SkillData.Result[i].LongAttackData.IsPingPong)
                    Hoster.SkillData.Result[i].LongAttackData.AutoRefine_at_Half = EditorGUILayout.Toggle("Auto Refine at Half", Hoster.SkillData.Result[i].LongAttackData.AutoRefine_at_Half);
                else
                    Hoster.SkillData.Result[i].LongAttackData.AutoRefine_at_Half = false;
                Hoster.SkillData.Result[i].LongAttackData.WithCollision = EditorGUILayout.Toggle("With Collision", Hoster.SkillData.Result[i].LongAttackData.WithCollision);
                if (Hoster.SkillData.Result[i].LongAttackData.WithCollision)
                {
                    Hoster.SkillData.Result[i].LongAttackData.TriggerOnce = EditorGUILayout.Toggle("Once", Hoster.SkillData.Result[i].LongAttackData.TriggerOnce);
                    if (!Hoster.SkillData.Result[i].LongAttackData.TriggerOnce)
                    {
                        EditorGUILayout.BeginHorizontal();
                        Hoster.SkillData.Result[i].LongAttackData.Refine_Cycle = EditorGUILayout.FloatField("Refine Cycle", Hoster.SkillData.Result[i].LongAttackData.Refine_Cycle);
                        GUILayout.Label("(s) zero means never.");
                        GUILayout.Label("", GUILayout.MaxWidth(30));
                        EditorGUILayout.EndHorizontal();
                        if (Hoster.SkillData.Result[i].LongAttackData.Refine_Cycle > 0)
                        {
                            Hoster.SkillData.Result[i].LongAttackData.Refine_Count = EditorGUILayout.IntField("Refine Count", Hoster.SkillData.Result[i].LongAttackData.Refine_Count);
                            if (Hoster.SkillData.Result[i].LongAttackData.Refine_Count < 1) Hoster.SkillData.Result[i].LongAttackData.Refine_Count = 1;
                        }
                    }
                }
                if (Hoster.SkillData.Result[i].LongAttackData.Type != XResultBulletType.Ring)
                {
                    EditorGUILayout.Space();
                    EditorGUILayout.LabelField("Manipulation setting", _myLabelStyle2);
                    Hoster.SkillData.Result[i].LongAttackData.Manipulation = EditorGUILayout.Toggle("Has Manipulation", Hoster.SkillData.Result[i].LongAttackData.Manipulation);
                    if (Hoster.SkillData.Result[i].LongAttackData.Manipulation)
                    {
                        Hoster.SkillData.Result[i].LongAttackData.ManipulationRadius = EditorGUILayout.FloatField("Radius", Hoster.SkillData.Result[i].LongAttackData.ManipulationRadius);
                        Hoster.SkillData.Result[i].LongAttackData.ManipulationForce = EditorGUILayout.FloatField("Force", Hoster.SkillData.Result[i].LongAttackData.ManipulationForce);
                    }
                    else
                    {
                        Hoster.SkillData.Result[i].LongAttackData.ManipulationRadius = 0;
                        Hoster.SkillData.Result[i].LongAttackData.ManipulationForce = 0;
                    }
                }

                EditorGUILayout.Space();
                if (Hoster.SkillData.NeedTarget && Hoster.SkillData.Result[i].LongAttackData.Type != XResultBulletType.Satellite && Hoster.SkillData.Result[i].LongAttackData.Type != XResultBulletType.Ring)
                {
                    Hoster.SkillData.Result[i].LongAttackData.Follow = EditorGUILayout.Toggle("Follow", Hoster.SkillData.Result[i].LongAttackData.Follow);
                }
                EditorGUILayout.BeginHorizontal();
                Hoster.SkillData.Result[i].LongAttackData.Runningtime = EditorGUILayout.FloatField("Running Time", Hoster.SkillData.Result[i].LongAttackData.Runningtime);
                GUILayout.Label("(s)");
                GUILayout.Label("", GUILayout.MaxWidth(30));
                EditorGUILayout.EndHorizontal();

                if (Hoster.SkillData.Result[i].LongAttackData.Type != XResultBulletType.Satellite &&
                    Hoster.SkillData.Result[i].LongAttackData.Type != XResultBulletType.Ring)
                {
                    EditorGUILayout.BeginHorizontal();
                    Hoster.SkillData.Result[i].LongAttackData.Stickytime = EditorGUILayout.FloatField("Sticky Time", Hoster.SkillData.Result[i].LongAttackData.Stickytime);
                    GUILayout.Label("(s)");
                    GUILayout.Label("", GUILayout.MaxWidth(30));
                    EditorGUILayout.EndHorizontal();

                    if (!Hoster.SkillData.Result[i].Warning) Hoster.SkillData.Result[i].LongAttackData.Velocity = EditorGUILayout.FloatField("Velocity", Hoster.SkillData.Result[i].LongAttackData.Velocity);
                }
                else
                {
                    if (Hoster.SkillData.Result[i].LongAttackData.Type == XResultBulletType.Satellite)
                    {
                        if (!Hoster.SkillData.Result[i].Warning)
                        {
                            EditorGUILayout.BeginHorizontal();
                            Hoster.SkillData.Result[i].LongAttackData.Palstance = EditorGUILayout.FloatField("Palstance", Hoster.SkillData.Result[i].LongAttackData.Palstance);
                            GUILayout.Label("(degree/s)");
                            GUILayout.Label("", GUILayout.MaxWidth(30));
                            EditorGUILayout.EndHorizontal();
                        }
                        EditorGUILayout.BeginHorizontal();
                        Hoster.SkillData.Result[i].LongAttackData.RingRadius = EditorGUILayout.FloatField("Ring Radius", Hoster.SkillData.Result[i].LongAttackData.RingRadius);
                        GUILayout.Label("(m)");
                        GUILayout.Label("", GUILayout.MaxWidth(30));
                        EditorGUILayout.EndHorizontal();
                    }
                    else if (Hoster.SkillData.Result[i].LongAttackData.Type == XResultBulletType.Ring)
                    {
                        if (!Hoster.SkillData.Result[i].Warning)
                        {
                            EditorGUILayout.BeginHorizontal();
                            Hoster.SkillData.Result[i].LongAttackData.RingVelocity = EditorGUILayout.FloatField("Ring Velocity", Hoster.SkillData.Result[i].LongAttackData.RingVelocity);
                            GUILayout.Label("(m/s)");
                            GUILayout.Label("", GUILayout.MaxWidth(30));
                            EditorGUILayout.EndHorizontal();
                        }
                        EditorGUILayout.BeginHorizontal();
                        Hoster.SkillData.Result[i].LongAttackData.RingRadius = EditorGUILayout.FloatField("Ring Width", Hoster.SkillData.Result[i].LongAttackData.RingRadius);
                        GUILayout.Label("(m)");
                        GUILayout.Label("", GUILayout.MaxWidth(30));
                        EditorGUILayout.EndHorizontal();
                        Hoster.SkillData.Result[i].LongAttackData.RingFull = EditorGUILayout.Toggle("Is Ring Full", Hoster.SkillData.Result[i].LongAttackData.RingFull);
                    }
                }

                if (Hoster.SkillData.Result[i].LongAttackData.WithCollision && Hoster.SkillData.Result[i].LongAttackData.Type != XResultBulletType.Ring)
                {
                    EditorGUILayout.BeginHorizontal();
                    Hoster.SkillData.Result[i].LongAttackData.Radius = EditorGUILayout.FloatField("Radius", Hoster.SkillData.Result[i].LongAttackData.Radius);
                    GUILayout.Label("(m)");
                    GUILayout.Label("", GUILayout.MaxWidth(30));
                    EditorGUILayout.EndHorizontal();
                }
                if (Hoster.SkillData.Result[i].LongAttackData.Type != XResultBulletType.Ring) Hoster.SkillData.Result[i].LongAttackData.FireAngle = EditorGUILayout.IntField("Angle (clockwise)", Hoster.SkillData.Result[i].LongAttackData.FireAngle);
                EditorGUILayout.Space();
                if (Hoster.SkillData.Result[i].LongAttackData.Type != XResultBulletType.Ring)
                    Hoster.SkillData.Result[i].LongAttackData.FlyWithTerrain = EditorGUILayout.Toggle("Fly with Terrain", Hoster.SkillData.Result[i].LongAttackData.FlyWithTerrain);
                else
                    Hoster.SkillData.Result[i].LongAttackData.FlyWithTerrain = false;
                if (Hoster.SkillData.NeedTarget &&
                    (Hoster.SkillData.Result[i].LongAttackData.Type != XResultBulletType.Satellite && Hoster.SkillData.Result[i].LongAttackData.Type != XResultBulletType.Ring))
                    Hoster.SkillData.Result[i].LongAttackData.AimTargetCenter = EditorGUILayout.Toggle("Aim Target Center", Hoster.SkillData.Result[i].LongAttackData.AimTargetCenter);

                EditorGUILayout.Space();
                if (Hoster.SkillData.Result[i].LongAttackData.Type != XResultBulletType.Ring)
                {
                    if (!Hoster.SkillData.Result[i].LongAttackData.IsPingPong)
                        Hoster.SkillData.Result[i].LongAttackData.TriggerAtEnd = EditorGUILayout.Toggle("Trigger At End", Hoster.SkillData.Result[i].LongAttackData.TriggerAtEnd);
                    else
                        Hoster.SkillData.Result[i].LongAttackData.TriggerAtEnd = false;
                }
                else
                {
                    Hoster.SkillData.Result[i].LongAttackData.TriggerAtEnd = false;
                }

                if (Hoster.SkillData.Result[i].LongAttackData.TriggerAtEnd)
                {
                    float cycle = (Hoster.SkillData.Result[i].LongAttackData.TriggerAtEnd_Cycle / XSkillPanel.frame);
                    EditorGUILayout.BeginHorizontal();
                    cycle = EditorGUILayout.FloatField("Trigger End Cycle", cycle);
                    GUILayout.Label("(frame)");
                    GUILayout.Label("", GUILayout.MaxWidth(30));
                    EditorGUILayout.EndHorizontal();
                    Hoster.SkillData.Result[i].LongAttackData.TriggerAtEnd_Cycle = cycle * XSkillPanel.frame;
                    Hoster.SkillData.Result[i].LongAttackData.TriggerAtEnd_Count = EditorGUILayout.IntField("Trigger End Repeated Count", Hoster.SkillData.Result[i].LongAttackData.TriggerAtEnd_Count);
                }

                EditorGUILayout.Space();
                EditorGUILayout.LabelField("Bullet presentation configuration.", _myLabelStyle2);
                GameObject o = EditorGUILayout.ObjectField("Prefab", Hoster.SkillDataExtra.ResultEx[i].BulletPrefab, typeof(GameObject), true) as GameObject;
                {
                    if (XInnerEditor.CheckPrefab(o) && AssetDatabase.GetAssetPath(o).Contains("Prefabs/Bullets/"))
                    {
                        Hoster.SkillDataExtra.ResultEx[i].BulletPrefab = o;
                        string path = AssetDatabase.GetAssetPath(o).Remove(0, 17);
                        Hoster.SkillData.Result[i].LongAttackData.Prefab = path.Remove(path.LastIndexOf('.'));
                    }
                    else
                    {
                        Hoster.SkillDataExtra.ResultEx[i].BulletPrefab = null;
                        Hoster.SkillData.Result[i].LongAttackData.Prefab = null;
                    }
                }
                EditorGUILayout.Space();
                if (!Hoster.SkillData.Result[i].LongAttackData.FlyWithTerrain && Hoster.SkillData.Result[i].LongAttackData.Type != XResultBulletType.Ring)
                {
                    GameObject hitfx = EditorGUILayout.ObjectField("Hit Ground Fx", Hoster.SkillDataExtra.ResultEx[i].BulletHitGroundFx, typeof(GameObject), true) as GameObject;
                    {
                        if (hitfx == null || !AssetDatabase.GetAssetPath(hitfx).Contains("Resources/Effects/"))
                        {
                            Hoster.SkillData.Result[i].LongAttackData.HitGround_Fx = null;
                            Hoster.SkillDataExtra.ResultEx[i].BulletHitGroundFx = null;
                        }
                        else
                        {
                            Hoster.SkillDataExtra.ResultEx[i].BulletHitGroundFx = hitfx;
                            string path = AssetDatabase.GetAssetPath(hitfx).Remove(0, 17);
                            Hoster.SkillData.Result[i].LongAttackData.HitGround_Fx = path.Remove(path.LastIndexOf('.'));
                        }
                    }
                    if (Hoster.SkillData.Result[i].LongAttackData.HitGround_Fx != null)
                    {
                        Hoster.SkillData.Result[i].LongAttackData.HitGroundFx_LifeTime = EditorGUILayout.FloatField("Hit Ground Fx Duration", Hoster.SkillData.Result[i].LongAttackData.HitGroundFx_LifeTime);
                    }
                }

                GameObject fx = EditorGUILayout.ObjectField("End Fx", Hoster.SkillDataExtra.ResultEx[i].BulletEndFx, typeof(GameObject), true) as GameObject;
                {
                    if (fx == null || !AssetDatabase.GetAssetPath(fx).Contains("Resources/Effects/"))
                    {
                        Hoster.SkillData.Result[i].LongAttackData.End_Fx = null;
                        Hoster.SkillDataExtra.ResultEx[i].BulletEndFx = null;
                    }
                    else
                    {
                        Hoster.SkillDataExtra.ResultEx[i].BulletEndFx = fx;
                        string path = AssetDatabase.GetAssetPath(fx).Remove(0, 17);
                        Hoster.SkillData.Result[i].LongAttackData.End_Fx = path.Remove(path.LastIndexOf('.'));
                    }
                }
                if (Hoster.SkillData.Result[i].LongAttackData.End_Fx != null)
                {
                    Hoster.SkillData.Result[i].LongAttackData.EndFx_LifeTime = EditorGUILayout.FloatField("End Fx Duration", Hoster.SkillData.Result[i].LongAttackData.EndFx_LifeTime);
                    Hoster.SkillData.Result[i].LongAttackData.EndFx_Ground = EditorGUILayout.Toggle("End Fx Ground", Hoster.SkillData.Result[i].LongAttackData.EndFx_Ground);
                }

                EditorGUILayout.Space();

                Hoster.SkillData.Result[i].LongAttackData.Audio = EditorGUILayout.TextField("Audio", Hoster.SkillData.Result[i].LongAttackData.Audio);
                if (!string.IsNullOrEmpty(Hoster.SkillData.Result[i].LongAttackData.Audio))
                {
                    Hoster.SkillData.Result[i].LongAttackData.Audio_Channel = AudioChannel.Motion;
                    EditorGUILayout.EnumPopup("Channel", Hoster.SkillData.Result[i].LongAttackData.Audio_Channel);
                }
                Hoster.SkillData.Result[i].LongAttackData.End_Audio = EditorGUILayout.TextField("End Audio", Hoster.SkillData.Result[i].LongAttackData.End_Audio);
                if (!string.IsNullOrEmpty(Hoster.SkillData.Result[i].LongAttackData.End_Audio))
                {
                    Hoster.SkillData.Result[i].LongAttackData.End_Audio_Channel = AudioChannel.Motion;
                    EditorGUILayout.EnumPopup("Channel", Hoster.SkillData.Result[i].LongAttackData.End_Audio_Channel);
                }
                EditorGUILayout.Space();
            }

            EditorGUILayout.LabelField("Other settings.", _myLabelStyle2);
            Hoster.SkillData.Result[i].Warning = EditorGUILayout.Toggle("Has Warning", Hoster.SkillData.Result[i].Warning);
            if (Hoster.SkillData.Result[i].Warning)
            {
                Hoster.SkillData.Result[i].Warning_Idx = EditorGUILayout.IntField("Waring Idx", Hoster.SkillData.Result[i].Warning_Idx);
            }

            if (Hoster.SkillData.Result[i].LongAttackEffect)
                Hoster.SkillData.Result[i].Affect_Direction = XResultAffectDirection.AttackDir;
            else
                Hoster.SkillData.Result[i].Affect_Direction = (XResultAffectDirection)EditorGUILayout.EnumPopup("Result Affect Direction", Hoster.SkillData.Result[i].Affect_Direction);

            if (i != Hoster.SkillData.Result.Count - 1)
            {
                GUILayout.Box("", line);
                EditorGUILayout.Space();
            }
        }
    }
}
