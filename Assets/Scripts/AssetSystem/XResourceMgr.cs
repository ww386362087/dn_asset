using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// 资源加载管理
/// 同步+异步 
/// Resources+AssetBundle
/// (AB部分暂未实现)
/// </summary>
public class XResourceMgr
{

    private struct AssetNode
    {
        public string path;
        public ResourceRequest request;
        public List<System.Action<Object>> cb;
    }

    //为了效率 避免update的时候重复计算list长度
    private static int cnt = 0;

    //update里大量重复的遍历
    private static List<AssetNode> list;


    public static void Update()
    {
        if (list != null && cnt > 0)
        {
            for (int i = cnt - 1; i >= 0; i++)
            {
                if (list[i].request.isDone)
                {
                    DownloadDone(list[i]);
                    break;
                }
            }
        }
    }

    public static T Load<T>(string path) where T : Object
    {
        return Resources.Load<T>(path);
    }

    public static Object[] LoadAll(string path)
    {
        return Resources.LoadAll(path);
    }

    public static T[] LoadAll<T>(string path) where T : Object
    {
        return Resources.LoadAll<T>(path);
    }

    public static Object Load(string path)
    {
        return Resources.Load(path);
    }

    public static void UnloadAsset(Object assetToUnload)
    {
        Resources.UnloadAsset(assetToUnload);
    }

    public static void LoadAsync<T>(string path, System.Action<Object> cb) where T : Object
    {
        AddSysnLoad<T>(path, cb);
    }

    private static void AddSysnLoad<T>(string path, System.Action<Object> cb) where T : Object
    {
        if (list == null) list = new List<AssetNode>();
        for (int i = 0, max = list.Count; i < max; i++)
        {
            if (list[i].path == path)
            {
                list[i].cb.Add(cb);
                return;
            }
        }
        AssetNode node = new AssetNode();
        node.path = path;
        node.request = Resources.LoadAsync<T>(path);
        node.cb = new List<System.Action<Object>>();
        node.cb.Add(cb);
        list.Add(node);
        cnt = list.Count;
    }


    private static void DownloadDone(AssetNode node)
    {
        for (int i = 0, max = node.cb.Count; i < max; i++)
        {
            node.cb[i](node.request.asset);
        }
        node.cb.Clear();
        list.Remove(node);
        cnt--;
    }


}
