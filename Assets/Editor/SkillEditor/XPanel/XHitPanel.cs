using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;


public class XHitPanel : XPanel
{

    protected override int Count
    {
        get { return Hoster.SkillData.Hit == null ? -1 : Hoster.SkillData.Hit.Count; }
    }

    protected override bool FoldOut
    {
        get { return Hoster.EditorData.XHit_foldout; }
        set { Hoster.EditorData.XHit_foldout = value; }
    }

    protected override string PanelName
    {
        get { return Hoster.SkillData.TypeToken == 3 ? "Hit Dummy Settings" : "Hit Effect"; }
    }

    public override void Add()
    {
        if (Hoster.SkillData.Hit == null) Hoster.SkillData.Hit = new List<XHitData>();
        Hoster.SkillData.Hit.Add(new XHitData());
        Hoster.SkillDataExtra.Add<XHitDataExtraEx>();
        Hoster.EditorData.XHit_foldout = true;
    }

    GUIStyle _myStyle = null;
    GUIStyle _myLabelStyle = null;
    GUIStyle _myLabelStyle2 = null;

    private void DummySettings()
    {
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Dummy Id", _myLabelStyle);
        GUILayout.FlexibleSpace();
        EditorGUI.BeginChangeCheck();
        int idx = EditorGUILayout.IntField((int)Hoster.ConfigData.Dummy);
        Hoster.SkillDataExtra.Dummy = XEditorLibrary.GetDummy((uint)idx);
        Hoster.ConfigData.Dummy = idx;

        if (Hoster.SkillDataExtra.Dummy != null)
        {
            GUILayout.FlexibleSpace();
            EditorGUILayout.LabelField(Hoster.SkillDataExtra.Dummy.name, _myLabelStyle, new GUILayoutOption[] { GUILayout.MaxWidth(150) });

            if (GUILayout.Button("add"))
            {
                GameObject hitter = UnityEngine.Object.Instantiate(Hoster.SkillDataExtra.Dummy, Hoster.transform.position, Quaternion.identity) as GameObject;
                hitter.AddComponent<XSkillHit>().PresentID = (int)Hoster.ConfigData.Dummy;
            }
        }
        EditorGUILayout.EndHorizontal();
    }

