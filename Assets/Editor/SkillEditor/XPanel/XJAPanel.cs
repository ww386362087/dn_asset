using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using XEditor;

public class XJAPanel : XPanel
{
    protected override int Count
    {
        get { return Hoster.SkillData.Ja != null ? Hoster.SkillData.Ja.Count : -1; }
    }


    public override void Add()
    {
        if (Hoster.SkillData.Ja == null) Hoster.SkillData.Ja = new List<XJAData>();
        Hoster.SkillData.Ja.Add(new XJAData());
        Hoster.ConfigData.Add<XJADataExtra>();
        Hoster.SkillDataExtra.Add<XJADataExtraEx>();
        Hoster.EditorData.XJA_foldout = true;
    }

    protected override void OnInnerGUI()
    {
        if (Hoster.SkillData.Ja == null) return;

        for (int i = 0; i < Hoster.SkillData.Ja.Count; i++)
        {
            Hoster.SkillData.Ja[i].Index = i;

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Next Skill", Hoster.SkillData.Ja[i].Next_Name);
            if (Hoster.SkillData.Ja[i].Next_Name != null && Hoster.SkillData.Ja[i].Next_Name.Length > 0)
            {
                if (GUILayout.Button("Delete", GUILayout.MaxWidth(70)))
                {
                    Hoster.SkillData.Ja[i].Next_Name = null;
                }
            }
            if (GUILayout.Button("Browser", GUILayout.MaxWidth(70)))
            {
                string file = EditorUtility.OpenFilePanel("Select Skp file", XEditorLibrary.Skp, "txt");
                if (file.Length > 0)
                {
                    int s = file.LastIndexOf('/');
                    int e = file.LastIndexOf('.');
                    Hoster.SkillData.Ja[i].Next_Name = file.Substring(s + 1, e - s - 1);

                    XSkillData skill = XDataIO<XSkillData>.singleton.DeserializeData(file);
                    Hoster.SkillDataExtra.JaEx[i].Next = skill;
                    Hoster.ConfigData.Ja[i].Next_Skill_PathWithName = file.Substring(file.IndexOf("SkillPackage/"));
                }
            }
            if (Hoster.SkillData.Ja[i].Next_Name == null)
            {
                Hoster.SkillDataExtra.JaEx[i].Next = null;
                Hoster.ConfigData.Ja[i].Next_Skill_PathWithName = null;
            }
            if (GUILayout.Button(_content_remove, GUILayout.MaxWidth(30)))
            {
                Hoster.SkillData.Ja.RemoveAt(i);
                Hoster.ConfigData.Ja.RemoveAt(i);
                Hoster.SkillDataExtra.JaEx.RemoveAt(i);
                EditorGUILayout.EndHorizontal();
                continue;
            }
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("JA Skill", Hoster.SkillData.Ja[i].Name);
            if (Hoster.SkillData.Ja[i].Name != null && Hoster.SkillData.Ja[i].Name.Length > 0)
            {
                if (GUILayout.Button("Delete", GUILayout.MaxWidth(70)))
                {
                    Hoster.SkillData.Ja[i].Name = null;
                }
            }
            if (GUILayout.Button("Browser", GUILayout.MaxWidth(70)))
            {
                string file = EditorUtility.OpenFilePanel("Select Skp file", XEditorLibrary.Skp, "txt");
                if (file.Length > 0)
                {
                    int s = file.LastIndexOf('/');
                    int e = file.LastIndexOf('.');
                    Hoster.SkillData.Ja[i].Name = file.Substring(s + 1, e - s - 1);

                    XSkillData skill = XDataIO<XSkillData>.singleton.DeserializeData(file);
                    Hoster.SkillDataExtra.JaEx[i].Ja = skill;
                    Hoster.ConfigData.Ja[i].JA_Skill_PathWithName = file.Substring(file.IndexOf("SkillPackage/"));
                }
            }
            if (Hoster.SkillData.Ja[i].Name == null)
            {
                Hoster.SkillDataExtra.JaEx[i].Ja = null;
                Hoster.ConfigData.Ja[i].JA_Skill_PathWithName = null;
            }
            EditorGUILayout.LabelField("", GUILayout.MaxWidth(30));
            EditorGUILayout.EndHorizontal();

            float ja_at = (Hoster.SkillData.Ja[i].At / XSkillInspector.frame);
            EditorGUILayout.BeginHorizontal();
            ja_at = EditorGUILayout.FloatField("Begin At", ja_at);
            GUILayout.Label("(frame)");
            GUILayout.Label("", GUILayout.MaxWidth(30));
            EditorGUILayout.EndHorizontal();
            Hoster.ConfigData.Ja[i].JA_Begin_Ratio = ja_at / Hoster.SkillDataExtra.SkillClip_Frame;
            if (Hoster.ConfigData.Ja[i].JA_Begin_Ratio > 1) Hoster.ConfigData.Ja[i].JA_Begin_Ratio = 1;
            EditorGUILayout.BeginHorizontal();
            Hoster.ConfigData.Ja[i].JA_Begin_Ratio = EditorGUILayout.Slider("Begin Ratio", Hoster.ConfigData.Ja[i].JA_Begin_Ratio, 0, 1);
            GUILayout.Label("(0~1)", EditorStyles.miniLabel);
            EditorGUILayout.EndHorizontal();
            Hoster.SkillData.Ja[i].At = (Hoster.ConfigData.Ja[i].JA_Begin_Ratio * Hoster.SkillDataExtra.SkillClip_Frame) * XSkillInspector.frame;

            float ja_end = (Hoster.SkillData.Ja[i].End / XSkillInspector.frame);
            EditorGUILayout.BeginHorizontal();
            ja_end = EditorGUILayout.FloatField("End At", ja_end);
            GUILayout.Label("(frame)");
            GUILayout.Label("", GUILayout.MaxWidth(30));
            EditorGUILayout.EndHorizontal();
            if (ja_end < ja_at) ja_end = ja_at;
            Hoster.ConfigData.Ja[i].JA_End_Ratio = ja_end / Hoster.SkillDataExtra.SkillClip_Frame;
            if (Hoster.ConfigData.Ja[i].JA_End_Ratio > 1) Hoster.ConfigData.Ja[i].JA_End_Ratio = 1;
            EditorGUILayout.BeginHorizontal();
            Hoster.ConfigData.Ja[i].JA_End_Ratio = EditorGUILayout.Slider("End Ratio", Hoster.ConfigData.Ja[i].JA_End_Ratio, 0, 1);
            GUILayout.Label("(0~1)", EditorStyles.miniLabel);
            EditorGUILayout.EndHorizontal();
            Hoster.SkillData.Ja[i].End = (Hoster.ConfigData.Ja[i].JA_End_Ratio * Hoster.SkillDataExtra.SkillClip_Frame) * XSkillInspector.frame;

            float ja_point = (Hoster.SkillData.Ja[i].Point / XSkillInspector.frame);
            EditorGUILayout.BeginHorizontal();
            ja_point = EditorGUILayout.FloatField("Point At", ja_point);
            GUILayout.Label("(frame)");
            GUILayout.Label("", GUILayout.MaxWidth(30));
            EditorGUILayout.EndHorizontal();
            Hoster.ConfigData.Ja[i].JA_Point_Ratio = ja_point / Hoster.SkillDataExtra.SkillClip_Frame;
            if (Hoster.ConfigData.Ja[i].JA_Point_Ratio > 1) Hoster.ConfigData.Ja[i].JA_Point_Ratio = 1;
            EditorGUILayout.BeginHorizontal();
            Hoster.ConfigData.Ja[i].JA_Point_Ratio = EditorGUILayout.Slider("Point Ratio", Hoster.ConfigData.Ja[i].JA_Point_Ratio, 0, 1);
            GUILayout.Label("(0~1)", EditorStyles.miniLabel);
            EditorGUILayout.EndHorizontal();
            Hoster.SkillData.Ja[i].Point = (Hoster.ConfigData.Ja[i].JA_Point_Ratio * Hoster.SkillDataExtra.SkillClip_Frame) * XSkillInspector.frame;

            if (i != Hoster.SkillData.Ja.Count - 1)
            {
                GUILayout.Box("", line);
                EditorGUILayout.Space();
            }
        }
    }

    protected override bool FoldOut
    {
        get { return Hoster.EditorData.XJA_foldout; }
        set { Hoster.EditorData.XJA_foldout = value; }
    }

    protected override string PanelName
    {
        get { return "Just Attack"; }
    }
}
