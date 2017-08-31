using UnityEditor;
using UnityEngine;

namespace XEditor
{

    /// <summary>
    /// Timeline 编辑器
    /// </summary>
    public class XCutSceneTimelineWindow : EditorWindow
    {
        public XClip _clip = null;
        public float _total_frame = 0;
        public float _play_at_frame = 0;
        private bool _ok = false;

        void OnGUI()
        {
            EditorGUILayout.Space();
            EditorGUILayout.BeginHorizontal();
            _play_at_frame = EditorGUILayout.FloatField("Play at Frame", _play_at_frame);
            GUILayout.Label("(frame)");
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Time At", (_play_at_frame * (1 / XEditorLibrary.FPS)).ToString("F2"));
            GUILayout.Label("(s)");
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.Space();
            _play_at_frame = EditorGUILayout.Slider("Ratio", _play_at_frame, 0, _total_frame);

            EditorGUILayout.Space();
            EditorGUILayout.Space();
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("OK"))
            {
                _ok = true;
                Close();
            }

            if (GUILayout.Button("Cancel"))
            {
                Close();
            }
            EditorGUILayout.EndHorizontal();
        }

        void OnDestroy()
        {
            if (_ok)
            {
                CutSceneWindow window = GetWindow<CutSceneWindow>(@"Cut Scene");

                if (_clip != null)
                {
                    window.RemoveClip(_clip);
                    _clip.TimeLine = _play_at_frame;
                    window.AddClip(_clip);
                }
                else
                {
                    window.AddClip(_play_at_frame);
                }

                window.Focus();
            }

            _clip = null;
        }
    }
}