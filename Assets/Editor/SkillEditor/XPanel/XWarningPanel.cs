using UnityEditor;
using UnityEngine;


public class XWarningPanel : XPanel
{
    
    protected override int Count
    {
        get { return Hoster.SkillData.Warning != null ? Hoster.SkillData.Warning.Count : -1; }
    }

    protected override bool FoldOut
    {
        get { return Hoster.EditorData.XWarning_foldout; }
        set { Hoster.EditorData.XWarning_foldout = value; }
    }

    protected override string PanelName
    {
        get { return "Warning"; }
    }

    protected override void OnInnerGUI()
    {
        if (Hoster.SkillData.Warning == null) return;

        for (int i = 0; i < Hoster.SkillData.Warning.Count; i++)
        {
            Hoster.SkillData.Warning[i].Index = i;

            float warning_at = (Hoster.SkillData.Warning[i].At / XSkillPanel.frame);

            EditorGUILayout.BeginHorizontal();
            warning_at = EditorGUILayout.FloatField("Warning At", warning_at);
            GUILayout.Label("(frame)");

            Hoster.SkillDataExtra.Warning[i].Ratio = warning_at / Hoster.SkillDataExtra.SkillClip_Frame;
            if (Hoster.SkillDataExtra.Warning[i].Ratio > 1) Hoster.SkillDataExtra.Warning[i].Ratio = 1;

            if (GUILayout.Button(_content_remove, GUILayout.MaxWidth(30), GUILayout.MinWidth(30)))
            {
                Hoster.SkillData.Warning.RemoveAt(i);
                Hoster.SkillDataExtra.Warning.RemoveAt(i);
                EditorGUILayout.EndHorizontal();
                continue;
            }
            EditorGUILayout.EndHorizontal();

            Hoster.SkillDataExtra.Warning[i].Ratio = EditorGUILayout.Slider("Ratio", Hoster.SkillDataExtra.Warning[i].Ratio, 0, 1);
            Hoster.SkillData.Warning[i].At = (Hoster.SkillDataExtra.Warning[i].Ratio * Hoster.SkillDataExtra.SkillClip_Frame) * XSkillPanel.frame;
            EditorGUILayout.Space();

            Hoster.SkillDataExtra.Warning[i].Fx = EditorGUILayout.ObjectField("Fx Object", Hoster.SkillDataExtra.Warning[i].Fx, typeof(GameObject), true) as GameObject;

            if (null == Hoster.SkillDataExtra.Warning[i].Fx || !AssetDatabase.GetAssetPath(Hoster.SkillDataExtra.Warning[i].Fx).Contains("Resources/Effects/"))
            {
                Hoster.SkillDataExtra.Warning[i].Fx = null;
            }
            if (null != Hoster.SkillDataExtra.Warning[i].Fx)
            {
                string path = AssetDatabase.GetAssetPath(Hoster.SkillDataExtra.Warning[i].Fx).Remove(0, 17);
                Hoster.SkillData.Warning[i].Fx = path.Remove(path.LastIndexOf('.'));
                EditorGUILayout.LabelField("Fx Name", Hoster.SkillData.Warning[i].Fx);

                Hoster.SkillData.Warning[i].Scale = EditorGUILayout.FloatField("Scale", Hoster.SkillData.Warning[i].Scale);
                EditorGUILayout.Space();

                EditorGUILayout.BeginHorizontal();
                Hoster.SkillData.Warning[i].FxDuration = EditorGUILayout.FloatField("Duration", Hoster.SkillData.Warning[i].FxDuration);
                GUILayout.Label("(s)");
                EditorGUILayout.EndHorizontal();

                Hoster.SkillData.Warning[i].Type = (XWarningType)EditorGUILayout.EnumPopup("Type", Hoster.SkillData.Warning[i].Type);
                switch (Hoster.SkillData.Warning[i].Type)
                {
                    case XWarningType.Warning_None:
                        Vector3 vec = new Vector3(Hoster.SkillData.Warning[i].OffsetX, Hoster.SkillData.Warning[i].OffsetY, Hoster.SkillData.Warning[i].OffsetZ);
                        vec = EditorGUILayout.Vector3Field("Offset", vec);
                        Hoster.SkillData.Warning[i].OffsetX = vec.x;
                        Hoster.SkillData.Warning[i].OffsetY = vec.y;
                        Hoster.SkillData.Warning[i].OffsetZ = vec.z;
                        break;
                    case XWarningType.Warning_Target:
                        if (!Hoster.SkillData.NeedTarget)
                        {
                            Hoster.SkillData.Warning[i].Type = XWarningType.Warning_None;
                            EditorUtility.DisplayDialog("Confirm your configuration.",
                                "Before select 'Warning_Target', you must choose 'Need Target' first.",
                                "Ok");
                        }
                        else
                        {
                            Vector3 vec2 = new Vector3(Hoster.SkillData.Warning[i].OffsetX, Hoster.SkillData.Warning[i].OffsetY, Hoster.SkillData.Warning[i].OffsetZ);
                            vec2 = EditorGUILayout.Vector3Field("Offset when no target", vec2);
                            Hoster.SkillData.Warning[i].OffsetX = vec2.x;
                            Hoster.SkillData.Warning[i].OffsetY = vec2.y;
                            Hoster.SkillData.Warning[i].OffsetZ = vec2.z;
                            Hoster.SkillData.Warning[i].RandomWarningPos = EditorGUILayout.Toggle("Is Pos Random", Hoster.SkillData.Warning[i].RandomWarningPos);
                            if (Hoster.SkillData.Warning[i].RandomWarningPos)
                            {
                                Hoster.SkillData.Warning[i].PosRandomRange = EditorGUILayout.FloatField("Random Range", Hoster.SkillData.Warning[i].PosRandomRange);
                                Hoster.SkillData.Warning[i].PosRandomCount = EditorGUILayout.IntField("Random Count", Hoster.SkillData.Warning[i].PosRandomCount);
                            }
                        }
                        break;
                    case XWarningType.Warning_All:
                        Hoster.SkillData.Warning[i].RandomWarningPos = EditorGUILayout.Toggle("Is Pos Random", Hoster.SkillData.Warning[i].RandomWarningPos);
                        if (Hoster.SkillData.Warning[i].RandomWarningPos)
                        {
                            Hoster.SkillData.Warning[i].PosRandomRange = EditorGUILayout.FloatField("Random Range", Hoster.SkillData.Warning[i].PosRandomRange);
                            Hoster.SkillData.Warning[i].PosRandomCount = EditorGUILayout.IntField("Random Count", Hoster.SkillData.Warning[i].PosRandomCount);
                        }
                        else
                        {
                            Hoster.SkillData.Warning[i].Mobs_Inclusived = EditorGUILayout.Toggle("Include Mobs", Hoster.SkillData.Warning[i].Mobs_Inclusived);
                        }
                        break;
                    case XWarningType.Warning_Multiple:
                        Hoster.SkillData.Warning[i].RandomWarningPos = EditorGUILayout.Toggle("Is Pos Random", Hoster.SkillData.Warning[i].RandomWarningPos);
                        if (Hoster.SkillData.Warning[i].RandomWarningPos)
                        {
                            Hoster.SkillData.Warning[i].PosRandomRange = EditorGUILayout.FloatField("Random Range", Hoster.SkillData.Warning[i].PosRandomRange);
                            Hoster.SkillData.Warning[i].PosRandomCount = EditorGUILayout.IntField("Random Count", Hoster.SkillData.Warning[i].PosRandomCount);
                        }
                        Hoster.SkillData.Warning[i].MaxRandomTarget = EditorGUILayout.IntField("Max Target", Hoster.SkillData.Warning[i].MaxRandomTarget);
                        break;
                }
            }

            if (i != Hoster.SkillData.Warning.Count - 1)
            {
                GUILayout.Box("", line);
                EditorGUILayout.Space();
            }
        }
    }
}

