using UnityEngine;
using UnityEditor;
using System.IO;
using System.Collections.Generic;

using PrefabSet = System.Collections.Generic.HashSet<UnityEngine.GameObject>;
using MatPrefabMat = System.Collections.Generic.Dictionary<UnityEngine.Material, System.Collections.Generic.HashSet<UnityEngine.GameObject>>;

namespace XEditor
{
    public class TextureFindEditor : EditorWindow
    {
        public enum EPlatform
        {
            Android,
            iPhone
        }

        public enum EResType
        {
            Fx = 0,
            UI,
            Equip,
            Prefab,
            Sykbox,
            Scene,
        }
        public enum ETexType
        {
            Format,
            Size
        }
        private class TexInfo
        {
            public List<Texture> texList = new List<Texture>();
            public long size = 0;
        }

        private EPlatform m_Platform = EPlatform.Android;
        private ETexType m_TexType = ETexType.Format;

        private Dictionary<string, TexInfo> m_FormatMap = new Dictionary<string, TexInfo>();
        private Dictionary<string, TexInfo> m_SizeMap = new Dictionary<string, TexInfo>();
        private HashSet<string> m_TexMap = new HashSet<string>();
        private Dictionary<Texture, MatPrefabMat> m_TexMatMap = new Dictionary<Texture, MatPrefabMat>();

        private List<Texture> m_texCache = new List<Texture>();
        private long m_TotalSize = 0;
        private int m_TotalCount = 0;
        private TexInfo m_TexInfo = null;
        private Texture m_Tex = null;
        private MatPrefabMat m_TexMat = null;
        private PrefabSet m_PrefabSet = null;
        private Vector2 typeScrollPos = Vector2.zero;
        private Vector2 texScrollPos = Vector2.zero;
        private Vector2 matScrollPos = Vector2.zero;
        private Vector2 prefabScrollPos = Vector2.zero;
        //pri
        private EResType resType = EResType.Fx;
        private string[] resPaths = new string[] {
            "Assets/Resources/Effects=0=1",
            "Assets/Resources/atlas/UI|Assets/Resources/StaticUI=0=1",
            "Assets/Resources/Equipments=2=0",
            "Assets/Resources/Prefabs=1=0",
            "Assets/Resources/Skyboxes=0=0",
            "Assets/XScene=0=1"};

        private void CacheTexMat(Texture tex, Material mat, GameObject prefab)
        {
            MatPrefabMat matPrefabMap = null;
            if (!m_TexMatMap.TryGetValue(tex, out matPrefabMap))
            {
                matPrefabMap = new MatPrefabMat();
                m_TexMatMap.Add(tex, matPrefabMap);
            }
            PrefabSet prefabSet = null;
            if (!matPrefabMap.TryGetValue(mat, out prefabSet))
            {
                matPrefabMap.Add(mat, null);
            }
            if (prefab != null)
            {
                if (prefabSet == null)
                {
                    prefabSet = new PrefabSet();
                    matPrefabMap[mat] = prefabSet;
                }
                if (!prefabSet.Contains(prefab))
                    prefabSet.Add(prefab);
            }
        }
        private void FindMat(string path)
        {
            DirectoryInfo di = new DirectoryInfo(path);
            FileInfo[] files = di.GetFiles("*.mat", SearchOption.AllDirectories);

            for (int i = 0; i < files.Length; ++i)
            {
                FileInfo fi = files[i];
                string matPath = fi.FullName.Replace("\\", "/");
                int index = matPath.IndexOf(path);
                matPath = matPath.Substring(index);

                EditorUtility.DisplayProgressBar(string.Format("{0}-{1}/{2}", matPath, i, files.Length), path, (float)i / files.Length);
                m_texCache.Clear();
                Material mat = AssetDatabase.LoadAssetAtPath(matPath, typeof(Material)) as Material;
                if (mat != null)
                {
                    MaterialEditor.GetMatTex(mat, m_texCache);
                    for (int j = 0; j < m_texCache.Count; ++j)
                    {
                        Texture tex = m_texCache[j];
                        CacheTexMat(tex, mat, null);
                    }
                }
                m_texCache.Clear();
            }
            EditorUtility.ClearProgressBar();
        }
        private void ScanPrefab(string path)
        {
            DirectoryInfo di = new DirectoryInfo(path);
            FileInfo[] files = di.GetFiles("*.prefab", SearchOption.AllDirectories);
            string[] fileName = new string[1];
            for (int i = 0; i < files.Length; ++i)
            {
                FileInfo fi = files[i];
                string prefabPath = fi.FullName.Replace("\\", "/");
                int index = prefabPath.IndexOf(path);
                prefabPath = prefabPath.Substring(index);
                EditorUtility.DisplayProgressBar(string.Format("{0}-{1}/{2}", prefabPath, i, files.Length), path, (float)i / files.Length);

                GameObject prefab = AssetDatabase.LoadAssetAtPath(prefabPath, typeof(GameObject)) as GameObject;
                GameObject instance = GameObject.Instantiate(prefab) as GameObject;

                fileName[0] = prefabPath;
                string[] reses = AssetDatabase.GetDependencies(fileName);
                for (int j = 0; j < reses.Length; ++j)
                {
                    string res = reses[j];
                    string resLow = res.ToLower();
                    if (resLow.EndsWith(".mat"))
                    {
                        m_texCache.Clear();
                        Material mat = AssetDatabase.LoadAssetAtPath(res, typeof(Material)) as Material;
                        MaterialEditor.GetMatTex(mat, m_texCache);
                        for (int k = 0; k < m_texCache.Count; ++k)
                        {
                            Texture t = m_texCache[k];
                            TryScanTex(t);
                            CacheTexMat(t, mat, prefab);
                        }
                        m_texCache.Clear();
                    }
                }
                GameObject.DestroyImmediate(instance);
            }
            EditorUtility.ClearProgressBar();
        }

