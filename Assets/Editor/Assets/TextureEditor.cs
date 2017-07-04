using UnityEngine;
using System.Collections.Generic;
using UnityEditor;
using System.Reflection;
using System;
using System.IO;


namespace XEditor
{
    [CustomEditor(typeof(TextureImporter))]
    [CanEditMultipleObjects]
    public class TextureEditor : Editor
    {
        public enum ETextureCompress
        {
            Compress,
            TrueColor,
            RGB16
        }
        public enum ETextureSize
        {
            Original,
            Half,
            Quarter,
            X32,
            X64,
            X128,
            X256,
            X512,
        }
        public class TexFormat
        {
            public ETextureCompress srcFormat = ETextureCompress.Compress;
            public ETextureSize alphaSize = ETextureSize.Original;
        }
        private Editor nativeEditor;
        private string path;
        private bool isUITex = false;
        private bool cannotHotfix = false;
        private bool needAlpha = false;
        private TextureImporter texImporter = null;
        private ETextureCompress srcFormat = ETextureCompress.Compress;
        private ETextureSize alphaSize = ETextureSize.Original;
        private List<TexFormat> texFormat = new List<TexFormat>();
        private static GUIStyle buttonStyle = null;
        private GUIStyle warningStyle = null;
        void TargetUpdate(SceneView sceneview)
        {
            Event e = Event.current;
        }

        public static bool IsDefaultFormat(bool isAtlas, TextureEditor.ETextureCompress format, TextureEditor.ETextureSize size)
        {
            if (isAtlas)
            {
                return format == TextureEditor.ETextureCompress.Compress && size == TextureEditor.ETextureSize.Original;
            }
            else
            {
                return format == TextureEditor.ETextureCompress.Compress && size == TextureEditor.ETextureSize.Half;
            }
        }

        public static void GetTexFormat(bool isAtlas, string userData, out ETextureCompress format, out ETextureSize size)
        {
            format = ETextureCompress.Compress;
            size = isAtlas ? ETextureSize.Original : ETextureSize.Half;
            if (!string.IsNullOrEmpty(userData))
            {
                string[] str = userData.Split(' ');
                if (str.Length > 0)
                {
                    try
                    {
                        format = (ETextureCompress)int.Parse(str[0]);
                    }
                    catch (Exception) { }
                }
                if (str.Length > 1)
                {
                    try
                    {
                        size = (ETextureSize)int.Parse(str[1]);
                    }
                    catch (Exception) { }
                }
            }
        }

        public static bool IsAtlas(string path)
        {
            int index = path.LastIndexOf(".");
            if (index >= 0)
            {
                path = path.Substring(0, index);
            }
            Material mat = AssetDatabase.LoadAssetAtPath(path + ".mat", typeof(Material)) as Material;
            GameObject atlas = AssetDatabase.LoadAssetAtPath(path + ".prefab", typeof(GameObject)) as GameObject;
            return mat != null && atlas != null;
        }

