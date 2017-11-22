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
    
    public static string DependFileName { get { return "Table/dep"; } }

    static string cacheDir;

    public static string BundleCacheDir
    {
        get
        {
            if (cacheDir == null)
            {
                bool is_test = false;
#if TEST
                is_test = true;
#endif
                switch (Application.platform)
                {
                    case RuntimePlatform.Android:
                        cacheDir = string.Format("{0}/update/Android/AssetBundles", is_test ? Application.streamingAssetsPath : Application.persistentDataPath);
                        break;
                    case RuntimePlatform.IPhonePlayer:
                        cacheDir = string.Format("{0}/update/iOS/AssetBundles", is_test ? Application.streamingAssetsPath : Application.persistentDataPath);
                        break;
                    default:
                        cacheDir = string.Format("{0}/update/AssetBundles", Application.streamingAssetsPath);
                        break;
                }
            }
            return cacheDir;
        }
    }

}