    protected override void OnInnerGUI()
    {
        if (_myStyle == null)
        {
            _myStyle = new GUIStyle(EditorStyles.foldout);
            _myStyle.margin.left = 30;
        }

        if (_myLabelStyle == null)
        {
            _myLabelStyle = new GUIStyle(GUI.skin.label);
            _myLabelStyle.padding.left = 19;
        }

        if (Hoster.SkillData.TypeToken == 3)
        {
            DummySettings();
            return;
        }
        else
        {
            Hoster.EditorData.XHitDummy_foldout = EditorGUILayout.Foldout(Hoster.EditorData.XHitDummy_foldout, "Hit Dummy Settings", _myStyle);
            if (Hoster.EditorData.XHitDummy_foldout)
            {
                DummySettings();
            }
        }
        EditorGUILayout.Space();

        if (_myLabelStyle2 == null)
        {
            _myLabelStyle2 = new GUIStyle(GUI.skin.label);
            _myLabelStyle2.fontStyle = FontStyle.Italic;
        }

        if (Hoster.SkillData.Hit == null) return;

        for (int i = 0; i < Hoster.SkillData.Hit.Count; i++)
        {
            Hoster.SkillData.Hit[i].Index = i;

            if (Hoster.SkillData.Hit[i].State != XBeHitState.Hit_Freezed)
            {
                Hoster.SkillData.Hit[i].CurveUsing = EditorGUILayout.Toggle("Using Curve", Hoster.SkillData.Hit[i].CurveUsing);
                if (Hoster.SkillData.Hit[i].CurveUsing)
                {
                    EditorGUILayout.LabelField("Entity's hit-track will be calculated according to a specific curve", _myLabelStyle2);
                }

                EditorGUILayout.Space();
            }

            EditorGUILayout.LabelField("Skill Hit Parameters' setting", _myLabelStyle2);

            EditorGUILayout.BeginHorizontal();
            Hoster.SkillData.Hit[i].State = (XBeHitState)EditorGUILayout.EnumPopup("Type", Hoster.SkillData.Hit[i].State);
            if (GUILayout.Button(_content_remove, GUILayout.MaxWidth(30)))
            {
                Hoster.SkillData.Hit.RemoveAt(i);
                Hoster.SkillDataExtra.HitEx.RemoveAt(i);
            }
            EditorGUILayout.EndHorizontal();

            if (i >= Hoster.SkillData.Hit.Count) continue;

            if (Hoster.SkillData.Hit[i].State != XBeHitState.Hit_Free)
            {
                if (Hoster.SkillData.Hit[i].State == XBeHitState.Hit_Back)
                {
                    Hoster.SkillData.Hit[i].State_Animation = (XBeHitState_Animation)EditorGUILayout.EnumPopup("Animation Type", Hoster.SkillData.Hit[i].State_Animation);
                }
                EditorGUILayout.Space();

                if (i < Hoster.SkillData.Hit.Count)
                {
                    if (Hoster.SkillData.Hit[i].State == XBeHitState.Hit_Freezed)
                    {
                        Hoster.SkillData.Hit[i].FreezePresent = EditorGUILayout.Toggle("Present", Hoster.SkillData.Hit[i].FreezePresent);
                        Hoster.SkillData.Hit[i].FreezeDuration = EditorGUILayout.FloatField("Duration", Hoster.SkillData.Hit[i].FreezeDuration);
                    }
                    else
                    {
                        EditorGUILayout.BeginHorizontal();
                        Hoster.SkillData.Hit[i].Time_Present_Straight = EditorGUILayout.FloatField("Present Straight", Hoster.SkillData.Hit[i].Time_Present_Straight);
                        EditorGUILayout.LabelField("s", GUILayout.MaxWidth(30));
                        EditorGUILayout.EndHorizontal();
                        EditorGUILayout.BeginHorizontal();
                        Hoster.SkillData.Hit[i].Time_Hard_Straight = EditorGUILayout.FloatField("Hard Straight", Hoster.SkillData.Hit[i].Time_Hard_Straight);
                        EditorGUILayout.LabelField("s", GUILayout.MaxWidth(30));
                        EditorGUILayout.EndHorizontal();

                        EditorGUILayout.Space();
                        EditorGUILayout.BeginHorizontal();
                        Hoster.SkillData.Hit[i].Random_Range = EditorGUILayout.Slider("Random Range (offline)", Hoster.SkillData.Hit[i].Random_Range, 0, 0.5f);
                        EditorGUILayout.LabelField("%", GUILayout.MaxWidth(30));
                        EditorGUILayout.EndHorizontal();

                        EditorGUILayout.Space();
                        EditorGUILayout.BeginHorizontal();
                        Hoster.SkillData.Hit[i].Offset = EditorGUILayout.FloatField("Offset", Hoster.SkillData.Hit[i].Offset);
                        EditorGUILayout.LabelField("m", GUILayout.MaxWidth(30));
                        EditorGUILayout.EndHorizontal();
                        if (Hoster.SkillData.Hit[i].State == XBeHitState.Hit_Fly)
                        {
                            EditorGUILayout.BeginHorizontal();
                            Hoster.SkillData.Hit[i].Height = EditorGUILayout.FloatField("Height", Hoster.SkillData.Hit[i].Height);
                            EditorGUILayout.LabelField("m", GUILayout.MaxWidth(30));
                            EditorGUILayout.EndHorizontal();
                        }
                    }

                    EditorGUILayout.Space();

                    Hoster.SkillDataExtra.HitEx[i].Fx = EditorGUILayout.ObjectField("Hit Fx", Hoster.SkillDataExtra.HitEx[i].Fx, typeof(GameObject), true) as GameObject;

                    if (null != Hoster.SkillDataExtra.HitEx[i].Fx && AssetDatabase.GetAssetPath(Hoster.SkillDataExtra.HitEx[i].Fx).Contains("Resources/Effects/"))
                    {
                        string path = AssetDatabase.GetAssetPath(Hoster.SkillDataExtra.HitEx[i].Fx).Remove(0, 17);
                        Hoster.SkillData.Hit[i].Fx = path.Remove(path.LastIndexOf('.'));
                        EditorGUILayout.LabelField("Fx Name", Hoster.SkillData.Hit[i].Fx);
                    }
                    else
                    {
                        Hoster.SkillData.Hit[i].Fx = null;
                        Hoster.SkillDataExtra.HitEx[i].Fx = null;
                        EditorGUILayout.LabelField("Fx Name", Hoster.SkillData.Hit[i].Fx);
                    }

                    Hoster.SkillData.Hit[i].Fx_Follow = EditorGUILayout.Toggle("Follow", Hoster.SkillData.Hit[i].Fx_Follow);
                    Hoster.SkillData.Hit[i].Fx_StickToGround = EditorGUILayout.Toggle("Stick On Ground", Hoster.SkillData.Hit[i].Fx_StickToGround);
                    EditorGUILayout.Space();
                    EditorGUILayout.Space();

                    if (Hoster.SkillData.Hit[i].State == XBeHitState.Hit_Back || Hoster.SkillData.Hit[i].State == XBeHitState.Hit_Roll)
                    {
                        EditorGUILayout.LabelField("Additional settings for hit-back & hit-roll in air condition", _myLabelStyle2);
                        Hoster.SkillData.Hit[i].Additional_Using_Default = EditorGUILayout.Toggle("Taken Default", Hoster.SkillData.Hit[i].Additional_Using_Default);
                        if (!Hoster.SkillData.Hit[i].Additional_Using_Default)
                        {
                            EditorGUILayout.BeginHorizontal();
                            Hoster.SkillData.Hit[i].Additional_Hit_Time_Present_Straight = EditorGUILayout.FloatField("Present Straight", Hoster.SkillData.Hit[i].Additional_Hit_Time_Present_Straight);
                            EditorGUILayout.LabelField("s", GUILayout.MaxWidth(30));
                            EditorGUILayout.EndHorizontal();
                            EditorGUILayout.BeginHorizontal();
                            Hoster.SkillData.Hit[i].Additional_Hit_Time_Hard_Straight = EditorGUILayout.FloatField("Hard Straight", Hoster.SkillData.Hit[i].Additional_Hit_Time_Hard_Straight);
                            EditorGUILayout.LabelField("s", GUILayout.MaxWidth(30));
                            EditorGUILayout.EndHorizontal();
                            EditorGUILayout.Space();
                            EditorGUILayout.BeginHorizontal();
                            Hoster.SkillData.Hit[i].Additional_Hit_Offset = EditorGUILayout.FloatField("Offset", Hoster.SkillData.Hit[i].Additional_Hit_Offset);
                            EditorGUILayout.LabelField("m", GUILayout.MaxWidth(30));
                            EditorGUILayout.EndHorizontal();
                            EditorGUILayout.BeginHorizontal();
                            Hoster.SkillData.Hit[i].Additional_Hit_Height = EditorGUILayout.FloatField("Height", Hoster.SkillData.Hit[i].Additional_Hit_Height);
                            EditorGUILayout.LabelField("m", GUILayout.MaxWidth(30));
                            EditorGUILayout.EndHorizontal();
                        }
                    }
                    EditorGUILayout.Space();
                }
            }

            if (i != Hoster.SkillData.Hit.Count - 1)
            {
                GUILayout.Box("", line);
                EditorGUILayout.Space();
            }
        }
    }

}