        public static int GetSize(int srcSize, ETextureSize size)
        {
            switch (size)
            {
                case ETextureSize.Original:
                    return srcSize;
                case ETextureSize.Half:
                    return srcSize / 2;
                case ETextureSize.Quarter:
                    return srcSize / 4;
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

        public void OnEnable()
        {
            texImporter = target as TextureImporter;
            texFormat.Clear();
            if (texImporter != null)
            {
                path = texImporter.assetPath;
                cannotHotfix = path.StartsWith("Assets/Resources/StaticUI");
                isUITex = (path.StartsWith("Assets/Resources/atlas/UI") || path.StartsWith("Assets/Resources/StaticUI")) && !path.EndsWith("_A.png");
                if (isUITex)
                {
                    string userData = texImporter.userData;

                    GetTexFormat(IsAtlas(path), texImporter.userData, out srcFormat, out alphaSize);
                    for (int i = 0; i < targets.Length; ++i)
                    {
                        TextureImporter ti = targets[i] as TextureImporter;
                        if (ti != null)
                        {
                            needAlpha = ti.DoesSourceTextureHaveAlpha();
                            TexFormat tf = new TexFormat();
                            GetTexFormat(IsAtlas(ti.assetPath), ti.userData, out tf.srcFormat, out tf.alphaSize);
                            texFormat.Add(tf);
                            if (tf.srcFormat != srcFormat)
                            {
                                srcFormat = (ETextureCompress)(-1);
                            }
                            if (tf.alphaSize != alphaSize)
                            {
                                alphaSize = (ETextureSize)(-1);
                            }
                        }
                    }
                }
            }

            SceneView.onSceneGUIDelegate = TargetUpdate;
            Type t = null;

            foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                foreach (Type type in assembly.GetTypes())
                {
                    if (type.Name.ToLower().Contains("textureimporterinspector"))
                    {
                        t = type;
                        break;
                    }
                }
            }
            nativeEditor = Editor.CreateEditor(target, t);

        }

        public override void OnInspectorGUI()
        {
            if (buttonStyle == null)
            {
                buttonStyle = new GUIStyle(EditorStyles.boldLabel);
                buttonStyle.alignment = TextAnchor.MiddleRight;
            }
            nativeEditor.OnInspectorGUI();
            if (cannotHotfix)
            {
                if (warningStyle == null)
                {
                    warningStyle = new GUIStyle(GUI.skin.label);
                    warningStyle.fontStyle = FontStyle.Bold;
                    warningStyle.normal.textColor = new Color(1, 0, 0);
                    warningStyle.fontSize = 20;
                }
                GUILayout.Label("此图不能热更新!!!", warningStyle);
            }
            if (isUITex && texImporter != null)
            {
                GUILayout.Space(20);
                ETextureCompress newSrcFormat = (ETextureCompress)EditorGUILayout.EnumPopup("压缩", srcFormat);
                ETextureSize newAlphaSize = (ETextureSize)EditorGUILayout.EnumPopup("Alpha缩放", alphaSize);
                if (newSrcFormat != srcFormat || newAlphaSize != alphaSize)
                {
                    if (newSrcFormat != srcFormat)
                    {
                        srcFormat = newSrcFormat;
                    }
                    if (newAlphaSize != alphaSize)
                    {
                        alphaSize = newAlphaSize;
                    }
                }
                GUILayout.Label(needAlpha ? "有alpha" : "无alpha");
                GUILayout.Space(10);
                GUILayout.BeginHorizontal();
                if (GUILayout.Button("Apply", GUILayout.MaxWidth(50)))
                {
                    int format = (int)newSrcFormat;
                    int size = (int)newAlphaSize;
                    for (int i = 0; i < targets.Length; ++i)
                    {
                        TextureImporter ti = targets[i] as TextureImporter;
                        TexFormat tf = texFormat[i];
                        if (ti != null)
                        {
                            int f = format == -1 ? (int)tf.srcFormat : format;
                            int s = size == -1 ? (int)tf.alphaSize : size;
                            ti.userData = string.Format("{0} {1}", f, s);
                            EditorUtility.DisplayProgressBar(string.Format("Processing-{0}/{1}", i, targets.Length), ti.assetPath, (float)i / targets.Length);
                        }
                    }
                    EditorUtility.ClearProgressBar();
                    EditorUtility.DisplayDialog("Finish", "All textures processed finish", "OK");
                }
                if (GUILayout.Button("Texture Status", GUILayout.MaxWidth(100)))
                {
                    TextureStatus window = (TextureStatus)EditorWindow.GetWindow(typeof(TextureStatus), true, "TextureStatus");
                    window.Show();
                }
                GUILayout.EndHorizontal();
            }
        }
    }

    internal class TextureStatus : EditorWindow
    {
        private Vector2 scrollPosition = Vector2.zero;

        private GUIStyle _labelstyle = null;
        private GUIStyle _labelstyle_1 = null;
        public class TexInfo
        {
            public string path = "";
            public TextureEditor.ETextureCompress srcFormat = TextureEditor.ETextureCompress.Compress;
            public TextureEditor.ETextureSize alphaSize = TextureEditor.ETextureSize.Original;
            public bool isAtlas = false;
        }
        private GameObject _model = null;
        private static List<TexInfo> allTexStatus = new List<TexInfo>();
        private int CompareNewMsg(TexInfo a, TexInfo b)
        {
            return a.path.CompareTo(b.path);
        }

