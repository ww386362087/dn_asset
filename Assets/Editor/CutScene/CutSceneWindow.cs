using System;
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;

namespace XEditor
{

    public class CutSceneWindow : EditorWindow
    {
        public enum EntitySpecies
        {
            Species_Boss = 1,
            Species_Opposer = 2,
            Species_Puppet = 3,
            Species_Ally = 4,
            Species_Npc = 7,
            Species_Role = 10,
            Species_Empty = 8,
            Species_Dummy = 9,
            Species_Neutral = 5,
            Species_Affiliate = 11,
            Species_Elite = 6
        }

        private bool _open_scene = false;
        private string _file = null;

        private SortedList<XClip, XClip> _clips = new SortedList<XClip, XClip>(new XClipComparer());

        private XClipType _type = XClipType.Actor;
        private GUIContent _content_add = new GUIContent("+");

        private AnimationClip _camera = null;
        private string _name = null;
        private string _script = null;
        private string _scene = null;
        private int _type_mask = -1;
        private bool _mourningborder = true;
        private bool _auto_end = true;
        private bool _general_show = true;
        private bool _general_bigguy = false;
        private bool _override_bgm = true;

        private float _fov = 45;
        private float _length = 0;
        private CameraTrigger _trigger = CameraTrigger.ToEffect;

        private XCutSceneData _run_data = null;
        private GUIStyle _labelstyle = null;
        Vector2 scrollPosition = Vector2.zero;

        public static List<string> ActorList = new List<string>();

        CutSceneWindow()
        {
            EditorApplication.playmodeStateChanged += OnQuit;
        }

        void OnEnable()
        {
            if (_run_data != null)
            {
                InnerLoad(_run_data);
            }
        }

        public void AddClip(float timeline)
        {
            switch (_type)
            {
                case XClipType.Actor:
                    AddClip(new XActorClip(timeline));
                    break;
                case XClipType.Player:
                    AddClip(new XPlayerClip(timeline));
                    break;
                case XClipType.Fx:
                    AddClip(new XFxClip(timeline));
                    break;
                case XClipType.Audio:
                    AddClip(new XAudioClip(timeline));
                    break;
                case XClipType.SubTitle:
                    AddClip(new XSubTitleClip(timeline));
                    break;
                case XClipType.Slash:
                    AddClip(new XSlashClip(timeline));
                    break;
            }
        }

        public void AddClip(XClip clip)
        {
            _clips.Add(clip, clip);
        }

        public XClip AddClip(XCutSceneClip clip)
        {
            XClip xclip = null;

            switch (clip.Type)
            {
                case XClipType.Actor: xclip = new XActorClip(clip); break;
                case XClipType.Player: xclip = new XPlayerClip(clip); break;
                case XClipType.Fx: xclip = new XFxClip(clip); break;
                case XClipType.Audio: xclip = new XAudioClip(clip); break;
                case XClipType.SubTitle: xclip = new XSubTitleClip(clip); break;
                case XClipType.Slash: xclip = new XSlashClip(clip); break;
            }
            if (xclip != null)
            {
                xclip.Flush();
                _clips.Add(xclip, xclip);
            }
            return xclip;
        }

        public void RemoveClip(XClip clip)
        {
            int idx = _clips.IndexOfValue(clip);
            if (idx >= 0 && idx < _clips.Count) _clips.RemoveAt(idx);
        }

        void OnDestroy()
        {
            if (EditorUtility.DisplayDialog("Save or not",
                        "Do you want to save your changes?",
                        "Ok", "No"))
            {
                Save();
            }
        }

        void OnQuit()
        {
            if (!EditorApplication.isPlaying &&
                !EditorApplication.isUpdating &&
                !EditorApplication.isCompiling &&
                !EditorApplication.isPlayingOrWillChangePlaymode)
            {
                GameObject _cameraObject = GameObject.Find(@"Main Camera");
                DestroyImmediate(_cameraObject.GetComponent<XScriptStandalone>());
            }
        }


