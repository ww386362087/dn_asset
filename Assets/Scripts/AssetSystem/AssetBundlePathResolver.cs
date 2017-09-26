using System.IO;
using UnityEngine;

public class AssetBundlePathResolver
{

    /// <summary>
    /// AB 保存的路径相对于 Assets/StreamingAssets 的名字
    /// </summary>
    public static string BundleSaveDirName { get { return "AssetBundles"; } }

    /// <summary>
    /// AB 保存的路径
    /// </summary>
    public static string AndroidBundleSavePath { get { return "Assets/StreamingAssets/update/Android/" + BundleSaveDirName; } }
    public static string iOSBundleSavePath { get { return "Assets/StreamingAssets/update/iOS/" + BundleSaveDirName; } }
    public static string DefaultBundleSavePath { get { return "Assets/StreamingAssets/update/" + BundleSaveDirName; } }
    /// <summary>
    /// AB打包的原文件HashCode要保存到的路径，下次可供增量打包
    /// </summary>
    public static string AndroidHashCacheSaveFile { get { return "Assets/Editor/Cache/Android/cache.txt"; } }
    public static string iOSHashCacheSaveFile { get { return "Assets/Editor/Cache/iOS/cache.txt"; } }
    public static string DefaultHashCacheSaveFile { get { return "Assets/Editor/Cache/cache.txt"; } }

    /// <summary>
    /// /// 在编辑器模型下将 abName 转为 Assets/... 路径
    /// 这样就可以不用打包直接用了
    /// </summary>
    public static string GetEditorModePath(string abName)
    {
        //将 Assets.AA.BB.prefab 转为 Assets/AA/BB.prefab
        abName = abName.Replace(".", "/");
        int last = abName.LastIndexOf("/");
        if (last == -1) return abName;
        return string.Format("{0}.{1}", abName.Substring(0, last), abName.Substring(last + 1));
    }

    /// <summary>
    /// 获取 AB 源文件路径（打包进安装包的）
    /// </summary>
    public static string GetBundleSourceFile(string path, bool forWWW = true)
    {
        string filePath = null;
        switch (Application.platform)
        {
            case RuntimePlatform.Android:
                if (forWWW)
                    filePath = string.Format("jar:file://{0}!/assets/update/Android/{1}/{2}", Application.dataPath, BundleSaveDirName, path);
                else
                    filePath = string.Format("{0}!assets/update/Android/{1}/{2}", Application.dataPath, BundleSaveDirName, path);
                break;
            case RuntimePlatform.IPhonePlayer:
                if (forWWW)
                    filePath = string.Format("file://{0}/Raw/update/iOS/{1}/{2}", Application.dataPath, BundleSaveDirName, path);
                else
                    filePath = string.Format("{0}/Raw/update/iOS/{1}/{2}", Application.dataPath, BundleSaveDirName, path);
                break;
            default:
                if (forWWW)
                    filePath = string.Format("file://{0}/StreamingAssets/update/{1}/{2}", Application.dataPath, BundleSaveDirName, path);
                else
                    filePath = string.Format("{0}/StreamingAssets/update/{1}/{2}", Application.dataPath, BundleSaveDirName, path);
                break;
        }
        return filePath;
    }

    /// <summary>
    /// AB 依赖信息文件名
    /// </summary>
    public static string DependFileName { get { return "dep.all"; } }

    static DirectoryInfo cacheDir;

    /// <summary>
    /// 用于缓存AB的目录，要求可写
    /// </summary>
    public static string BundleCacheDir
    {
        get
        {
            if (cacheDir == null)
            {
                string dir;
                bool is_test = false;
#if TEST
                is_test = true;
#endif
                switch (Application.platform)
                {
                    case RuntimePlatform.Android:
                        dir = string.Format("{0}/update/AssetBundles", is_test ? Application.streamingAssetsPath : Application.persistentDataPath);
                        break;
                    case RuntimePlatform.IPhonePlayer:
                        dir = string.Format("{0}/update/AssetBundles", is_test ? Application.streamingAssetsPath : Application.persistentDataPath);
                        break;
                    default:
                        dir = string.Format("{0}/update/AssetBundles", Application.streamingAssetsPath);
                        break;
                }
                cacheDir = new DirectoryInfo(dir);
                if (!cacheDir.Exists) cacheDir.Create();
            }
            return cacheDir.FullName;
        }
    }

}
