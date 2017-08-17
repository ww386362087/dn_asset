
using System;
using System.Collections;
using System.IO;
using UnityEngine;


public class LoaderBase
{
    protected AssetBundleData data;
    protected MonoBehaviour mono;
    protected int depsCnt = 0;

    public LoaderBase(AssetBundleData d)
    {
        data = d;
        mono = GameEnine.entrance;
        depsCnt = data.dependencies.Length;
    }
 
}


/// <summary>
/// 同步加载
/// </summary>
public class Loader : LoaderBase
{
    public Loader(AssetBundleData d) 
        : base(d)
    {
    }


    public UnityEngine.Object LoadImm()
    {
        if (depsCnt > 0)
        {
            LoadDeps();
        }
        return LoadAsset();
    }

    protected void LoadDeps()
    {
        for (int i = 0; i < depsCnt; i++)
        {
            AssetBundleData ad = ABManager.singleton.depInfoReader.GetAssetBundleInfo(data.dependencies[i]);
            Loader loader = new Loader(ad);
            loader.LoadImm();
        }
    }

    private UnityEngine.Object LoadAsset()
    {
        uint hash = data.hash;
        if (ABManager.singleton.IsCached(hash))
        {
            return ABManager.singleton.GetCache(hash);
        }
        else
        {
            UnityEngine.Object obj = Application.isEditor ? LoadFromCacheFile(hash) : LoadFromPackage(hash);
            ABManager.singleton.CacheObject(hash, obj);
            return obj;
        }
    }

    private UnityEngine.Object LoadFromCacheFile(uint bundleName)
    {

        string file = Path.Combine(AssetBundlePathResolver.BundleCacheDir, bundleName + ".ab");
        AssetBundle bundle = AssetBundle.LoadFromFile(file);
        UnityEngine.Object obj = bundle.LoadAsset(data.loadName);
       // bundle.Unload(false);
        return obj;
    }

    /// <summary>
    /// 从源文件(安装包里)加载
    /// </summary>
    private UnityEngine.Object LoadFromPackage(uint bundleName)
    {
        string file = AssetBundlePathResolver.GetBundleSourceFile(bundleName + ".ab", false);
        AssetBundle bundle = AssetBundle.LoadFromFile(file);
        UnityEngine.Object obj = bundle.LoadAsset(data.loadName);
    //    bundle.Unload(false);
        return obj;
    }
}

/// <summary>
/// 异步加载
/// </summary>
public class AsyncLoader : LoaderBase
{
    Action<UnityEngine.Object> loadCB;

    int loadCnt;

    public AsyncLoader(AssetBundleData d)
        : base(d)
    {
        loadCnt = depsCnt;
    }

    public void LoadImm(Action<UnityEngine.Object> cb)
    {
        loadCB = cb;
        if (depsCnt > 0)
        {
            LoadDeps();
        }
        else
        {
            LoadAsset();
        }
    }

    protected void LoadDeps()
    {
        for (int i = 0; i < depsCnt; i++)
        {
            AssetBundleData ad = ABManager.singleton.depInfoReader.GetAssetBundleInfo(data.dependencies[i]);
            AsyncLoader loader = new AsyncLoader(ad);
            loader.LoadImm(OnDepLoadFinish);
        }
    }

    private void OnDepLoadFinish(UnityEngine.Object obj)
    {
        loadCnt++;
    }


    private void LoadAsset()
    {
        uint hash = data.hash;
        if (ABManager.singleton.IsCached(hash))
        {
            OnComplete(hash, ABManager.singleton.GetCache(hash));
        }
        else
        {
            IEnumerator etor = Application.isEditor ? LoadFromCacheFile(hash, OnComplete) : LoadFromPackage(hash, OnComplete);
            mono.StartCoroutine(etor);
        }
    }

    private void OnComplete(uint hash, UnityEngine.Object obj)
    {
        loadCnt++;
        if (depsCnt == loadCnt)  //所有依赖加载完毕
        {
            LoadAsset();
        }
        else if (loadCnt > depsCnt)
        {
            loadCB(obj);
        }

    }


    private IEnumerator LoadFromCacheFile(uint bundleName, Action<uint, UnityEngine.Object> cb)
    {
        string file = Path.Combine(AssetBundlePathResolver.BundleCacheDir, bundleName + ".ab");
        AssetBundleCreateRequest req = AssetBundle.LoadFromFileAsync(file);
        while (!req.isDone) yield return null;
        AssetBundle bundle = req.assetBundle;
        ABManager.singleton.CacheObject(bundleName, bundle);
        cb(bundleName, bundle.LoadAsset(data.loadName));
        bundle.Unload(false);
    }

    /// <summary>
    /// 从源文件(安装包里)加载
    /// </summary>
    /// <returns></returns>
    private IEnumerator LoadFromPackage(uint bundleName, Action<uint, UnityEngine.Object> cb)
    {
        string file = AssetBundlePathResolver.GetBundleSourceFile(bundleName + ".ab", false);
        AssetBundleCreateRequest req = AssetBundle.LoadFromFileAsync(file);
        while (!req.isDone) yield return null;
        AssetBundle bundle = req.assetBundle;
        ABManager.singleton.CacheObject(bundleName, bundle);
        cb(bundleName, bundle.LoadAsset(data.loadName));
        bundle.Unload(false);
    }


}