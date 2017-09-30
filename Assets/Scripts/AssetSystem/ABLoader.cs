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
    public Loader(AssetBundleData d) : base(d) { }
    
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
            //    //被依赖的bundle要重新load 不能直接从cache列表取 否则最终的prefab会缺少依赖的资源
            //    //为了避免内存里有多个对象 要把之前cache的卸载掉
            //    //直接Unload时,若依然有物体用该图，那么物体就变全黑 
            //    UnityEngine.Object obj = ABManager.singleton.GetCache(hash);
            //    //XResourceMgr.UnloadAsset(obj);
            //    obj = LoadFromBundle(data);
            //    ABManager.singleton.CacheObject(hash, obj);
            //    return obj;
            var asset = ABManager.singleton.GetCache(hash);
            asset.ref_cnt++;
            return asset.obt;
        }
        else
        {
            XAssetBundle bundle = ABManager.singleton.GetBundle(data);
            UnityEngine.Object obj = bundle.LoadAsset(data.loadName);
            ABManager.singleton.CacheObject(hash, obj);
            return obj;
        }
    }

    private UnityEngine.Object LoadFromBundle(AssetBundleData data)
    {
        XAssetBundle bundle = ABManager.singleton.GetBundle(data);
        return bundle.LoadAsset(data.loadName);
    }
    
}

/// <summary>
/// 异步加载
/// </summary>
public class AsyncLoader : LoaderBase
{
    Action<UnityEngine.Object> loadCB;

    int loadCnt;

    public AsyncLoader(AssetBundleData d) : base(d)
    {
        loadCnt = depsCnt;
    }

    public void LoadAsyn(Action<UnityEngine.Object> cb)
    {
        loadCB = cb;
        if (depsCnt > 0)
        {
            LoadDeps();
        }
        LoadAsset();
    }

    protected void LoadDeps()
    {
        for (int i = 0; i < depsCnt; i++)
        {
            AssetBundleData ad = ABManager.singleton.depInfoReader.GetAssetBundleInfo(data.dependencies[i]);
            AsyncLoader loader = new AsyncLoader(ad);
            loader.LoadAsyn(OnDepLoadFinish);
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
            var asset = ABManager.singleton.GetCache(hash);
            asset.ref_cnt++;
            OnComplete(hash, asset.obt);
        }
        else
        {
            IEnumerator etor = LoadFromCacheFile(hash, OnComplete);
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
        new XAssetBundle(data, bundle);
    }

}
