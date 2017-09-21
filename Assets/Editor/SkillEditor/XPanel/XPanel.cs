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

    public virtual void Init() { }

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

    public abstract void Add();


    protected abstract void OnInnerGUI();
    protected virtual void OnInnerUpdate() { }

    protected abstract bool FoldOut { get; set; }
    protected abstract string PanelName { get; }
    protected abstract int Count { get; }
}