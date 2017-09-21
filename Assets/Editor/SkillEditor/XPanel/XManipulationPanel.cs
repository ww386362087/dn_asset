using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class XManipulationPanel : XPanel
{
    protected override int Count
    {
        get { return Hoster.SkillData.Manipulation == null ? -1 : Hoster.SkillData.Manipulation.Count; }
    }

    protected override bool FoldOut
    {
        get { return Hoster.EditorData.XManipulation_foldout; }
        set { Hoster.EditorData.XManipulation_foldout = value; }
    }

    protected override string PanelName
    {
        get { return "Manipulation"; }
    }

    public override void Add()
    {
        if (Hoster.SkillData.Manipulation == null) Hoster.SkillData.Manipulation = new List<XManipulationData>();
        Hoster.SkillData.Manipulation.Add(new XManipulationData());
        Hoster.SkillDataExtra.Add<XManipulationDataExtra>();
        Hoster.EditorData.XManipulation_foldout = true;
    }
    protected override void OnInnerGUI()
    {
        if (Hoster.SkillData.Manipulation == null) return;

        for (int i = 0; i < Hoster.SkillData.Manipulation.Count; i++)
        {
            Hoster.SkillData.Manipulation[i].Index = i;
            EditorGUILayout.BeginHorizontal();

            Hoster.SkillDataExtra.ManipulationEx[i].Present = EditorGUILayout.Toggle("Present", Hoster.SkillDataExtra.ManipulationEx[i].Present);

            if (GUILayout.Button(_content_remove, GUILayout.MaxWidth(30)))
            {
                Hoster.SkillData.Manipulation.RemoveAt(i);
                Hoster.SkillDataExtra.ManipulationEx.RemoveAt(i);
            }
            EditorGUILayout.EndHorizontal();

            if (i < Hoster.SkillData.Manipulation.Count)
            {
                float play_at = (Hoster.SkillData.Manipulation[i].At / XSkillPanel.frame);
                EditorGUILayout.BeginHorizontal();
                play_at = EditorGUILayout.FloatField("Manipulate At ", play_at);
                GUILayout.Label("(frame)");
                GUILayout.Label("", GUILayout.MaxWidth(30));
                EditorGUILayout.EndHorizontal();

                Hoster.SkillDataExtra.ManipulationEx[i].At_Ratio = play_at / Hoster.SkillDataExtra.SkillClip_Frame;
                if (Hoster.SkillDataExtra.ManipulationEx[i].At_Ratio > 1) Hoster.SkillDataExtra.ManipulationEx[i].At_Ratio = 1;

                EditorGUILayout.BeginHorizontal();
                Hoster.SkillDataExtra.ManipulationEx[i].At_Ratio = EditorGUILayout.Slider("Ratio", Hoster.SkillDataExtra.ManipulationEx[i].At_Ratio, 0, 1);
                GUILayout.Label("(0~1)", EditorStyles.miniLabel);
                EditorGUILayout.EndHorizontal();

                Hoster.SkillData.Manipulation[i].At = (Hoster.SkillDataExtra.ManipulationEx[i].At_Ratio * Hoster.SkillDataExtra.SkillClip_Frame) * XSkillPanel.frame;

                /////////////////////////////////////////////////
                float end_at = (Hoster.SkillData.Manipulation[i].End / XSkillPanel.frame);
                EditorGUILayout.BeginHorizontal();
                end_at = EditorGUILayout.FloatField("Manipulate End", end_at);
                GUILayout.Label("(frame)");
                GUILayout.Label("", GUILayout.MaxWidth(30));
                EditorGUILayout.EndHorizontal();
                if (end_at < play_at) end_at = play_at;
                Hoster.SkillDataExtra.ManipulationEx[i].End_Ratio = end_at / Hoster.SkillDataExtra.SkillClip_Frame;
                if (Hoster.SkillDataExtra.ManipulationEx[i].End_Ratio > 1) Hoster.SkillDataExtra.ManipulationEx[i].End_Ratio = 1;
                EditorGUILayout.BeginHorizontal();
                Hoster.SkillDataExtra.ManipulationEx[i].End_Ratio = EditorGUILayout.Slider("Ratio End", Hoster.SkillDataExtra.ManipulationEx[i].End_Ratio, 0, 1);
                GUILayout.Label("(0~1)", EditorStyles.miniLabel);
                EditorGUILayout.EndHorizontal();
                Hoster.SkillData.Manipulation[i].End = (Hoster.SkillDataExtra.ManipulationEx[i].End_Ratio * Hoster.SkillDataExtra.SkillClip_Frame) * XSkillPanel.frame;

                ////////////////////////////////////////////////
                EditorGUILayout.Space();

                Hoster.SkillData.Manipulation[i].Degree = EditorGUILayout.FloatField("Degree", Hoster.SkillData.Manipulation[i].Degree);
                Hoster.SkillData.Manipulation[i].Radius = EditorGUILayout.FloatField("Radius", Hoster.SkillData.Manipulation[i].Radius);
                Hoster.SkillData.Manipulation[i].Force = EditorGUILayout.FloatField("Force", Hoster.SkillData.Manipulation[i].Force);

                EditorGUILayout.Space();
                Vector3 vec = new Vector3(Hoster.SkillData.Manipulation[i].OffsetX, 0, Hoster.SkillData.Manipulation[i].OffsetZ);
                vec = EditorGUILayout.Vector3Field("Offset", vec);
                Hoster.SkillData.Manipulation[i].OffsetX = vec.x;
                Hoster.SkillData.Manipulation[i].OffsetZ = vec.z;
            }

            if (i != Hoster.SkillData.Manipulation.Count - 1)
            {
                GUILayout.Box("", line);
                EditorGUILayout.Space();
            }
        }
    }

}
