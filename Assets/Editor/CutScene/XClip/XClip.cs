using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace XEditor
{
    
    public class XClipComparer : IComparer<XClip>
    {
        public int Compare(XClip x, XClip y)
        {
            if (x == y)
                return 0;
            else if (x.TimeLine != y.TimeLine)
                return (int)(x.TimeLine - y.TimeLine);
            else
                return x.ClipType - y.ClipType;
        }
    }


    public abstract class XClip
    {
        public XClip(float timeline)
        {
            _valid = true;

            CutSceneClip.Type = XClipType.Actor;
            CutSceneClip.TimeLineAt = timeline;
        }

        public XClip(XCutSceneClip data)
        {
            _valid = true;
            CutSceneClip = data;
        }

        private bool _valid;

        private GUIContent _content_minor = new GUIContent("-");
        private GUIContent _content_edit = new GUIContent("e");
        private GUILayoutOption[] _line = new GUILayoutOption[] { GUILayout.ExpandWidth(true), GUILayout.Height(1) };

        private bool _fold_out = false;

        private float _time_line_total_frame = 0;

        public abstract string Name { get; }
        private GUIStyle _myStyle = null;
        private static GUIStyle _myStyle2 = null;

        public bool Valid { get { return _valid; } }

        public XClipType ClipType { get { return CutSceneClip.Type; } }

        public float TimeLineTotal { get { return _time_line_total_frame; } set { _time_line_total_frame = value; } }

        public float TimeLine
        {
            get { return CutSceneClip.TimeLineAt; }
            set
            {
                CutSceneClip.TimeLineAt = value;
            }
        }

        public string Title
        {
            get
            {
                string frame = CutSceneClip.TimeLineAt.ToString("F1") + "fm/";
                string seconds = (CutSceneClip.TimeLineAt * (1 / XEditorLibrary.FPS)).ToString("F2") + "s) ";
                return "(Timeline " + frame + seconds + CutSceneClip.Type.ToString() + ": " + Name;
            }
        }

        protected GUIStyle _textStyle = null;
        public abstract void OnTextColor();

        public void OnGUI(XCutSceneData data)
        {
            if (CutSceneClip.TimeLineAt > _time_line_total_frame) return;
            if (_myStyle == null)
            {
                _myStyle = new GUIStyle(GUI.skin.label);
                _myStyle.margin.left = 15;
            }
            if (_textStyle == null)
            {
                _textStyle = new GUIStyle(EditorStyles.foldout);
                OnTextColor();
            }
            EditorGUILayout.BeginHorizontal();
            _fold_out = EditorGUILayout.Foldout(_fold_out, Title, _textStyle);
            GUILayout.FlexibleSpace();
            if (GUILayout.Button(_content_minor, GUILayout.MaxWidth(25), GUILayout.MaxHeight(15)))
            {
                _valid = false;
            }
            if (GUILayout.Button(_content_edit, GUILayout.MaxWidth(25), GUILayout.MaxHeight(15)))
            {
                XCutSceneTimelineWindow window = EditorWindow.GetWindow<XCutSceneTimelineWindow>(@"Timeline At:");
                window._play_at_frame = CutSceneClip.TimeLineAt;
                window._total_frame = _time_line_total_frame;
                window._clip = this;
            }
            EditorGUILayout.EndHorizontal();
            if (_fold_out)
            {
                EditorGUILayout.BeginVertical(_myStyle);
                GUILayout.Box("", _line);
                OnInnerGUI(data);
                EditorGUILayout.EndVertical();
            }
        }

        public abstract XCutSceneClip CutSceneClip { get; set; }
        public abstract void Flush();
        public abstract void Dump();
        protected abstract void OnInnerGUI(XCutSceneData data);

        public static string LocateRes(Object o)
        {
            if (o == null) return null;
            string path = AssetDatabase.GetAssetPath(o);
            if (path.StartsWith("Assets/Animation/"))
            {
                path = path.Remove(0, 7);
            }
            else
            {
                path = path.Remove(0, 17);
            }
            return path.Remove(path.LastIndexOf('.'));
        }

        public static string LocateBone(GameObject o)
        {
            if (o == null) return null;
            string name = "";
            Transform parent = o.transform;
            while (parent.parent != null)
            {
                name = name.Length == 0 ? parent.name : parent.name + "/" + name;
                parent = parent.parent;
            }
            return name.Length > 0 ? name : null;
        }

        public static Vector3 Vector3FieldEx(string label, Vector3 value, ref bool fold, bool labeloff = false, bool yoff = true, params GUILayoutOption[] options)
        {
            if (_myStyle2 == null)
            {
                _myStyle2 = new GUIStyle(GUI.skin.label);
                _myStyle2.margin.left = 13;
            }
            fold = EditorGUILayout.Foldout(fold, label);
            if (fold)
            {
                EditorGUILayout.BeginVertical(_myStyle2);
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("X", GUILayout.Width(15));
                value.x = EditorGUILayout.FloatField(value.x, GUILayout.MaxWidth(250));
                if (!labeloff) EditorGUILayout.LabelField("(m)", GUILayout.Width(25));
                EditorGUILayout.EndHorizontal();

                if (yoff) value.y = 0;
                else
                {
                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField("Y", GUILayout.Width(15));
                    value.y = EditorGUILayout.FloatField(value.y, GUILayout.MaxWidth(250));
                    if (!labeloff) EditorGUILayout.LabelField("(m)", GUILayout.Width(25));
                    EditorGUILayout.EndHorizontal();
                }
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Z", GUILayout.Width(15));
                value.z = EditorGUILayout.FloatField(value.z, GUILayout.MaxWidth(250));
                if (!labeloff) EditorGUILayout.LabelField("(m)", GUILayout.Width(25));
                EditorGUILayout.EndHorizontal();
                EditorGUILayout.EndVertical();
            }
            return value;
        }
    }


}