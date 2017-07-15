using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// 资源加载管理
/// 同步+异步 
/// Resources+AssetBundle
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

    //记录resource 里的资源列表
    private static List<AssetNode> list;

    private static bool useAB = true;

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

    public static T Load<T>(string path, AssetType type) where T : Object
    {
        if (useAB && ABManager.singleton.Exist(path, type))
        {
            Object o = ABManager.singleton.LoadImm(path, type);
            if (type == AssetType.Prefab)
            {
                if (typeof(T) == typeof(GameObject) || typeof(T) == typeof(Transform))
                    return o as T;
                else
                    return (o as GameObject).GetComponent<T>();
            }
            else
                return o as T;
        }
        else
        {
            return Resources.Load<T>(path);
        }
    }

    public static Object[] LoadAll(string path, AssetType type)
    {
        return Resources.LoadAll(path);
    }

    public static T[] LoadAll<T>(string path) where T : Object
    {
        return Resources.LoadAll<T>(path);
    }

    public static Object Load(string path, AssetType type)
    {
        if (useAB && ABManager.singleton.Exist(path, type))
        {
            return ABManager.singleton.LoadImm(path, type);
        }
        else
        {
            return Resources.Load(path);
        }
    }

    public static void UnloadAsset(Object assetToUnload)
    {
        Resources.UnloadAsset(assetToUnload);
    }

    public static void LoadAsync<T>(string path, AssetType type, System.Action<Object> cb) where T : Object
    {
        AddSysnLoad<T>(path, type, cb);
    }

    public static void CancelLoad(System.Action<Object> cb)
    {
        if (list == null || cnt <= 0) return;
        for (int i = cnt - 1; i >= 0; i--)
        {
            if (list[i].cb.Contains(cb))
                list[i].cb.Remove(cb);

            if (list[i].cb.Count <= 0)
            {
                list.RemoveAt(i);
                break;
            }
        }
    }

    private static void AddSysnLoad<T>(string path, AssetType type, System.Action<Object> cb) where T : Object
    {
        if (useAB && ABManager.singleton.Exist(path, type))
        {
            ABManager.singleton.LoadImm(path, type, cb);
        }
        else
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
