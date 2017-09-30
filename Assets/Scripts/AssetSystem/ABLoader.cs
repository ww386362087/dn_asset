using System;
using System.Collections;
using System.IO;
using UnityEngine;

public class LoaderBase
{
    protected AssetBundleData data;
    protected MonoBehaviour mono;
    protected int depsCnt = 0;
    /// <summary>
    /// 依赖的资源都是false
    /// 根只有是GameObject的时候为true
    /// </summary>
    protected bool isCloneAsset = false;

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
    
    public UnityEngine.Object Load<T>()
    {
        isCloneAsset = XResources.IsCloneAsset<T>();
        return InnerLoad();
    }

    private UnityEngine.Object InnerLoad()
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
            loader.InnerLoad();
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
            ABManager.singleton.CacheObject(hash, obj,isCloneAsset);
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

    public AsyncLoader(AssetBundleData d) : base(d) { }

    public void LoadAsyn<T>(Action<UnityEngine.Object> cb)
    {
        isCloneAsset = XResources.IsCloneAsset<T>();
        loadCB = cb;
        InnerLoad();
    }

    private void InnerLoad()
    {
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
            loader.InnerLoad();
        }
    }

    private void LoadAsset()
    {
        uint hash = data.hash;
        if (ABManager.singleton.IsCached(hash))
        {
            var asset = ABManager.singleton.GetCache(hash);
            asset.ref_cnt++;
            OnComplete(hash, asset.obt, asset.is_clone_asset);
        }
        else
        {
            IEnumerator etor = LoadFromBundle(hash, OnComplete);
            mono.StartCoroutine(etor);
        }
    }

    private void OnComplete(uint hash, UnityEngine.Object obj, bool isClone)
    {
        if (loadCB != null)
        {
            if (isClone)
            {
                GameObject go = GameObject.Instantiate(obj) as GameObject;
                loadCB(obj);
            }
            else
            {
                loadCB(obj);
            }
        }
    }

    private IEnumerator LoadFromBundle(uint bundleName, Action<uint, UnityEngine.Object, bool> cb)
    {
        string file = Path.Combine(AssetBundlePathResolver.BundleCacheDir, bundleName + ".ab");
        AssetBundleCreateRequest req = AssetBundle.LoadFromFileAsync(file);
        while (!req.isDone) yield return null;
        AssetBundle bundle = req.assetBundle;
        ABManager.singleton.CacheObject(bundleName, bundle, isCloneAsset);
        cb(bundleName, bundle.LoadAsset(data.loadName), isCloneAsset);
        new XAssetBundle(data, bundle);
    }

}
