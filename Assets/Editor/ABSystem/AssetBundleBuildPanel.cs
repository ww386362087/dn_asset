using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace ABSystem
{
    class AssetBundleBuildPanel : EditorWindow
    {
        [MenuItem("ABSystem/Builder Panel")]
        static void Open()
        {
            GetWindow<AssetBundleBuildPanel>("ABSystem", true);
        }

        
        public static bool ABUpdateOpen = false;
        public static List<XMetaResPackage> assetBundleUpdate = new List<XMetaResPackage>();
        public static string TargetFolder = "";

        public static T LoadAssetAtPath<T>(string path) where T : Object
        {
            return AssetDatabase.LoadAssetAtPath<T>(savePath);
        }

        public static string savePath
        {
            get { return "Assets/Editor/ABSystem/Config/config.asset"; }
        }

        private AssetBundleBuildConfig _config;
        private ReorderableList _list;
        private ReorderableList _fileList;

        public Vector2 scrollPosition = Vector2.zero;

        void OnListElementGUI(Rect rect, int index, bool isactive, bool isfocused)
        {
            const float GAP = 5;
            AssetBundleFilter filter = _config.filters[index];
            rect.y++;
            Rect r = rect;
            r.width = 16;
            r.height = 18;
            filter.valid = GUI.Toggle(r, filter.valid, GUIContent.none);

            r.xMin = r.xMax + GAP;
            r.xMax = rect.xMax - 300;
            GUI.enabled = false;
            filter.path = GUI.TextField(r, filter.path);
            GUI.enabled = true;

            r.xMin = r.xMax + GAP;
            r.width = 50;
            if (GUI.Button(r, "Select"))
            {
                string dataPath = Application.dataPath;
                string selectedPath = EditorUtility.OpenFolderPanel("Path", dataPath, "");
                if (!string.IsNullOrEmpty(selectedPath))
                {
                    if (selectedPath.StartsWith(dataPath))
                    {
                        filter.path = "Assets/" + selectedPath.Substring(dataPath.Length + 1);
                    }
                    else
                    {
                        ShowNotification(new GUIContent("不能在Assets目录之外!"));
                    }
                }
            }
            r.xMin = r.xMax + GAP;
            r.xMax = rect.xMax;
            filter.filter = GUI.TextField(r, filter.filter);
        }

        void OnFileListElementGUI(Rect rect, int index, bool isactive, bool isfocused)
        {
            const float GAP = 5;

            AssetBundleFilter file = _config.files[index];
            rect.y++;

            Rect r = rect;
            r.width = 16;
            r.height = 18;
            file.valid = GUI.Toggle(r, file.valid, GUIContent.none);

            r.xMin = r.xMax + GAP;
            r.xMax = rect.xMax - 300;
            GUI.enabled = false;
            file.path = GUI.TextField(r, file.path);
            GUI.enabled = true;

            r.xMin = r.xMax + GAP;
            r.width = 50;
            if (GUI.Button(r, "Select"))
            {
                string dataPath = Application.dataPath;
                string selectedPath = EditorUtility.OpenFilePanel("Path", dataPath, "");
                if (!string.IsNullOrEmpty(selectedPath))
                {
                    if (selectedPath.StartsWith(dataPath))
                    {
                        file.path = "Assets/" + selectedPath.Substring(dataPath.Length + 1);
                    }
                    else
                    {
                        ShowNotification(new GUIContent("不能在Assets目录之外!"));
                    }
                }
            }

            r.xMin = r.xMax + GAP;
            r.xMax = rect.xMax;
            file.filter = GUI.TextField(r, file.filter);
        }

        void OnListHeaderGUI(Rect rect)
        {
            EditorGUI.LabelField(rect, "Asset Filter");
        }

        void OnFileListHeaderGUI(Rect rect)
        {
            EditorGUI.LabelField(rect, "Asset File");
        }

        void OnGUI()
        {
            bool execBuild = false;
            if (_config == null)
            {
                _config = LoadAssetAtPath<AssetBundleBuildConfig>(savePath);
                if (_config == null)
                {
                    _config = CreateInstance<AssetBundleBuildConfig>(); 
                }
            }

            if (_list == null)
            {
                _list = new ReorderableList(_config.filters, typeof(AssetBundleFilter));
                _list.drawElementCallback = OnListElementGUI;
                _list.drawHeaderCallback = OnListHeaderGUI;
                _list.draggable = true;
                _list.elementHeight = 22;
            }

            //tool bar
            GUILayout.BeginHorizontal(EditorStyles.toolbar);
            {
                if (GUILayout.Button("Add", EditorStyles.toolbarButton))
                {
                    _config.filters.Add(new AssetBundleFilter());
                }
                if (GUILayout.Button("Save", EditorStyles.toolbarButton))
                {
                    Save();
                }
                GUILayout.FlexibleSpace();
                if (GUILayout.Button("Build", EditorStyles.toolbarButton))
                {
                    execBuild = true;
                }
            }
            GUILayout.EndHorizontal();

            scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition, false, false);

            if (_fileList == null)
            {
                _fileList = new ReorderableList(_config.files, typeof(AssetBundleFilter));
                _fileList.drawElementCallback = OnFileListElementGUI;
                _fileList.drawHeaderCallback = OnFileListHeaderGUI;
                _fileList.draggable = true;
                _fileList.elementHeight = 22;
            }

            //context
            GUILayout.BeginVertical();

            //format
            GUILayout.BeginHorizontal();
            {
                EditorGUILayout.PrefixLabel("DepInfoFileFormat");
                _config.depInfoFileFormat = (AssetBundleBuildConfig.Format)EditorGUILayout.EnumPopup(_config.depInfoFileFormat);
            }
            GUILayout.EndHorizontal();

            GUILayout.Space(10);
            _list.DoLayoutList();
            GUILayout.EndVertical();

            GUILayout.Space(10);
            _fileList.DoLayoutList();
            EditorGUILayout.EndScrollView();
            if (GUI.changed) EditorUtility.SetDirty(_config);

            if (execBuild) Build();
        }

        private void Build()
        {
            Save();
            BuildAssetBundles();
        }

        public static void BuildAssetBundles()
        {
            AssetBundleBuildConfig config = LoadAssetAtPath<AssetBundleBuildConfig>(savePath);
            ABBuilder builder = new ABBuilder();
            builder.SetDataWriter(config.depInfoFileFormat == AssetBundleBuildConfig.Format.Text ?
                new AssetBundleDataWriter() : new AssetBundleDataBinaryWriter());

            builder.Begin();

            for (int i = 0; i < config.filters.Count; i++)
            {
                AssetBundleFilter f = config.filters[i];
                if (f.valid && Directory.Exists(f.path))
                {
                    builder.AddRootTargets(new DirectoryInfo(f.path), new string[] { f.filter });
                }
            }

            for (int i = 0; i < config.files.Count; ++i)
            {
                AssetBundleFilter f = config.files[i];
                if (f.valid && File.Exists(f.path)) builder.AddRootTargets(f.path);
            }
            builder.Export();
            builder.End();

            EditorUtility.DisplayDialog("AssetBundle Build Finish", "AssetBundle Build Finish!", "OK");
        }

        void Save()
        {
            if (LoadAssetAtPath<AssetBundleBuildConfig>(savePath) == null)
            {
                AssetDatabase.CreateAsset(_config, savePath);
            }
            else
            {
                EditorUtility.SetDirty(_config);
            }
        }

    }


}