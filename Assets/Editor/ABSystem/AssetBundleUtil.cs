using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;


namespace ABSystem
{
    class AssetBundleUtil 
    {

        public static AssetBundlePathResolver pathResolver;
        public static DirectoryInfo AssetDir = new DirectoryInfo(Application.dataPath);
        public static string AssetPath = AssetDir.FullName;
        public static DirectoryInfo ProjectDir = AssetDir.Parent;
        public static string ProjectPath = ProjectDir.FullName;

        static Dictionary<int, AssetTarget> _object2target;
        static Dictionary<string, AssetTarget> _assetPath2target;
        static Dictionary<string, string> _fileHashCache;
        static Dictionary<string, AssetCacheInfo> _fileHashOld;

        public static void Init()
        {
            _object2target = new Dictionary<int, AssetTarget>();
            _assetPath2target = new Dictionary<string, AssetTarget>();
            _fileHashCache = new Dictionary<string, string>();
            _fileHashOld = new Dictionary<string, AssetCacheInfo>();
            LoadCache();
        }

        public static void ClearCache()
        {
            _object2target = null;
            _assetPath2target = null;
            _fileHashCache = null;
            _fileHashOld = null;
        }

        public static string GetBundleDir()
        {
            string cacheTxtFilePath;
            switch (EditorUserBuildSettings.activeBuildTarget)
            {
                case BuildTarget.Android:
                    cacheTxtFilePath = AssetBundlePathResolver.AndroidBundleSavePath;
                    break;
                case BuildTarget.iOS:
                    cacheTxtFilePath = AssetBundlePathResolver.iOSBundleSavePath;
                    break;
                default:
                    cacheTxtFilePath = AssetBundlePathResolver.DefaultBundleSavePath;
                    break;
            }
            return cacheTxtFilePath;
        }

        public static string GetCacheFile()
        {
            string cacheTxtFilePath;
            switch (EditorUserBuildSettings.activeBuildTarget)
            {
                case BuildTarget.Android:
                    cacheTxtFilePath = AssetBundlePathResolver.AndroidHashCacheSaveFile;
                    break;
                case BuildTarget.iOS:
                    cacheTxtFilePath = AssetBundlePathResolver.iOSHashCacheSaveFile;
                    break;
                default:
                    cacheTxtFilePath = AssetBundlePathResolver.DefaultHashCacheSaveFile;
                    break;
            }
            return cacheTxtFilePath;
        }

        public static void LoadCache()
        {
            string cacheTxtFilePath = GetCacheFile();

            if (File.Exists(cacheTxtFilePath))
            {
                string value = File.ReadAllText(cacheTxtFilePath);
                StringReader sr = new StringReader(value);

                //读取缓存的信息
                while (true)
                {
                    string path = sr.ReadLine();
                    if (path == null)
                        break;

                    AssetCacheInfo cache = new AssetCacheInfo();
                    cache.fileHash = sr.ReadLine();
                    cache.metaHash = sr.ReadLine();
                    cache.bundleCrc = sr.ReadLine();
                    int depsCount = Convert.ToInt32(sr.ReadLine());
                    cache.depNames = new string[depsCount];
                    for (int i = 0; i < depsCount; i++)
                    {
                        cache.depNames[i] = sr.ReadLine();
                    }
                    _fileHashOld[path] = cache;
                }
            }
        }

        public static void SaveCache()
        {
            Dictionary<string, string> hash = new Dictionary<string, string>();
            string assetPath = "";
            foreach (AssetTarget target in _object2target.Values)
            {
                if (hash.TryGetValue(target.bundleName, out assetPath))
                {
                    EditorUtility.DisplayDialog("Hash Duplicate", assetPath + " & " + target.assetPath, "OK");
                    return;
                }
                else
                {
                    hash.Add(target.bundleName, target.assetPath);
                }
            }

            StreamWriter sw = new StreamWriter(GetCacheFile());
            
            foreach (AssetTarget target in _object2target.Values)
            {
                target.WriteCache(sw);
            }

            sw.Flush();
            sw.Close();
        }

        public static List<AssetTarget> GetAll()
        {
            return new List<AssetTarget>(_object2target.Values);
        }


        public static AssetTarget Load(FileInfo file, System.Type t)
        {
            AssetTarget target = null;
            string fullPath = file.FullName;
            int index = fullPath.IndexOf("Assets");
            if (index != -1)
            {
                string assetPath = fullPath.Substring(index);
                if (_assetPath2target.ContainsKey(assetPath))
                {
                    target = _assetPath2target[assetPath];
                }
                else
                {
                    UnityEngine.Object o = null;
                    if (t == null)
                        o = AssetDatabase.LoadMainAssetAtPath(assetPath);
                    else
                        o = AssetDatabase.LoadAssetAtPath(assetPath, t);

                    if (o != null)
                    {
                        int instanceId = o.GetInstanceID();

                        if (_object2target.ContainsKey(instanceId))
                        {
                            target = _object2target[instanceId];
                        }
                        else
                        {
                            target = new AssetTarget(o, file, assetPath);
                            string key = string.Format("{0}/{1}", assetPath, instanceId);
                            _assetPath2target[key] = target;
                            _object2target[instanceId] = target;
                        }
                    }
                }
            }
            return target;
        }

        public static AssetTarget Load(FileInfo file)
        {
            return Load(file, null);
        }

        public static string ConvertToABName(string assetPath)
        {
            string bn = assetPath
                .Replace(AssetPath, "")
                .Replace('\\', '.')
                .Replace('/', '.')
                .Replace(" ", "_")
                .ToLower();
            return bn;
        }

        public static string GetFileHash(string path, bool force = false)
        {
            string _hexStr = null;
            if (_fileHashCache.ContainsKey(path) && !force)
            {
                _hexStr = _fileHashCache[path];
            }
            else if (File.Exists(path) == false)
            {
                _hexStr = "FileNotExists";
            }
            else
            {
                FileStream fs = new FileStream(path,
                    FileMode.Open,
                    FileAccess.Read,
                    FileShare.Read);

                _hexStr = HashUtil.Get(fs);
                _fileHashCache[path] = _hexStr;
                fs.Close();
            }

            return _hexStr;
        }

        public static AssetCacheInfo GetCacheInfo(string path)
        {
            if (_fileHashOld.ContainsKey(path))
                return _fileHashOld[path];
            return null;
        }
    }
}