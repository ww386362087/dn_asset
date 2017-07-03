using UnityEngine;
using System.Collections;
using UnityEditor;
namespace XEditor
{

    [CanEditMultipleObjects]
    [CustomEditor(typeof(TextAsset))]
    public class BytesTableEditor : Editor
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
                        if (GUILayout.Button("Table2Bytes", GUILayout.Width(80)))
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
                    }
                    break;
                case EType.EBytes:
                    {
                        GUI.enabled = true;
                        if (GUILayout.Button("Open", GUILayout.Width(80)))
                        {
                            XEditor.BytesTableViewEditor window = (XEditor.BytesTableViewEditor)EditorWindow.GetWindow(typeof(XEditor.BytesTableViewEditor), true, "BytesTableViewEditor");
                            window.Init(target);
                            window.Show();
                        }
                    }
                    break;
                case EType.EOther:
                    GUI.enabled = true;
                    GUILayout.Label((target as TextAsset).text);
                    break;
            }
        }
    }
}