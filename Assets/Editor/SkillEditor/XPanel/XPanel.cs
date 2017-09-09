using System;
using UnityEditor;
using System.Collections.Generic;
using UnityEngine;

public abstract class XPanel
{
    protected GUIContent _content_add = new GUIContent("+");
    protected GUIContent _content_remove = new GUIContent("-", "Remove Item.");

    private GUIStyle _style = null;

   private GUILayoutOption[] _line;

    protected GUILayoutOption[] line
    {
        get
        {
            if (_line == null) _line = new GUILayoutOption[] { GUILayout.Width(EditorGUIUtility.labelWidth), GUILayout.Height(1) };
            return _line;
        }
    }

    public XSkillHoster Hoster { get; set; }

    public virtual void Init()
    {
    }

    public void OnGUI()
    {
        if (_style == null) _style = new GUIStyle(GUI.skin.GetStyle("Label"));
        _style.alignment = TextAnchor.UpperRight;

        EditorGUILayout.BeginHorizontal();
        FoldOut = EditorGUILayout.Foldout(FoldOut, PanelName);
        GUILayout.FlexibleSpace();
        if (Count > 0) EditorGUILayout.LabelField("Total " + Count.ToString(), _style);
        EditorGUILayout.EndHorizontal();

        if (FoldOut)
        {
            GUILayout.Box("", GUILayout.ExpandWidth(true), GUILayout.Height(1));
            OnInnerGUI();
        }
        else
        {
            OnInnerUpdate();
        }
    }

    public void Add<T>()
        where T : XBaseData, new()
    {
        T data = new T();
        Type t = typeof(T);

        if (t == typeof(XResultData)) { if (Hoster.SkillData.Result == null) Hoster.SkillData.Result = new List<XResultData>(); Hoster.SkillData.Result.Add(data as XResultData); }
        else if (t == typeof(XJAData)) { if (Hoster.SkillData.Ja == null) Hoster.SkillData.Ja = new List<XJAData>(); Hoster.SkillData.Ja.Add(data as XJAData); }
        else if (t == typeof(XHitData)) { if (Hoster.SkillData.Hit == null) Hoster.SkillData.Hit = new List<XHitData>(); Hoster.SkillData.Hit.Add(data as XHitData); }
        else if (t == typeof(XFxData)) { if (Hoster.SkillData.Fx == null) Hoster.SkillData.Fx = new List<XFxData>(); Hoster.SkillData.Fx.Add(data as XFxData); }
       else if (t == typeof(XWarningData)) { if (Hoster.SkillData.Warning == null) Hoster.SkillData.Warning = new List<XWarningData>(); Hoster.SkillData.Warning.Add(data as XWarningData); }
        else if (t == typeof(XMobUnitData)) { if (Hoster.SkillData.Mob == null) Hoster.SkillData.Mob = new List<XMobUnitData>(); Hoster.SkillData.Mob.Add(data as XMobUnitData); }
       
        Hoster.EditorData.ToggleFold<T>(true);

        if (t == typeof(XResultData))
        {
            AddExtra<XResultDataExtra>();
            AddExtraEx<XResultDataExtraEx>();
        }
        else if (t == typeof(XJAData))
        {
            AddExtra<XJADataExtra>();
            AddExtraEx<XJADataExtraEx>();
        }
        else if (t == typeof(XFxData)) AddExtraEx<XFxDataExtra>();
        else if (t == typeof(XWarningData)) AddExtraEx<XWarningDataExtra>();
        else if (t == typeof(XMobUnitData)) AddExtraEx<XMobUnitDataExtra>();
        else if (t == typeof(XHitData))AddExtraEx<XHitDataExtraEx>();
    }

    private void AddExtra<T>()
        where T : XBaseDataExtra, new()
    {
        T data = new T();
        Hoster.ConfigData.Add<T>(data);
    }

    private void AddExtraEx<T>()
        where T : XBaseDataExtra, new()
    {
        T data = new T();
        Hoster.SkillDataExtra.Add<T>(data);
    }

    protected abstract void OnInnerGUI();
    protected virtual void OnInnerUpdate() { }

    protected abstract bool FoldOut { get; set; }
    protected abstract string PanelName { get; }
    protected abstract int Count { get; }
}