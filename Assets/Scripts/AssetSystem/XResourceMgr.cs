using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// 资源加载管理
/// 同步+异步 
/// Resources+AssetBundle
/// </summary>
public class XResourceMgr
{
    /// <summary>
    /// 异步资源
    /// </summary>
    private struct AsynAsset
    {
        public string path;
        public ResourceRequest request;
        public List<System.Action<Object>> cb;
    }

    /// <summary>
    /// 同步资源
    /// </summary>
    private struct Asset
    {
        public string path;
        public AssetType type;
        public Object obj;
    }

    //为了效率 避免update的时候重复计算list长度
    private static int _cnt = 0;

    //记录resource 里的异步资源列表
    private static List<AsynAsset> _asyn_list;


    private static List<Asset> _syn_list;

    private static bool useAB = true;

    public static void Update()
    {
        if (_asyn_list != null && _cnt > 0)
        {
            for (int i = _cnt - 1; i >= 0; i++)
            {
                if (_asyn_list[i].request.isDone)
                {
                    DownloadDone(_asyn_list[i]);
                    break;
                }
            }
        }
    }

    public static T Load<T>(string path, AssetType type) where T : Object
    {
        if (_syn_list == null) _syn_list = new List<Asset>();
        object obj = FindInSynPool(path, type);
        if (obj != null)
        {
            return Obj2T<T>(obj);
        }
        else
        {
            if (useAB && ABManager.singleton.Exist(path, type))
            {
                Object o = ABManager.singleton.LoadImm(path, type);
                _syn_list.Add(new Asset() { path = path, type = type, obj = o });
                return Obj2T<T>(o);
            }
            else
            {
                Object o = Resources.Load<T>(path);
                _syn_list.Add(new Asset() { path = path, type = type, obj = o });
                return o as T;
            }
        }
    }



    private static Object FindInSynPool(string path, AssetType type)
    {
        if (_syn_list != null)
        {
            var e = _syn_list.GetEnumerator();
            while (e.MoveNext())
            {
                if (e.Current.path == path && e.Current.type == type)
                {
                    return e.Current.obj;
                }
            }
        }
        return null;
    }


    private static T Obj2T<T>(object o) where T : Object
    {
        if (typeof(T) == typeof(GameObject)
            || typeof(T) == typeof(Transform))
            return o as T;
        else
            return (o as GameObject).GetComponent<T>();
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

    public static void UnloadAsset(string path,AssetType type)
    {
        Object o = FindInSynPool(path, type);
        UnloadAsset(o);
    }

    public static void UnloadAsset(Object assetToUnload)
    {
        if (assetToUnload != null)
            Resources.UnloadAsset(assetToUnload);
    }

    public static void LoadAsync<T>(string path, AssetType type, System.Action<Object> cb) where T : Object
    {
        AddSysnLoad<T>(path, type, cb);
    }

    public static void CancelLoad(System.Action<Object> cb)
    {
        if (_asyn_list == null || _cnt <= 0) return;
        for (int i = _cnt - 1; i >= 0; i--)
        {
            if (_asyn_list[i].cb.Contains(cb))
                _asyn_list[i].cb.Remove(cb);

            if (_asyn_list[i].cb.Count <= 0)
            {
                _asyn_list.RemoveAt(i);
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
            if (_asyn_list == null) _asyn_list = new List<AsynAsset>();
            for (int i = 0, max = _asyn_list.Count; i < max; i++)
            {
                if (_asyn_list[i].path == path)
                {
                    _asyn_list[i].cb.Add(cb);
                    return;
                }
            }
            AsynAsset node = new AsynAsset();
            node.path = path;
            node.request = Resources.LoadAsync<T>(path);
            node.cb = new List<System.Action<Object>>();
            node.cb.Add(cb);
            _asyn_list.Add(node);
            _cnt = _asyn_list.Count;
        }
    }


    private static void DownloadDone(AsynAsset node)
    {
        for (int i = 0, max = node.cb.Count; i < max; i++)
        {
            node.cb[i](node.request.asset);
        }
        node.cb.Clear();
        _asyn_list.Remove(node);
        _cnt--;
    }

}
