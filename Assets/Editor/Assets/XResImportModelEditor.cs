using UnityEngine;
using System.Collections;
using UnityEditor;
using System.Collections.Generic;
using System.IO;
using XEditor;


namespace XEditor
{
    public enum ETextureSize
    {
        Original,
        Half,
        X32,
        X64,
        X128,
        X256,
        X512,
    }

    public enum ETextureCompress
    {
        TrueColor,
        Alpha8,
        Compress,
        Delete,
        RGB16,
    }

    public class XResImportEditor
    {
        public static bool bAccordingSettings = true;
        public static XImageImporterSet Sets = null;
    }


    public class XUITextureImporterData
    {

        public string path = "";
        public ETextureSize textureSize = ETextureSize.Original;
        public ETextureCompress textureCompress = ETextureCompress.TrueColor;
        private string name = "";
        private Texture2D tex = null;
        public string GetName()
        {
            return name;
        }
        public void SetName(string n)
        {
            name = n;
        }
        public Texture2D GetTex()
        {
            return tex;
        }
        public void SetTex(Texture2D t)
        {
            tex = t;
        }

        public int GetSize(int srcSize)
        {
            switch (textureSize)
            {
                case ETextureSize.Original:
                    return srcSize;
                case ETextureSize.Half:
                    return srcSize / 2;
                case ETextureSize.X32:
                    return 32;
                case ETextureSize.X64:
                    return 64;
                case ETextureSize.X128:
                    return 128;
                case ETextureSize.X256:
                    return 256;
                case ETextureSize.X512:
                    return 512;
            }
            return srcSize;
        }
    }

    public class XImageImporterSet
    {
        public List<XUITextureImporterData> ImageSet = new List<XUITextureImporterData>();
        public void Init()
        {
            HashSet<string> names = new HashSet<string>();
            for (int i = ImageSet.Count - 1; i >= 0; --i)
            {
                XUITextureImporterData data = ImageSet[i];
                Texture2D tex = AssetDatabase.LoadAssetAtPath(data.path, typeof(UnityEngine.Texture2D)) as Texture2D;
                if (tex != null)
                {
                    data.SetTex(tex);
                    data.SetName(tex.name);
                    if (names.Contains(tex.name))
                    {
                        ImageSet.RemoveAt(i);
                    }
                    else
                    {
                        names.Add(tex.name);
                    }
                }
                else
                {
                    string name = data.path;
                    int index = data.path.LastIndexOf("/");
                    if (index >= 0)
                    {
                        name = data.path.Substring(index + 1);
                    }
                    index = name.LastIndexOf(".");
                    if (index >= 0)
                    {
                        name = name.Substring(0, index);
                    }
                    data.SetName(name);
                }
            }
        }
        public XUITextureImporterData Find(string name)
        {
            for (int i = 0; i < ImageSet.Count; ++i)
            {
                XUITextureImporterData data = ImageSet[i];
                if (data != null && data.GetName() == name)
                {
                    return data;
                }
            }
            return null;
        }
    }

    public class XResModelImportEditor
    {
        public static bool bAccordingSettings = true;
        public static XModelImporterSet Sets = null;

        [MenuItem(@"XEditor/Res Import Setting.../Model Settings")]
        static void Execute()
        {
            EditorWindow.GetWindowWithRect<XResModelImportEditorWnd>(new Rect(0, 0, 900, 500), true, @"XRes Import Editor");
        }

        private static bool IsAssetAFolder(UnityEngine.Object obj)
        {
            string path = "";
            if (obj == null) return false;
            path = AssetDatabase.GetAssetPath(obj.GetInstanceID());
            if (path.Length > 0)
            {
                return Directory.Exists(path);
            }
            return false;
        }

