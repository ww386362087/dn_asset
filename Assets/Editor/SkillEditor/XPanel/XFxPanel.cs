using System.Collections.Generic;
using UnityEditor;
using UnityEngine;


public class XFxPanel : XPanel
{
    
    protected override int Count
    {
        get { return Hoster.SkillData.Fx == null ? -1 : Hoster.SkillData.Fx.Count; }
    }

    protected override bool FoldOut
    {
        get { return Hoster.EditorData.XFx_foldout; }
        set { Hoster.EditorData.XFx_foldout = value; }
    }

    protected override string PanelName
    {
        get { return "Fx"; }
    }

    public override void Add()
    {
        if (Hoster.SkillData.Fx == null) Hoster.SkillData.Fx = new List<XFxData>();
        Hoster.SkillData.Fx.Add(new XFxData());
        Hoster.SkillDataExtra.Add<XFxDataExtra>();
        Hoster.EditorData.XFx_foldout = true;
    }

    protected override void OnInnerGUI()
    {
        if (Hoster.SkillData.Fx == null) return;

        for (int i = 0; i < Hoster.SkillData.Fx.Count; i++)
        {
            Hoster.SkillData.Fx[i].Combined = (Hoster.SkillData.TypeToken == 3);

            Hoster.SkillData.Fx[i].Index = i;
            EditorGUILayout.BeginHorizontal();
            Hoster.SkillData.Fx[i].Type = (SkillFxType)EditorGUILayout.EnumPopup("Type Based on", Hoster.SkillData.Fx[i].Type);
            if (GUILayout.Button(_content_remove, GUILayout.MaxWidth(30)))
            {
                Hoster.SkillData.Fx.RemoveAt(i);
                Hoster.SkillDataExtra.Fx.RemoveAt(i);
                EditorGUILayout.EndHorizontal();
                continue;
            }
            EditorGUILayout.EndHorizontal();

            Hoster.SkillDataExtra.Fx[i].Fx = EditorGUILayout.ObjectField("Fx Object", Hoster.SkillDataExtra.Fx[i].Fx, typeof(GameObject), true) as GameObject;
            if (null == Hoster.SkillDataExtra.Fx[i].Fx || !AssetDatabase.GetAssetPath(Hoster.SkillDataExtra.Fx[i].Fx).Contains("Resources/Effects/"))
            {
                Hoster.SkillDataExtra.Fx[i].Fx = null;
            }
            if (null != Hoster.SkillDataExtra.Fx[i].Fx)
            {
                string path = AssetDatabase.GetAssetPath(Hoster.SkillDataExtra.Fx[i].Fx).Remove(0, 17);
                Hoster.SkillData.Fx[i].Fx = path.Remove(path.LastIndexOf('.'));
                EditorGUILayout.LabelField("Fx Name", Hoster.SkillData.Fx[i].Fx);

                if (Hoster.SkillData.Fx[i].Type == SkillFxType.FirerBased)
                {
                    if (!Hoster.SkillData.Fx[i].StickToGround)
                    {
                        if (Hoster.SkillData.Fx[i].Bone == null || Hoster.SkillData.Fx[i].Bone.Length == 0)
                            Hoster.SkillDataExtra.Fx[i].BindTo = null;
                        Hoster.SkillDataExtra.Fx[i].BindTo = EditorGUILayout.ObjectField("Bone", Hoster.SkillDataExtra.Fx[i].BindTo, typeof(GameObject), true) as GameObject;
                        if (Hoster.SkillDataExtra.Fx[i].BindTo != null)
                        {
                            string name = "";
                            Transform parent = Hoster.SkillDataExtra.Fx[i].BindTo.transform;
                            while (parent.parent != null)
                            {
                                name = name.Length == 0 ? parent.name : parent.name + "/" + name;
                                parent = parent.parent;
                            }
                            Hoster.SkillData.Fx[i].Bone = name.Length > 0 ? name : null;
                        }
                        else
                            Hoster.SkillData.Fx[i].Bone = null;
                    }
                }

                EditorGUILayout.Space();
                Vector3 vec = new Vector3(Hoster.SkillData.Fx[i].ScaleX, Hoster.SkillData.Fx[i].ScaleY, Hoster.SkillData.Fx[i].ScaleZ);
                vec = EditorGUILayout.Vector3Field("Scale", vec);
                Hoster.SkillData.Fx[i].ScaleX = vec.x;
                Hoster.SkillData.Fx[i].ScaleY = vec.y;
                Hoster.SkillData.Fx[i].ScaleZ = vec.z;

                if (Hoster.SkillData.Fx[i].Type == SkillFxType.TargetBased)
                {
                    vec.Set(Hoster.SkillData.Fx[i].Target_OffsetX, Hoster.SkillData.Fx[i].Target_OffsetY, Hoster.SkillData.Fx[i].Target_OffsetZ);
                    vec = EditorGUILayout.Vector3Field("Offset Target", vec);
                    Hoster.SkillData.Fx[i].Target_OffsetX = vec.x;
                    Hoster.SkillData.Fx[i].Target_OffsetY = vec.y;
                    Hoster.SkillData.Fx[i].Target_OffsetZ = vec.z;

                    vec.Set(Hoster.SkillData.Fx[i].OffsetX, Hoster.SkillData.Fx[i].OffsetY, Hoster.SkillData.Fx[i].OffsetZ);
                    vec = EditorGUILayout.Vector3Field("Offset Firer when no Target", vec);
                    Hoster.SkillData.Fx[i].OffsetX = vec.x;
                    Hoster.SkillData.Fx[i].OffsetY = vec.y;
                    Hoster.SkillData.Fx[i].OffsetZ = vec.z;
                }
                else
                {
                    vec.Set(Hoster.SkillData.Fx[i].OffsetX, Hoster.SkillData.Fx[i].OffsetY, Hoster.SkillData.Fx[i].OffsetZ);
                    vec = EditorGUILayout.Vector3Field("Offset", vec);
                    Hoster.SkillData.Fx[i].OffsetX = vec.x;
                    Hoster.SkillData.Fx[i].OffsetY = vec.y;
                    Hoster.SkillData.Fx[i].OffsetZ = vec.z;
                }

                EditorGUILayout.Space();
                float fx_at = (Hoster.SkillData.Fx[i].At / XSkillPanel.frame);
                EditorGUILayout.BeginHorizontal();
                fx_at = EditorGUILayout.FloatField("Play At", fx_at);
                GUILayout.Label("(frame)");
                GUILayout.Label("", GUILayout.MaxWidth(30));
                EditorGUILayout.EndHorizontal();

                Hoster.SkillDataExtra.Fx[i].Ratio = fx_at / Hoster.SkillDataExtra.SkillClip_Frame;
                if (Hoster.SkillDataExtra.Fx[i].Ratio > 1) Hoster.SkillDataExtra.Fx[i].Ratio = 1;

                EditorGUILayout.BeginHorizontal();
                Hoster.SkillDataExtra.Fx[i].Ratio = EditorGUILayout.Slider("Ratio", Hoster.SkillDataExtra.Fx[i].Ratio, 0, 1);
                GUILayout.Label("(0~1)", EditorStyles.miniLabel);
                EditorGUILayout.EndHorizontal();

                Hoster.SkillData.Fx[i].At = (Hoster.SkillDataExtra.Fx[i].Ratio * Hoster.SkillDataExtra.SkillClip_Frame) * XSkillPanel.frame;

                ////////////////////////////////
                if (Hoster.SkillData.Fx[i].End < 0) Hoster.SkillData.Fx[i].End = Hoster.SkillDataExtra.SkillClip_Frame * XSkillPanel.frame;
                float fx_end_at = (Hoster.SkillData.Fx[i].End / XSkillPanel.frame);
                EditorGUILayout.BeginHorizontal();
                fx_end_at = EditorGUILayout.FloatField("End At", fx_end_at);
                GUILayout.Label("(frame)");
                GUILayout.Label("", GUILayout.MaxWidth(30));
                EditorGUILayout.EndHorizontal();

                Hoster.SkillDataExtra.Fx[i].End_Ratio = fx_end_at / Hoster.SkillDataExtra.SkillClip_Frame;
                if (Hoster.SkillDataExtra.Fx[i].End_Ratio > 1) Hoster.SkillDataExtra.Fx[i].End_Ratio = 1;

                EditorGUILayout.BeginHorizontal();
                Hoster.SkillDataExtra.Fx[i].End_Ratio = EditorGUILayout.Slider("End Ratio", Hoster.SkillDataExtra.Fx[i].End_Ratio, 0, 1);
                GUILayout.Label("(0~1)", EditorStyles.miniLabel);
                EditorGUILayout.EndHorizontal();

                Hoster.SkillData.Fx[i].End = (Hoster.SkillDataExtra.Fx[i].End_Ratio * Hoster.SkillDataExtra.SkillClip_Frame) * XSkillPanel.frame;
                if (Hoster.SkillData.Fx[i].End < Hoster.SkillData.Fx[i].At) Hoster.SkillData.Fx[i].At = Hoster.SkillData.Fx[i].End;
                EditorGUILayout.Space();

                Hoster.SkillData.Fx[i].StickToGround = EditorGUILayout.Toggle("Stick On Ground", Hoster.SkillData.Fx[i].StickToGround);
                /*
                 * follow mode can not use for TargetBased
                 * in that case the fx life-time will not in controlled.
                 */
                if (Hoster.SkillData.Fx[i].Type == SkillFxType.FirerBased)
                    Hoster.SkillData.Fx[i].Follow = EditorGUILayout.Toggle("Follow", Hoster.SkillData.Fx[i].Follow);
                else
                    Hoster.SkillData.Fx[i].Follow = false;
                EditorGUILayout.Space();

                EditorGUILayout.BeginHorizontal();
                Hoster.SkillData.Fx[i].Destroy_Delay = EditorGUILayout.FloatField("Delay Destroy", Hoster.SkillData.Fx[i].Destroy_Delay);
                GUILayout.Label("(s)");
                EditorGUILayout.EndHorizontal();
                Hoster.SkillData.Fx[i].Shield = EditorGUILayout.Toggle("Shield", Hoster.SkillData.Fx[i].Shield);
            }
            else
            {
                Hoster.SkillData.Fx[i].Fx = null;
            }

            if (i != Hoster.SkillData.Fx.Count - 1)
            {
                GUILayout.Box("", line);
                EditorGUILayout.Space();
            }
        }
    }
}
