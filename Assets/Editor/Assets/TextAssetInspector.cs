using UnityEngine;
using UnityEditor;
using System.Reflection;
using System;

namespace XEditor
{

    [CanEditMultipleObjects]
    [CustomEditor(typeof(TextAsset))]
    public class TextInspector : Editor
    {
        enum EType
        {
            ECSV,
            EBytes,
            EOther
        }


        private EType tType = EType.EOther;
        public void OnEnable()
        {
            string path = AssetDatabase.GetAssetPath(target);
            if (path.StartsWith("Assets/Resources/Table/") && path.EndsWith(".bytes"))
            {
                tType = EType.EBytes;
            }
            else if (path.StartsWith("Assets/Table/") && path.EndsWith(".csv"))
            {
                tType = EType.ECSV;
            }
        }

        public override void OnInspectorGUI()
        {
            switch (tType)
            {
                case EType.ECSV:
                    {
                        GUI.enabled = true;
                        GUILayout.Space(12);
                        if (GUILayout.Button("生成Bytes", GUILayout.Width(90)))
                        {
                            if (targets == null || targets.Length == 1)
                            {
                                TableEditor.Table2Bytes(target);
                            }
                            else
                            {
                                TableEditor.Table2Bytes(targets);
                            }
                        }
                        GUILayout.Space(12);
                        if (GUILayout.Button("生成代码", GUILayout.Width(90)))
                        {
                            if (targets == null || targets.Length == 1)
                            {
                                TableEditor.Table2Codes(target);
                            }
                            else
                            {
                                TableEditor.Table2Codes(targets);
                            }
                        }
                        GUILayout.Space(12);
                        if (GUILayout.Button("Table2All", GUILayout.Width(90)))
                        {
                            if (targets == null || targets.Length == 1)
                            {
                                TableEditor.Table2Bytes(target);
                                TableEditor.Table2Codes(target);
                            }
                            else
                            {
                                TableEditor.Table2Codes(targets);
                                TableEditor.Table2Bytes(targets);
                            }
                        }
                        GUILayout.Space(12);
                        Assembly ass = Assembly.Load("Assembly-CSharp");
                        Type type = ass.GetType("XTable." + target.name);
                        string str = "字段列表:\n";
                        if (type != null)
                        {
                            Type tableType = type.GetNestedType("RowData");
                            FieldInfo[] fields = tableType.GetFields();
                            GUILayout.Label("Preview:\n  字段数：" + fields.Length + "\n  字节数：" + ((target as TextAsset).bytes.Length));

                            for (int i = 0; i < fields.Length; i++)
                            {
                                str += "   " + fields[i].Name + "\n";
                            }
                        }
                        GUILayout.Space(4);
                        GUILayout.Label(str);
                    }
                    break;
                case EType.EBytes:
                    {
                        GUI.enabled = true;
                        if (GUILayout.Button("Open", GUILayout.Width(80)))
                        {
                            BytesTableViewEditor window = (BytesTableViewEditor)EditorWindow.GetWindow(typeof(XEditor.BytesTableViewEditor), true, "BytesTableViewEditor");
                            window.Init(target);
                            window.Show();
                        }
                    }
                    break;
                case EType.EOther:
                    GUI.enabled = true;
                    string txt = (target as TextAsset).text;
                    if (txt.Length > 1 << 12) txt = txt.Substring(0, 1 << 12) + "\n......";
                    GUILayout.Label(txt);
                    break;
            }
        }
    }

}