        [MenuItem("Assets/Re-import Res by default")]
        static void ReImportResbyDefault()
        {
            bAccordingSettings = false;
            UnityEngine.Object[] os = Selection.GetFiltered(typeof(UnityEngine.Object), SelectionMode.Assets);
            foreach (UnityEngine.Object o in os)
            {
                string path = AssetDatabase.GetAssetPath(o.GetInstanceID());
                if (path.EndsWith(".meta")) continue;
                AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceSynchronousImport | ImportAssetOptions.ImportRecursive);
            }
            bAccordingSettings = true;
        }
    }

    public class XModelImporterSet
    {
        public List<XModelImporterData> ModelSet = new List<XModelImporterData>();
    }

    public class XModelImporterData
    {
        public string model;

        public ModelImporterMeshCompression compression;
        public ModelImporterTangentSpaceMode normal;
        public ModelImporterTangentSpaceMode tangent;
        public bool active = true;
    }

    [ExecuteInEditMode]
    internal class XResModelImportEditorWnd : EditorWindow
    {
        private Vector2 scrollPosition = Vector2.zero;

        private GUIStyle _labelstyle = null;
        private GUIStyle _labelstyle_1 = null;

        private GameObject _model = null;
        private XModelImporterData _data = new XModelImporterData();

        private XModelImporterSet _set = new XModelImporterSet();

        void OnEnable()
        {
            _set = XDataIO<XModelImporterSet>.singleton.DeserializeData("Assets/Editor/ResImporter/ImporterData/Model/ResourceImportXML.xml");
        }

        void OnGUI()
        {
            scrollPosition = GUILayout.BeginScrollView(scrollPosition, false, false);

            if (_labelstyle == null)
            {
                _labelstyle = new GUIStyle(EditorStyles.boldLabel);
                _labelstyle.fontSize = 11;
            }

            if (_labelstyle_1 == null)
            {
                _labelstyle_1 = new GUIStyle(EditorStyles.boldLabel);
                _labelstyle_1.fontStyle = FontStyle.BoldAndItalic;
                _labelstyle_1.fontSize = 11;
            }

            EditorGUILayout.Space();
            GUILayout.Label("Resource Importer:", _labelstyle);
            EditorGUILayout.Space();

            _model = EditorGUILayout.ObjectField("Select Model:", _model, typeof(GameObject), true) as GameObject;

            if (_model != null)
            {
                string path = AssetDatabase.GetAssetPath(_model);

                if (path.EndsWith(".FBX") || path.EndsWith(".fbx"))
                {
                    _data.model = path;
                    EditorGUILayout.LabelField("Path", _data.model);

                    EditorGUILayout.Space();
                    _data.compression = (ModelImporterMeshCompression)EditorGUILayout.EnumPopup("Mesh Compression", (ModelImporterMeshCompression)_data.compression);
                    _data.normal = (ModelImporterTangentSpaceMode)EditorGUILayout.EnumPopup("Normals", (ModelImporterTangentSpaceMode)_data.normal);

                    if (_data.normal != ModelImporterTangentSpaceMode.None)
                        _data.tangent = (ModelImporterTangentSpaceMode)EditorGUILayout.EnumPopup("Tangents", (ModelImporterTangentSpaceMode)_data.tangent);
                    else
                        _data.tangent = ModelImporterTangentSpaceMode.None;

                    if (GUILayout.Button("Confirm", GUILayout.MaxWidth(80)))
                    {
                        XModelImporterData data = null;

                        foreach (XModelImporterData model in _set.ModelSet)
                        {
                            if (model.model == _data.model)
                            {
                                data = model;
                                break;
                            }
                        }

                        if (data == null) data = new XModelImporterData();

                        data.model = _data.model;
                        data.compression = _data.compression;
                        data.normal = _data.normal;
                        data.tangent = _data.tangent;

                        if (!_set.ModelSet.Contains(data)) _set.ModelSet.Add(data);

                        XDataIO<XModelImporterSet>.singleton.SerializeData("Assets/Editor/ResImporter/ImporterData/Model/ResourceImportXML.xml", _set);
                        _model = null;

                        XResImportEditor.Sets = null;
                        AssetDatabase.Refresh();
                    }
                }
                else
                    _model = null;
            }

            EditorGUILayout.Space();
            EditorGUILayout.Space();
            if (_set.ModelSet.Count > 0)
            {
                EditorGUILayout.LabelField("Resources Detail :", _labelstyle);
                EditorGUILayout.Space();
                GUILayout.BeginHorizontal();
                GUILayout.Label("", new GUILayoutOption[] { GUILayout.Width(120) });
                GUILayout.Label("Path", _labelstyle_1, new GUILayoutOption[] { GUILayout.Width(300) });
                GUILayout.Label("Compression", _labelstyle_1, new GUILayoutOption[] { GUILayout.Width(100) });
                GUILayout.Label("Normal", _labelstyle_1, new GUILayoutOption[] { GUILayout.Width(70) });
                GUILayout.Label("Tangent", _labelstyle_1, new GUILayoutOption[] { GUILayout.Width(70) });
                GUILayout.Label("", new GUILayoutOption[] { GUILayout.Width(50) });

                GUILayout.EndHorizontal();

                foreach (XModelImporterData model in _set.ModelSet)
                {
                    int n = model.model.LastIndexOf("/");
                    string name = model.model.Substring(n + 1);
                    name = name.Substring(0, name.Length - 4);

                    string path = model.model.Substring(0, n);
                    path = path.Substring(7);

                    GUILayout.BeginHorizontal();
                    GUILayout.Label(new GUIContent(name, name), _labelstyle_1, new GUILayoutOption[] { GUILayout.Width(120) });
                    GUILayout.Label(new GUIContent(path, path), _labelstyle_1, new GUILayoutOption[] { GUILayout.Width(300) });
                    GUILayout.Label(model.compression.ToString(), _labelstyle_1, new GUILayoutOption[] { GUILayout.Width(100) });
                    GUILayout.Label(model.normal.ToString(), _labelstyle_1, new GUILayoutOption[] { GUILayout.Width(70) });
                    GUILayout.Label(model.tangent.ToString(), _labelstyle_1, new GUILayoutOption[] { GUILayout.Width(70) });
                    if (GUILayout.Button("Edit"))
                    {
                        _model = AssetDatabase.LoadAssetAtPath(model.model, typeof(GameObject)) as GameObject;
                        _data = model;
                    }
                    if (GUILayout.Button("ReImp"))
                    {
                        AssetDatabase.ImportAsset(model.model);
                        AssetDatabase.Refresh();
                    }
                    if (GUILayout.Button("Del"))
                    {
                        if (EditorUtility.DisplayDialog("Confirm your delete",
                            "Are you sure to delete it?",
                            "Ok", "Cancel"))
                            model.active = false;
                    }

                    GUILayout.EndHorizontal();
                }

                for (int i = _set.ModelSet.Count - 1; i >= 0; i--)
                {
                    if (!_set.ModelSet[i].active)
                    {
                        string model = _set.ModelSet[i].model;
                        _set.ModelSet.RemoveAt(i);

                        XDataIO<XModelImporterSet>.singleton.SerializeData("Assets/Editor/ResImporter/ImporterData/Model/ResourceImportXML.xml", _set);
                        XResImportEditor.Sets = null;

                        AssetDatabase.ImportAsset(model);
                        AssetDatabase.Refresh();
                    }
                }
            }

            EditorGUILayout.EndScrollView();
        }
    }

}