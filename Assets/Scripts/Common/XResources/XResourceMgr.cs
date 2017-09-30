using System.Collections.Generic;
using UnityEngine;

public class Asset
{
    public Object obt;
    public int ref_cnt;
}


/// <summary>
/// 异步资源
/// </summary>
public struct AsynAsset
{
    public string path;
    public AssetType type;
    public ResourceRequest request;
    public List<System.Action<Object>> cb;
}


public class XResourceMgr : XSingleton<XResourceMgr>
{

    private Dictionary<uint, Asset> map = new Dictionary<uint, Asset>();

    //异步资源列表
    private static List<AsynAsset> asyn_list = new List<AsynAsset>();

    //为了效率 避免update的时候重复计算list长度
    private static int asyn_loading_cnt = 0;

    public void Update()
    {
        if (asyn_loading_cnt > 0)
        {
            for (int i = asyn_loading_cnt - 1; i >= 0; i++)
            {
                if (asyn_list[i].request.isDone)
                {
                    DownloadDone(asyn_list[i]);
                    break;
                }
            }
        }
    }


    public Object Load<T>(string path, AssetType type) where T : Object
    {
        uint hash = XCommon.singleton.XHash(path + type);
        if (map.ContainsKey(hash))
        {
          //  XDebug.Log("contain:" + path, " type: " + type);
            map[hash].ref_cnt++;
            return map[hash].obt;
        }
        else
        {
            T obt = Resources.Load<T>(path);
            Asset asset = new Asset { obt = obt, ref_cnt = 1 };
            map.Add(hash, asset);
            return obt;
        }
    }

    public void AsynLoad(string path, AssetType type, System.Action<Object> cb)
    {
        uint hash = XCommon.singleton.XHash(path + type);
        AsynAsset asset;
        if (map.ContainsKey(hash)) //已经加载的 计数器加一 
        {
            map[hash].ref_cnt++;
            cb(map[hash].obt);
        }
        else if (IsAsynLoading(path, out asset)) //已正在加载的 回调cache
        {
            asset.cb.Add(cb);
        }
        else//没有加载的 开始加载
        {
            AsynAsset node = new AsynAsset();
            node.path = path;
            node.type = type;
            node.request = Resources.LoadAsync(path);
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


    public void Unload(string path, AssetType type)
    {
        uint hash = XCommon.singleton.XHash(path + type);
        if (map.ContainsKey(hash))
        {
            map[hash].ref_cnt--;
            if (map[hash].ref_cnt <= 0)
            {
                XResources.UnloadAsset(map[hash].obt);
                map[hash].obt = null;
                map.Remove(hash);
            }
        }
    }


    private void DownloadDone(AsynAsset node)
    {
        Asset asset = new Asset { obt = node.request.asset, ref_cnt = node.cb.Count };
        uint hash = XCommon.singleton.XHash(node.path + node.type);
        map.Add(hash, asset);
        for (int i = 0, max = node.cb.Count; i < max; i++)
        {
            node.cb[i](node.request.asset);
        }
        node.cb.Clear();
        asyn_list.Remove(node);
        asyn_loading_cnt--;
    }

}