        private void TryScanTex(Texture tex)
        {
            if (tex != null)
            {
                string texPath = AssetDatabase.GetAssetPath(tex);
                if (m_TexMap.Contains(texPath))
                {
                    return;
                }
                m_TexMap.Add(texPath);

                InnerScanTex(tex, texPath);
            }
        }
        private void InnerScanTex(Texture tex, string texPath)
        {
            AssetImporter assetImport = AssetImporter.GetAtPath(texPath);
            TextureImporter texImport = assetImport as TextureImporter;
            if (texImport == null)
            {
                 XDebug.LogError(texPath);
                return;
            }
            int texSize = 0;
            TextureImporterFormat format;
            texImport.GetPlatformTextureSettings(m_Platform.ToString(), out texSize, out format);
            long sizePer32 = 1;
            switch (format)
            {
                case TextureImporterFormat.ETC_RGB4:
                    sizePer32 = 1;
                    break;
                case TextureImporterFormat.RGB24:
                    sizePer32 = 6;
                    break;
                case TextureImporterFormat.RGBA16:
                    sizePer32 = 4;
                    break;
                case TextureImporterFormat.RGBA32:
                    sizePer32 = 8;
                    break;
                case TextureImporterFormat.Alpha8:
                    sizePer32 = 2;
                    break;
            }
            long size = tex.width * tex.height * sizePer32 / 2;
            string sizeStr = string.Format("{0}x{0}", tex.width, tex.height);
            string formatStr = format.ToString();
            TexInfo ti = null;
            if (!m_FormatMap.TryGetValue(formatStr, out ti))
            {
                ti = new TexInfo();
                m_FormatMap.Add(formatStr, ti);
            }
            ti.texList.Add(tex);
            ti.size += size;

            ti = null;
            if (!m_SizeMap.TryGetValue(sizeStr, out ti))
            {
                ti = new TexInfo();
                m_SizeMap.Add(sizeStr, ti);
            }
            ti.texList.Add(tex);
            ti.size += size;
            m_TotalSize += size;
            m_TotalCount++;
        }
        private void InnerScanTex(string texPath)
        {
            if (m_TexMap.Contains(texPath))
            {
                return;
            }
            m_TexMap.Add(texPath);

            Texture tex = AssetDatabase.LoadAssetAtPath(texPath, typeof(Texture)) as Texture;
            InnerScanTex(tex, texPath);
        }

        private void ScanTex(string path, string ext)
        {
            DirectoryInfo di = new DirectoryInfo(path);
            FileInfo[] files = di.GetFiles(ext, SearchOption.AllDirectories);
            for (int i = 0; i < files.Length; ++i)
            {
                FileInfo fi = files[i];
                string texPath = fi.FullName.Replace("\\", "/");
                int index = texPath.IndexOf(path);
                texPath = texPath.Substring(index);

                EditorUtility.DisplayProgressBar(string.Format("{0}-{1}/{2}", texPath, i, files.Length), path, (float)i / files.Length);
                InnerScanTex(texPath);
            }
            EditorUtility.ClearProgressBar();
        }

        private void FindTexs(string pathInfo)
        {
            m_FormatMap.Clear();
            m_SizeMap.Clear();
            m_TexMap.Clear();
            m_TexMatMap.Clear();
            m_TexInfo = null;
            m_TexMat = null;
            m_Tex = null;
            m_TotalSize = 0;
            m_TotalCount = 0;
            EditorUtility.UnloadUnusedAssetsImmediate();
            string[] parsePath = pathInfo.Split('=');
            string paths = parsePath[0];
            string findType = parsePath[1];
            string scanMat = parsePath[2];
            string[] pathList = paths.Split('|');

            if (scanMat == "1")
            {
                for (int i = 0; i < pathList.Length; ++i)
                {
                    string subPath = pathList[i];
                    FindMat(subPath);
                }
            }
            bool findTex = false;
            bool findDependencies = false;
            if (findType == "0")
            {
                findTex = true;
            }
            else if (findType == "1")
            {
                findDependencies = true;
            }
            else if (findType == "2")
            {
                findTex = true;
                findDependencies = true;
            }
            for (int i = 0; i < pathList.Length; ++i)
            {
                string subPath = pathList[i];
                if (findTex)
                {
                    ScanTex(subPath, "*.png");
                    ScanTex(subPath, "*.tga");
                }

                if (findDependencies)
                {
                    ScanPrefab(subPath);
                }
            }
            m_TexMap.Clear();
        }