        void InnerLoad(XCutSceneData data)
        {
            _run_data = data;

            _name = data.Name;
            _script = data.Script;
            _scene = data.Scene;
            _camera = Resources.Load(data.CameraClip, typeof(AnimationClip)) as AnimationClip;
            _type_mask = data.TypeMask;
            _auto_end = data.AutoEnd;
            _general_show = data.GeneralShow;
            _general_bigguy = data.GeneralBigGuy;
            _override_bgm = data.OverrideBGM;
            _mourningborder = data.Mourningborder;
            _fov = data.FieldOfView;
            _length = data.Length;
            _trigger = (CameraTrigger)Enum.Parse(typeof(CameraTrigger), data.Trigger);

            _clips.Clear();
            ActorList.Clear();
            ActorList.Add("None");
            foreach (XActorDataClip clip in data.Actors)
            {
                TimeChecker(clip, data);
                XClip xclip = AddClip(clip);
                ActorList.Add(xclip.Name);
            }
            foreach (XPlayerDataClip clip in data.Player)
            {
                TimeChecker(clip, data);
                XClip xclip = AddClip(clip);
                ActorList.Add(xclip.Name);
            }
            foreach (XFxDataClip clip in data.Fxs)
            {
                TimeChecker(clip, data);
                AddClip(clip);
            }
            foreach (XAudioDataClip clip in data.Audios)
            {
                TimeChecker(clip, data);
                AddClip(clip);
            }
            foreach (XSubTitleDataClip clip in data.SubTitle)
            {
                TimeChecker(clip, data);
                AddClip(clip);
            }
            foreach (XSlashDataClip clip in data.Slash)
            {
                TimeChecker(clip, data);
                AddClip(clip);
            }

            if (_open_scene && _scene != null && _scene.Length != 0)
            {
                Scene scene = EditorSceneManager.GetActiveScene();
                if (scene.name.Length > 0 && !EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
                {
                    return;
                }
                else
                {
                    EditorSceneManager.OpenScene(_scene + ".unity");
                }
                _open_scene = false;
            }
        }

        void TimeChecker(XCutSceneClip clip, XCutSceneData data)
        {
            if (clip.TimeLineAt >= data.TotalFrame)
            {
                EditorUtility.DisplayDialog("Confirm your configuration.",
                            "clip play-at time bigger than cutscene length!",
                            "Ok");

                throw new Exception("clip time bigger than cutscene time!");
            }
        }

        void Load(XCutSceneData data)
        {
            if (EditorApplication.isPlaying)
                EditorApplication.isPlaying = false;
            InnerLoad(data);
        }

        XCutSceneData GetCurrentData()
        {
            if (_name == null || _name.Length == 0 || _camera == null) return null;

            XCutSceneData data = new XCutSceneData();
            data.CameraClip = XClip.LocateRes(_camera);
            data.Name = _name;
            data.Script = _script;
            data.Scene = _scene;
            data.TotalFrame = _camera.length * XEditorLibrary.FPS;
            data.TypeMask = _type_mask;
            data.AutoEnd = _auto_end;
            data.GeneralShow = _general_show;
            data.GeneralBigGuy = _general_bigguy;
            data.OverrideBGM = _override_bgm;
            data.Mourningborder = _mourningborder;
            data.FieldOfView = _fov;
            data.Length = _length;
            data.Trigger = _trigger.ToString();

            foreach (XClip clip in _clips.Values)
            {
                if (clip.Valid)
                {
                    if (_camera != null)
                    {
                        clip.Dump();
                        switch (clip.ClipType)
                        {
                            case XClipType.Actor: data.Actors.Add(clip.CutSceneClip as XActorDataClip); break;
                            case XClipType.Player: data.Player.Add(clip.CutSceneClip as XPlayerDataClip); break;
                            case XClipType.Audio: data.Audios.Add(clip.CutSceneClip as XAudioDataClip); break;
                            case XClipType.Fx: data.Fxs.Add(clip.CutSceneClip as XFxDataClip); break;
                            case XClipType.SubTitle: data.SubTitle.Add(clip.CutSceneClip as XSubTitleDataClip); break;
                            case XClipType.Slash: data.Slash.Add(clip.CutSceneClip as XSlashDataClip); break;
                        }
                    }
                }
            }
            return data;
        }

        void Save()
        {
            XCutSceneData data = GetCurrentData();
            if (data != null)
            {
                _run_data = data;

                string file = XEditorPath.Cts + data.Name + ".txt";
                XDataIO<XCutSceneData>.singleton.SerializeData(file, data);
            }
        }
        //GameObject o = null;
        void OnGUI()
        {
            if (_labelstyle == null)
            {
                _labelstyle = new GUIStyle(EditorStyles.boldLabel);
                _labelstyle.fontSize = 11;
            }

            scrollPosition = GUILayout.BeginScrollView(scrollPosition, false, false);
            EditorGUILayout.Space();
            GUILayout.Label("CutScene Editor:", _labelstyle);
            EditorGUILayout.Space();
            _name = EditorGUILayout.TextField("Name", _name);
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Scene", _scene);
            if (GUILayout.Button("Browser", GUILayout.MaxWidth(80)))
            {
                string file = EditorUtility.OpenFilePanel("Select unity scene file", XEditorPath.Sce, "unity");

                if (file.Length != 0)
                {
                    file = file.Remove(file.LastIndexOf("."));
                    _scene = file.Remove(0, file.IndexOf(XEditorPath.Sce));

                    Scene scene = EditorSceneManager.GetActiveScene();

                    if (scene.name.Length == 0 || !EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
                    {
                        EditorSceneManager.OpenScene(_scene + ".unity");
                    }
                }
            }
            EditorGUILayout.EndHorizontal();
            _auto_end = EditorGUILayout.Toggle("Auto End", _auto_end);
            EditorGUILayout.Space();
            _general_show = EditorGUILayout.Toggle("General Clip", _general_show);
            if (_general_show)
            {
                _general_bigguy = EditorGUILayout.Toggle("General Big Guy", _general_bigguy);

                if (_general_bigguy)
                    _camera = Resources.Load("Animation/Main_Camera/Cut_Scene/cutscene_generalshow_bigguy", typeof(AnimationClip)) as AnimationClip;
                else
                    _camera = Resources.Load("Animation/Main_Camera/Cut_Scene/cutscene_generalshow", typeof(AnimationClip)) as AnimationClip;

                EditorGUILayout.ObjectField("Camera Clip", _camera, typeof(AnimationClip), true);
            }
            else
                _camera = EditorGUILayout.ObjectField("Camera Clip", _camera, typeof(AnimationClip), true) as AnimationClip;
            if (_camera != null)
            {
                _length = _camera.length;
                EditorGUILayout.LabelField("CutScene Length", _length.ToString("F3") + "s" + "\t" + (_length * XEditorLibrary.FPS).ToString("F1") + "(frame)");
            }

            EditorGUILayout.Space();
            _type_mask = (int)(EntitySpecies)EditorGUILayout.EnumMaskField("Type Mask", (EntitySpecies)_type_mask);
            _mourningborder = EditorGUILayout.Toggle("Mourningborder", _mourningborder);

            EditorGUILayout.Space();
            _fov = EditorGUILayout.FloatField("FieldOfVeiw", _fov);
            _trigger = (CameraTrigger)EditorGUILayout.EnumPopup("Trigger", _trigger);
            EditorGUILayout.Space();
            _override_bgm = EditorGUILayout.Toggle("Override BGM", _override_bgm);
            EditorGUILayout.Space();

            UpdateScript();

            GUILayout.Label("TimeLine:", _labelstyle);
            EditorGUILayout.Space();
            EditorGUILayout.BeginHorizontal();
            _type = (XClipType)EditorGUILayout.EnumPopup("Add Clip", _type);
            if (GUILayout.Button(_content_add, GUILayout.MaxWidth(25), GUILayout.MaxHeight(15)))
            {
                if (_camera != null && _name != null && _name.Length > 0)
                {
                    XCutSceneTimelineWindow window = EditorWindow.GetWindow<XCutSceneTimelineWindow>(@"Timeline At:");
                    window._total_frame = _camera.length * XEditorLibrary.FPS;
                    window._clip = null;
                }
                else
                {
                    EditorUtility.DisplayDialog("Confirm your selection.",
                        "Please select camera clip or name the cutscene",
                        "Ok");
                }
            }
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.Space();
            ActorList.Clear();
            ActorList.Add("None");

            foreach (XClip clip in _clips.Values)
            {
                if (clip.Valid)
                {
                    if (_camera != null)
                    {
                        clip.TimeLineTotal = _camera.length * XEditorLibrary.FPS;
                        clip.OnGUI(GetCurrentData());

                        if (clip.ClipType == XClipType.Actor || clip.ClipType == XClipType.Player)
                        {
                            int all = ActorList.FindAll(s => s == clip.Name).Count;
                            if (all > 0)
                                ActorList.Add(clip.Name + " " + (all + 1));
                            else
                                ActorList.Add(clip.Name);
                        }
                    }
                }
            }

            EditorGUILayout.Space();
            EditorGUILayout.Space();
            EditorGUILayout.Space();

            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Run"))
            {
                if (_name != null && _name.Length > 0 && _camera != null && !EditorApplication.isPlaying)
                {
                    GameObject _cameraObject = GameObject.Find(@"Main Camera");
                    XScriptStandalone xss = _cameraObject.AddComponent<XScriptStandalone>();
                    _run_data = GetCurrentData();
                    xss._cut_scene_data = _run_data;
                    EditorApplication.ExecuteMenuItem("Edit/Play");
                }
            }

            if (GUILayout.Button("Pause"))
            {
                if (EditorApplication.isPlaying)
                    EditorApplication.isPaused = !EditorApplication.isPaused;
            }

            if (GUILayout.Button("Quit"))
            {
                if (EditorApplication.isPlaying)
                    EditorApplication.isPlaying = false;
            }
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.Space();
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Save"))
            {
                Save();
            }

            if (GUILayout.Button("Open"))
            {
                _file = EditorUtility.OpenFilePanel("Select cutscene file", XEditorPath.Cts, "txt");
                if (_file.Length != 0)
                {
                    _open_scene = true;
                    Load(XDataIO<XCutSceneData>.singleton.DeserializeData(_file.Substring(_file.IndexOf("Assets/"))));
                }
            }
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.EndScrollView();
        }

        void UpdateScript()
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.TextField("Script", _script);
            if (GUILayout.Button("Browser", GUILayout.MaxWidth(80)))
            {
                _script = ScriptFileBrowser();
            }
            EditorGUILayout.EndHorizontal();
        }

        private string ScriptFileBrowser()
        {
            string file = "";// EditorUtility.OpenFilePanel("Select script file", XSkillScriptGen.singleton.ScriptPath, "cs");

            if (file == null || file.Length == 0) return "";
            file = file.Remove(file.LastIndexOf('.'));
            return file.Substring(file.LastIndexOf('/') + 1);
        }
    }

}