using System.Collections.Generic;
using UnityEngine;

public class Asset
{
    public Object obt;
    public int ref_cnt;
    public bool is_clone_asset;
}


public struct AsynAsset
{
    /// <summary>
    /// resources路径
    /// </summary>
    public string path;
    /// <summary>
    /// 目前就是Gameobject or Transform两种类型
    /// </summary>
    public bool is_clone_asset;
    /// <summary>
    /// 资源格式 后缀名
    /// </summary>
    public AssetType type;

    public ResourceRequest request;
    /// <summary>
    /// 回调列表
    /// </summary>
    public List<System.Action<Object>> cb;
}


public class XResController 
{
    private Dictionary<uint, Asset> map = new Dictionary<uint, Asset>();

    //正在加载中的异步资源列表
    private List<AsynAsset> asyn_list = new List<AsynAsset>();

    //为了效率 避免update的时候重复计算list长度
    private int asyn_loading_cnt = 0;
    

    public void Update()
    {
        if (asyn_loading_cnt > 0)
        {
            for (int i = asyn_loading_cnt - 1; i >= 0; i--)
            {
                if (asyn_list[i].request.isDone)
                {
                    OnLoaded(asyn_list[i]);
                    break;
                }
            }
        }
    }


    public Object Load<T>(string path, AssetType type,out uint hash) where T : Object
    {
        hash = XCommon.singleton.XHash(path + type);
        if (map.ContainsKey(hash))
        {
            map[hash].ref_cnt++;
            return map[hash].obt;
        }
        else
        {
            T obt = Resources.Load<T>(path);
            bool isClone = XResources.IsCloneAsset<T>();
            Asset asset = new Asset { obt = obt, ref_cnt = 1, is_clone_asset = isClone };
            map.Add(hash, asset);
            return obt;
        }
    }

    public void AsynLoad<T>(string path, AssetType type, System.Action<Object> cb) where T : Object
    {
        uint hash = XCommon.singleton.XHash(path + type);
        AsynAsset asset;
        if (map.ContainsKey(hash) && map[hash].obt != null) //已经加载的 计数器加一 
        {
            map[hash].ref_cnt++;
            if (map[hash].is_clone_asset)
            {
                GameObject go = GameObject.Instantiate(map[hash].obt) as GameObject;
                XResources.SetAsynAssetIndex(go.GetInstanceID(), hash);
                cb(go);
            }
            else
            {
                cb(map[hash].obt);
                XResources.SetAsynAssetIndex(map[hash].obt.GetInstanceID(), hash);
            }
        }
        else if (IsAsynLoading(path, out asset)) //已正在加载的 回调cache
        {
            asset.cb.Add(cb);
        }
        else //没有加载的 开始加载
        {
            AsynAsset node = new AsynAsset();
            node.path = path;
            node.type = type;
            node.is_clone_asset = XResources.IsCloneAsset<T>();
            node.request = Resources.LoadAsync<T>(path);
            node.cb = new List<System.Action<Object>>();
            node.cb.Add(cb);
            asyn_list.Add(node);
            asyn_loading_cnt = asyn_list.Count;
        }
    }


    public bool IsAsynLoading(string path, out AsynAsset asset)
    {
        for (int i = 0, max = asyn_list.Count; i < max; i++)
        {
            if (asyn_list[i].path == path)
            {
                asset = asyn_list[i];
                return true;
            }
        }
        asset = default(AsynAsset);
        return false;
    }


    public bool Unload(uint hash)
    {
        if (map != null && map.ContainsKey(hash))
        {
            map[hash].ref_cnt--;
            if (map[hash].ref_cnt <= 0)
            {
                XResources.UnloadAsset(map[hash]);
                map[hash].obt = null;
                map.Remove(hash);
                return true;
            }
        }
        return false;
    }

    /// <summary>
    /// 一般是切换场景的时候调用 如：OnLeaveScene
    /// </summary>
    public void UnloadAll()
    {
        var e = map.GetEnumerator();
        while (e.MoveNext())
        {
            XResources.UnloadAsset(e.Current.Value);
        }
        e.Dispose();
        map.Clear();
    }

    private void OnLoaded(AsynAsset node)
    {
        Asset asset = new Asset { obt = node.request.asset, ref_cnt = node.cb.Count, is_clone_asset = node.is_clone_asset };
        uint hash = XCommon.singleton.XHash(node.path + node.type);
        map.Add(hash, asset);
        for (int i = 0, max = node.cb.Count; i < max; i++)
        {
            if (node.is_clone_asset)
            {
                GameObject go = GameObject.Instantiate(node.request.asset) as GameObject;
                node.cb[i](go);
            }
            else
            {
                node.cb[i](node.request.asset);
            }
        }
        node.cb.Clear();
        asyn_list.Remove(node);
        asyn_loading_cnt--;
    }
    
}