        protected virtual void OnGUI()
        {
            GUILayout.BeginHorizontal();
            resType = (EResType)EditorGUILayout.EnumPopup("资源目录", resType, GUILayout.MaxWidth(250));
            m_Platform = (EPlatform)EditorGUILayout.EnumPopup("平台", m_Platform, GUILayout.MaxWidth(250));
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();
            ETexType lastTexType = m_TexType;
            m_TexType = (ETexType)EditorGUILayout.EnumPopup("统计类型", m_TexType, GUILayout.MaxWidth(250));
            if (lastTexType != m_TexType)
            {
                m_TexInfo = null;
                m_TexMat = null;
                m_Tex = null;
                m_PrefabSet = null;
            }
            EditorGUILayout.LabelField(string.Format("Total Count:{0} TotalSize:{1}KB-{2}MB ", m_TotalCount, m_TotalSize / 1024, m_TotalSize / 1024 / 1024));
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Scan", GUILayout.MaxWidth(150)))
            {
                string resPath = resPaths[(int)resType];
                FindTexs(resPath);
            }
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();

            GUILayout.BeginVertical();
            typeScrollPos = GUILayout.BeginScrollView(typeScrollPos, false, false, GUILayout.MinHeight(400));
            Dictionary<string, TexInfo> texMap = m_FormatMap;
            if (m_TexType == ETexType.Size)
            {
                texMap = m_SizeMap;
            }

            Dictionary<string, TexInfo>.Enumerator it = texMap.GetEnumerator();
            while (it.MoveNext())
            {
                GUILayout.BeginHorizontal();
                string name = it.Current.Key;
                TexInfo ti = it.Current.Value;
                EditorGUILayout.LabelField(string.Format("{0} Count:{1} Size:{2}KB-{3}MB ", name, ti.texList.Count, ti.size / 1024, ti.size / 1024 / 1024), GUILayout.MaxWidth(400));
                if (GUILayout.Button("Select", GUILayout.MaxWidth(50)))
                {
                    m_TexInfo = ti;
                    m_TexMat = null;
                    m_Tex = null;
                    m_PrefabSet = null;
                }
                GUILayout.EndHorizontal();
            }
            EditorGUILayout.EndScrollView();

            GUILayout.EndVertical();

            GUILayout.BeginVertical();
            texScrollPos = GUILayout.BeginScrollView(texScrollPos, false, false, GUILayout.MinHeight(400));
            if (m_TexInfo != null)
            {
                for (int i = 0; i < m_TexInfo.texList.Count; ++i)
                {
                    GUILayout.BeginHorizontal();
                    Texture tex = m_TexInfo.texList[i];
                    if (m_TexMat != null && m_Tex == tex)
                    {
                        EditorGUILayout.ObjectField(string.Format("{0} mat:{1}", tex.name, m_TexMat.Count), tex, typeof(Texture), true, GUILayout.MaxWidth(450));
                    }
                    else
                    {
                        EditorGUILayout.ObjectField(tex.name, tex, typeof(Texture), true, GUILayout.MaxWidth(450));
                    }

                    if (GUILayout.Button("Mat", GUILayout.MaxWidth(50)))
                    {
                        m_Tex = tex;
                        m_TexMatMap.TryGetValue(m_Tex, out m_TexMat);
                        m_PrefabSet = null;
                    }
                    GUILayout.EndHorizontal();
                }
            }

            EditorGUILayout.EndScrollView();
            GUILayout.EndVertical();
            GUILayout.EndHorizontal();
            GUILayout.Space(10);
            GUILayout.BeginHorizontal();
            GUILayout.BeginVertical();
            matScrollPos = GUILayout.BeginScrollView(matScrollPos, false, false, GUILayout.MinHeight(400));
            if (m_TexMat != null)
            {
                MatPrefabMat.Enumerator texMatIt = m_TexMat.GetEnumerator();
                while (texMatIt.MoveNext())
                {
                    GUILayout.BeginHorizontal();
                    Material mat = texMatIt.Current.Key;
                    EditorGUILayout.ObjectField(mat.name, mat, typeof(Material), true, GUILayout.MaxWidth(450));

                    GUILayout.EndHorizontal();
                }
            }
            EditorGUILayout.EndScrollView();
            GUILayout.EndVertical();

            GUILayout.BeginVertical();
            prefabScrollPos = GUILayout.BeginScrollView(prefabScrollPos, false, false, GUILayout.MinHeight(500));
            if (m_PrefabSet != null)
            {
                PrefabSet.Enumerator prefabIt = m_PrefabSet.GetEnumerator();
                while (prefabIt.MoveNext())
                {
                    GUILayout.BeginHorizontal();
                    GameObject prefab = prefabIt.Current;
                    EditorGUILayout.ObjectField(prefab.name, prefab, typeof(GameObject), true, GUILayout.MaxWidth(550));
                    GUILayout.EndHorizontal();
                }
            }
            EditorGUILayout.EndScrollView();

            GUILayout.EndVertical();
            GUILayout.EndHorizontal();
        }
    }
}
