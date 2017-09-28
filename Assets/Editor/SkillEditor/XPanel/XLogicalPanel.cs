using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class XLogicalPanel : XPanel
{
    GUIStyle _myLabelStyle = null;

    // private List<string> _states = new List<string>();

    //private int _defualt_mask_value;
    //private int _states_mask_value;

    protected override int Count
    {
        get { return -1; }
    }

    public override void Init(){ }

    protected override void OnInnerGUI()
    {
        if (_myLabelStyle == null)
        {
            _myLabelStyle = new GUIStyle(GUI.skin.label);
            _myLabelStyle.fontStyle = FontStyle.Italic;
        }

        Hoster.SkillData.Logical.StrickenMask = (XStrickenResponse)EditorGUILayout.EnumPopup("Stricken Type", Hoster.SkillData.Logical.StrickenMask);
        EditorGUILayout.LabelField("Be hit by Ultra skill will default to Cease.", _myLabelStyle);
        EditorGUILayout.Space();

        Hoster.SkillData.Logical.AttackOnHitDown = EditorGUILayout.Toggle("Attack On Hit-Down", Hoster.SkillData.Logical.AttackOnHitDown);
        EditorGUILayout.LabelField("Take effect when enemy is hit down(on ground).", _myLabelStyle);
        EditorGUILayout.Space();

        DataLayout(ref Hoster.SkillData.Logical.Not_Move_At, ref Hoster.ConfigData.Logical.Not_Move_At_Ratio, "Not Move At");
        DataLayout(ref Hoster.SkillData.Logical.Not_Move_End, ref Hoster.ConfigData.Logical.Not_Move_End_Ratio, "Not Move End");
        if (Hoster.SkillData.Logical.Not_Move_End < Hoster.SkillData.Logical.Not_Move_At)
            Hoster.SkillData.Logical.Not_Move_End = Hoster.SkillData.Logical.Not_Move_At;
        EditorGUILayout.Space();

        DataLayout(ref Hoster.SkillData.Logical.Rotate_At, ref Hoster.ConfigData.Logical.Rotate_At_Ratio, "Rotate At");
        DataLayout(ref Hoster.SkillData.Logical.Rotate_End, ref Hoster.ConfigData.Logical.Rotate_End_Ratio, "Rotate End");

        if (Hoster.SkillData.Logical.Rotate_End < Hoster.SkillData.Logical.Rotate_At)
            Hoster.SkillData.Logical.Rotate_End = Hoster.SkillData.Logical.Rotate_At;

        if (Hoster.SkillData.Logical.Rotate_End > 0)
        {
            Hoster.SkillData.Logical.Rotate_Speed = EditorGUILayout.FloatField("Rotate Speed", Hoster.SkillData.Logical.Rotate_Speed);
            EditorGUILayout.LabelField("Rotate speed is 0 means using firer's default r-speed.", _myLabelStyle);
        }
        Hoster.SkillData.Logical.Rotate_Server_Sync = EditorGUILayout.Toggle("Rotate Server Sync", Hoster.SkillData.Logical.Rotate_Server_Sync);
        EditorGUILayout.LabelField("Client direction will sync with server immediately without interpolation.", _myLabelStyle);
        EditorGUILayout.Space();

        int mask = 0;
        for (byte i = 0; i < 32; i++)
        {
            if (((1 << i) & Hoster.SkillData.Logical.CanCastAt_QTE) != 0)
            {
                if (i == 0)
                {
                    mask = 0; break;
                }
                mask |= (1 << XQTEStatusLibrary.GetStatusIdx(i));
            }
        }

        mask = EditorGUILayout.MaskField("At QTE", mask, XQTEStatusLibrary.NameList.ToArray());

        Hoster.SkillData.Logical.CanCastAt_QTE = 0;
        for (byte i = 0; i < 32; i++)
        {
            if (((1 << i) & mask) != 0)
            {
                if (i == 0)
                {
                    Hoster.SkillData.Logical.CanCastAt_QTE = 0; break;
                }
                Hoster.SkillData.Logical.CanCastAt_QTE |= (1 << XQTEStatusLibrary.GetStatusValue(i));
            }
        }
        if (Hoster.SkillData.Logical.CanCastAt_QTE != 0)
        {
            Hoster.SkillData.Logical.QTE_Key = EditorGUILayout.Popup("QTE Key", Hoster.SkillData.Logical.QTE_Key, XQTEStatusLibrary.NameList.ToArray());
        }
        EditorGUILayout.Space();
        EditorGUILayout.BeginHorizontal();
        Hoster.EditorData.XQTEStatus_foldout = EditorGUILayout.Foldout(Hoster.EditorData.XQTEStatus_foldout, "QTE Status");
        if (GUILayout.Button(_content_add, GUILayout.MaxWidth(30)))
        {
            if (Hoster.SkillData.Logical.QTEData == null)
            {
                Hoster.SkillData.Logical.QTEData = new List<XQTEData>();
                Hoster.ConfigData.Logical.QTEDataEx.Clear();
            }

            Hoster.SkillData.Logical.QTEData.Add(new XQTEData());
            Hoster.ConfigData.Logical.QTEDataEx.Add(new XQTEDataExtra());
            if (Hoster.SkillData.Logical.QTEData.Count > 4)
            {
                 XDebug.LogError("Too much QTE(should < 4 Now)");
            }
        }
        EditorGUILayout.EndHorizontal();
        if (Hoster.EditorData.XQTEStatus_foldout && Hoster.SkillData.Logical.QTEData != null)
        {
            for (int i = 0; i < Hoster.SkillData.Logical.QTEData.Count; i++)
            {
                EditorGUILayout.BeginHorizontal();

                Hoster.SkillData.Logical.QTEData[i].QTE = XQTEStatusLibrary.GetStatusValue(EditorGUILayout.Popup("Status", XQTEStatusLibrary.GetStatusIdx(Hoster.SkillData.Logical.QTEData[i].QTE), XQTEStatusLibrary.NameList.ToArray()));

                if (GUILayout.Button(_content_remove, GUILayout.MaxWidth(30)))
                {
                    Hoster.SkillData.Logical.QTEData.RemoveAt(i);
                    Hoster.ConfigData.Logical.QTEDataEx.RemoveAt(i);
                }
                EditorGUILayout.EndHorizontal();

                if (i < Hoster.SkillData.Logical.QTEData.Count)
                {
                    if (Hoster.SkillData.Logical.QTEData[i].QTE > 0)
                    {
                        if (Hoster.ConfigData.Logical.QTEDataEx.Count == i)
                        {
                            XQTEDataExtra qteex = new XQTEDataExtra();
                            Hoster.ConfigData.Logical.QTEDataEx.Add(qteex);
                        }
                        DataLayout(ref Hoster.SkillData.Logical.QTEData[i].At, ref Hoster.ConfigData.Logical.QTEDataEx[i].QTE_At_Ratio, "QTE At");
                        DataLayout(ref Hoster.SkillData.Logical.QTEData[i].End, ref Hoster.ConfigData.Logical.QTEDataEx[i].QTE_End_Ratio, "QTE End");
                        if (Hoster.SkillData.Logical.QTEData[i].End < Hoster.SkillData.Logical.QTEData[i].At)
                            Hoster.SkillData.Logical.QTEData[i].End = Hoster.SkillData.Logical.QTEData[i].At;
                    }
                    EditorGUILayout.LabelField("Indicate skill period in which status.", _myLabelStyle);
                    EditorGUILayout.Space();
                }
            }
        }

        EditorGUILayout.Space();

        Hoster.SkillData.Logical.CanReplacedby = EditorGUILayout.MaskField("Can Replaced By", Hoster.SkillData.Logical.CanReplacedby, XSkillData.Skills);
        DataLayout(ref Hoster.SkillData.Logical.CanCancelAt, ref Hoster.ConfigData.Logical.Cancel_At_Ratio, "Cancel Into Next");

        EditorGUILayout.Space();
        Hoster.SkillData.Logical.SuppressPlayer = EditorGUILayout.Toggle("Suppress Player", Hoster.SkillData.Logical.SuppressPlayer);
        EditorGUILayout.LabelField("Make Game-Player out of control during casting the skill.(usually using for Boss show up!)", _myLabelStyle);
        EditorGUILayout.Space();

        Hoster.SkillData.Logical.Association = EditorGUILayout.Toggle("Association", Hoster.SkillData.Logical.Association);
        if (Hoster.SkillData.Logical.Association)
        {
            Hoster.SkillData.Logical.MoveType = EditorGUILayout.Toggle("Move Version", Hoster.SkillData.Logical.MoveType);

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Association Skill", Hoster.SkillData.Logical.Association_Skill);
            if (Hoster.SkillData.Logical.Association_Skill != null && Hoster.SkillData.Logical.Association_Skill.Length > 0)
            {
                if (GUILayout.Button("Delete", GUILayout.MaxWidth(70)))
                {
                    Hoster.SkillData.Logical.Association_Skill = null;
                }
            }
            if (GUILayout.Button("Browser", GUILayout.MaxWidth(70)))
            {
                string file = EditorUtility.OpenFilePanel("Select Skp file", XEditorLibrary.Skp, "txt");
                if (file.Length > 0)
                {
                    int s = file.LastIndexOf('/');
                    int e = file.LastIndexOf('.');
                    Hoster.SkillData.Logical.Association_Skill = file.Substring(s + 1, e - s - 1);
                }
            }
            EditorGUILayout.EndHorizontal();
        }

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Strength Preservation", _myLabelStyle);
        Hoster.SkillData.Logical.PreservedStrength = EditorGUILayout.IntField("Value", Hoster.SkillData.Logical.PreservedStrength);
        if (Hoster.SkillData.Logical.PreservedStrength > 0)
        {
            DataLayout(ref Hoster.SkillData.Logical.PreservedAt, ref Hoster.ConfigData.Logical.Preserved_Ratio, "Preserved At");
            DataLayout(ref Hoster.SkillData.Logical.PreservedEndAt, ref Hoster.ConfigData.Logical.Preserved_End_Ratio, "Preserved End At");
            if (Hoster.SkillData.Logical.PreservedEndAt < Hoster.SkillData.Logical.PreservedAt)
                Hoster.SkillData.Logical.PreservedEndAt = Hoster.SkillData.Logical.PreservedAt;
        }

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Skill ex-string", _myLabelStyle);
        Hoster.SkillData.Logical.Exstring = EditorGUILayout.TextField("Context", Hoster.SkillData.Logical.Exstring);
        DataLayout(ref Hoster.SkillData.Logical.Exstring_At, ref Hoster.ConfigData.Logical.ExString_Ratio, "ExString At");
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Do not be selected as target", _myLabelStyle);
        DataLayout(ref Hoster.SkillData.Logical.Not_Selected_At, ref Hoster.ConfigData.Logical.Not_Selected_At_Ratio, "Not Selected At");
        DataLayout(ref Hoster.SkillData.Logical.Not_Selected_End, ref Hoster.ConfigData.Logical.Not_Selected_End_Ratio, "Not Selected End");
        if (Hoster.SkillData.Logical.Not_Selected_End < Hoster.SkillData.Logical.Not_Selected_At)
            Hoster.SkillData.Logical.Not_Selected_End = Hoster.SkillData.Logical.Not_Selected_At;
    }

    protected override bool FoldOut
    {
        get { return Hoster.EditorData.XLogical_foldout; }
        set { Hoster.EditorData.XLogical_foldout = value; }
    }

    protected override string PanelName
    {
        get { return "Logic"; }
    }

    private void DataLayout(ref float data, ref float ratio, string name)
    {
        float result_at = (data / XSkillPanel.frame);
        EditorGUILayout.BeginHorizontal();
        result_at = EditorGUILayout.FloatField(name, result_at);
        GUILayout.Label("(frame)");
        GUILayout.Label("", GUILayout.MaxWidth(30));
        EditorGUILayout.EndHorizontal();

        ratio = (Hoster.SkillData.Time / XSkillPanel.frame) > 0 ? result_at / (Hoster.SkillData.Time / XSkillPanel.frame) : 0;
        if (ratio > 1) ratio = 1;

        EditorGUILayout.BeginHorizontal();
        ratio = EditorGUILayout.Slider("Ratio", ratio, 0, 1);
        GUILayout.Label("(0~1)", EditorStyles.miniLabel);
        EditorGUILayout.EndHorizontal();

        data = (ratio * (Hoster.SkillData.Time / XSkillPanel.frame)) * XSkillPanel.frame;
    }

    public override void Add()
    {
        if (Hoster.SkillData.Logical.QTEData == null)
        {
            Hoster.SkillData.Logical.QTEData = new List<XQTEData>();
            Hoster.ConfigData.Logical.QTEDataEx.Clear();
        }

        Hoster.SkillData.Logical.QTEData.Add(new XQTEData());
        Hoster.ConfigData.Logical.QTEDataEx.Add(new XQTEDataExtra());
        if (Hoster.SkillData.Logical.QTEData.Count > 4)
        {
             XDebug.LogError("Too much QTE(should < 4 Now)");
        }
    }
}