        private void RefreshTexStatus()
        {
            allTexStatus.Clear();
            string[] arrStrPath = Directory.GetFiles(Application.dataPath + "/Resources/atlas/UI/", "*.png", SearchOption.AllDirectories);
            for (int i = 0; i < arrStrPath.Length; ++i)
            {
                string strTempPath = arrStrPath[i].Replace(@"\", "/");
                strTempPath = strTempPath.Substring(strTempPath.IndexOf("Assets"));
                TextureImporter textureImporter = AssetImporter.GetAtPath(strTempPath) as TextureImporter;
                if (textureImporter != null)
                {
                    TextureEditor.ETextureCompress f = TextureEditor.ETextureCompress.Compress;
                    TextureEditor.ETextureSize s = TextureEditor.ETextureSize.Original;
                    bool isAtlas = TextureEditor.IsAtlas(strTempPath);
                    TextureEditor.GetTexFormat(isAtlas, textureImporter.userData, out f, out s);

                    if (!TextureEditor.IsDefaultFormat(isAtlas, f, s))
                    {
                        TexInfo tf = new TexInfo();
                        tf.path = strTempPath;
                        tf.srcFormat = f;
                        tf.alphaSize = s;
                        tf.isAtlas = isAtlas;
                        allTexStatus.Add(tf);
                    }
                }
            }
            allTexStatus.Sort(CompareNewMsg);
        }

        private void SetDefaultAlpha()
        {
            string[] arrStrPath = Directory.GetFiles(Application.dataPath + "/Resources/atlas/UI/", "*.png", SearchOption.AllDirectories);

            for (int i = 0; i < arrStrPath.Length; ++i)
            {
                string strTempPath = arrStrPath[i].Replace(@"\", "/");
                strTempPath = strTempPath.Substring(strTempPath.IndexOf("Assets"));
                TextureImporter textureImporter = AssetImporter.GetAtPath(strTempPath) as TextureImporter;
                if (textureImporter != null)
                {
                    TextureEditor.ETextureCompress f = TextureEditor.ETextureCompress.Compress;
                    TextureEditor.ETextureSize s = TextureEditor.ETextureSize.Original;
                    bool isAtlas = TextureEditor.IsAtlas(strTempPath);
                    TextureEditor.GetTexFormat(isAtlas, textureImporter.userData, out f, out s);
                    if (!isAtlas && (f != TextureEditor.ETextureCompress.Compress || s != TextureEditor.ETextureSize.Half))
                    {
                        f = TextureEditor.ETextureCompress.Compress;
                        s = TextureEditor.ETextureSize.Half;
                        textureImporter.userData = string.Format("{0} {1}", (int)f, (int)s);
                        AssetDatabase.ImportAsset(textureImporter.assetPath, ImportAssetOptions.ForceUpdate);
                    }
                }
                EditorUtility.DisplayProgressBar(string.Format("SetDefaultAlpha:{0}/{1}", i, arrStrPath.Length), strTempPath, (float)i / arrStrPath.Length);
            }
            EditorUtility.ClearProgressBar();
            EditorUtility.DisplayDialog("Finish", "All textures processed finish", "OK");
        }
        private void OnEnable()
        {
            RefreshTexStatus();
        }

        void OnGUI()
        {
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
            GUILayout.Label("UI Texture Stats:", _labelstyle);
            if (GUILayout.Button("SetDefault", GUILayout.MaxWidth(80)))
            {
                SetDefaultAlpha();
            }
            EditorGUILayout.Space();

            scrollPosition = GUILayout.BeginScrollView(scrollPosition, false, false);
            for (int i = 0; i < allTexStatus.Count; ++i)
            {
                TexInfo tf = allTexStatus[i];
                GUILayout.BeginHorizontal();
                GUILayout.Label(tf.path);
                EditorGUILayout.EnumPopup("压缩", tf.srcFormat);
                EditorGUILayout.EnumPopup("Alpha缩放", tf.alphaSize);
                GUILayout.EndHorizontal();
            }
            EditorGUILayout.EndScrollView();
        }
    }


    public class TextureModify
    {

        public static int scaleSize = 2;

        public delegate bool EnumTexCallback(Texture2D tex, TextureImporter textureImporter, string path);
        public delegate void EnumTexPostCallback();
        public static void EnumTextures(EnumTexCallback cb, string title, EnumTexPostCallback postCb = null)
        {
            UnityEngine.Object[] textures = Selection.GetFiltered(typeof(Texture), SelectionMode.DeepAssets);
            if (textures != null)
            {
                for (int i = 0; i < textures.Length; ++i)
                {
                    Texture2D tex = textures[i] as Texture2D;
                    string path = "";
                    if (tex != null)
                    {
                        path = AssetDatabase.GetAssetPath(tex);
                        TextureImporter textureImporter = AssetImporter.GetAtPath(path) as TextureImporter;
                        if (cb(tex, textureImporter, path))
                        {
                            AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceUpdate);
                        }
                    }
                    EditorUtility.DisplayProgressBar(string.Format("{0}-{1}/{2}", title, i, textures.Length), path, (float)i / textures.Length);
                }
            }
            AssetDatabase.Refresh();
            EditorUtility.ClearProgressBar();
            if (postCb != null)
            {
                postCb();
            }
            EditorUtility.DisplayDialog("Finish", "All textures processed finish", "OK");
        }

        public static Texture2D MakeAlphaRTex(string alphaTexPath, int size, Texture2D src)
        {
            Texture2D alphaTex = new Texture2D(src.width, src.height, TextureFormat.ARGB32, false);
            for (int y = 0, ymax = src.height; y < ymax; ++y)
            {
                for (int x = 0, xmax = src.width; x < xmax; ++x)
                {
                    Color c0 = src.GetPixel(x, y);
                    Color a = new Color(c0.a, 1, 1, 1);
                    alphaTex.SetPixel(x, y, a);
                }
            }
            byte[] bytes = alphaTex.EncodeToPNG();
            File.WriteAllBytes(alphaTexPath, bytes);
            AssetDatabase.ImportAsset(alphaTexPath, ImportAssetOptions.ForceUpdate);
            TextureImporter alphaTextureImporter = AssetImporter.GetAtPath(alphaTexPath) as TextureImporter;
            if (alphaTextureImporter != null)
            {
                int alphaSize = size;
                alphaTextureImporter.textureType = TextureImporterType.Advanced;
                alphaTextureImporter.anisoLevel = 0;
                alphaTextureImporter.mipmapEnabled = false;
                alphaTextureImporter.isReadable = false;
                alphaTextureImporter.npotScale = TextureImporterNPOTScale.ToNearest;
                alphaTextureImporter.SetPlatformTextureSettings(BuildTarget.Android.ToString(), alphaSize, TextureImporterFormat.ETC_RGB4);
                alphaTextureImporter.SetPlatformTextureSettings(BuildTarget.iPhone.ToString(), alphaSize, TextureImporterFormat.PVRTC_RGB4);
                AssetDatabase.ImportAsset(alphaTexPath, ImportAssetOptions.ForceUpdate);
            }
            return AssetDatabase.LoadAssetAtPath(alphaTexPath, typeof(Texture2D)) as Texture2D;
        }

        /// <summary>
        /// 生成一张alpha图
        /// </summary>
        public static Texture2D ConvertTexRtex(Texture2D src)
        {
            string texPath = AssetDatabase.GetAssetPath(src);
            int index = texPath.LastIndexOf('.');
            string alphaTexPath = texPath.Substring(0, index) + "_A.png";
            int size = src.width > src.height ? src.width : src.height;
            TextureImporter texImporter = AssetImporter.GetAtPath(texPath) as TextureImporter;
            if (!texImporter.DoesSourceTextureHaveAlpha()) return null;
            texImporter.textureType = TextureImporterType.Advanced;
            texImporter.anisoLevel = 0;
            texImporter.mipmapEnabled = size > 256;
            texImporter.npotScale = TextureImporterNPOTScale.ToNearest;
            texImporter.wrapMode = TextureWrapMode.Repeat;
            texImporter.filterMode = FilterMode.Bilinear;
            texImporter.SetPlatformTextureSettings(BuildTarget.Android.ToString(), size, TextureImporterFormat.RGBA32);
            texImporter.SetPlatformTextureSettings(BuildTarget.iPhone.ToString(), size, TextureImporterFormat.RGBA32);
            texImporter.isReadable = true;
            AssetDatabase.ImportAsset(texPath, ImportAssetOptions.ForceUpdate);
            Texture2D alphaTex = MakeAlphaRTex(alphaTexPath, size / scaleSize, src);
            texImporter.SetPlatformTextureSettings(BuildTarget.Android.ToString(), size, TextureImporterFormat.ETC_RGB4);
            texImporter.SetPlatformTextureSettings(BuildTarget.iPhone.ToString(), size, TextureImporterFormat.PVRTC_RGB4);
            texImporter.isReadable = false;
            AssetDatabase.ImportAsset(texPath, ImportAssetOptions.ForceUpdate);
            return alphaTex;
        }

        //压缩图片
        private static bool _DefaultCompress(Texture2D tex, TextureImporter textureImporter, string path)
        {
            if (path.IndexOf("_A.") >= 0 || path.IndexOf("_NRM.") >= 0)
                return false;
            int size = tex.width > tex.height ? tex.width : tex.height;
            textureImporter.SetPlatformTextureSettings(BuildTarget.Android.ToString(), size, TextureImporterFormat.RGBA32);
            textureImporter.SetPlatformTextureSettings(BuildTarget.iPhone.ToString(), size, TextureImporterFormat.RGBA32);
            textureImporter.textureType = TextureImporterType.Advanced;
            textureImporter.anisoLevel = 0;
            textureImporter.mipmapEnabled = size > 256;
            textureImporter.npotScale = TextureImporterNPOTScale.ToNearest;
            textureImporter.wrapMode = TextureWrapMode.Repeat;
            textureImporter.filterMode = FilterMode.Bilinear;

            if (textureImporter.DoesSourceTextureHaveAlpha())
            {
                textureImporter.isReadable = true;
                AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceUpdate);
                int extIndex = path.LastIndexOf(".");
                if (extIndex >= 0)
                {
                    textureImporter.isReadable = true;

                    string alphaTexPath = path.Substring(0, extIndex) + "_A.png";

                    MakeAlphaRTex(alphaTexPath, size / 2, tex);
                }
            }
            textureImporter.SetPlatformTextureSettings(BuildTarget.Android.ToString(), size, TextureImporterFormat.ETC_RGB4);
            textureImporter.SetPlatformTextureSettings(BuildTarget.iPhone.ToString(), size, TextureImporterFormat.PVRTC_RGB4);
            textureImporter.isReadable = false;
            return true;
        }

        [MenuItem(@"Assets/Tool/Texture/DefaultCompress")]
        private static void DefaultCompress()
        {
            EnumTextures(_DefaultCompress, "DefaultCompress");
        }

        [MenuItem(@"Assets/Tool/Texture/TextureCombine")]
        private static void CombineTex()
        {
            Rect wr = new Rect(0, 0, 800, 800);
            TextureCombine window = (TextureCombine)EditorWindow.GetWindowWithRect(typeof(TextureCombine), wr, true, "合并贴图");
            window.Show();
        }

        [MenuItem(@"Assets/Tool/Res/FindTex")]
        private static void FindTex()
        {
            //Rect wr = new Rect(0, 0, 1800, 1200);
            TextureFindEditor window = (TextureFindEditor)EditorWindow.GetWindow(typeof(TextureFindEditor), true, "查找贴图");
            window.Show();
        }

        [MenuItem(@"Assets/Tool/Texture/Compress")]
        private static void Compress()
        {
            Rect wr = new Rect(0, 0, 300, 400);
            TextureCommonCompress window = (TextureCommonCompress)EditorWindow.GetWindowWithRect(typeof(TextureCommonCompress), wr, true, "压缩贴图");
            window.Show();
        }
    }
}
