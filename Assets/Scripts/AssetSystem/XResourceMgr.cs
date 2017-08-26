using UnityEngine;
using System.Collections.Generic;
using System.IO;

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
        public bool fromAB;
    }

    private static MemoryStream shareMemoryStream = new MemoryStream(8192);//512k

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

    private static Object FindInSynPool(string path, AssetType type)
    {
        if (_syn_list != null)
        {
            var e = _syn_list.GetEnumerator();
            while (e.MoveNext())
            {
                if (e.Current.path.Equals(path)
                    && e.Current.type.Equals(type))
                {
                    return e.Current.obj;
                }
            }
        }
        return null;
    }

    private static bool RemoveInSynPool(string path, AssetType type)
    {
        if (_syn_list != null)
        {
            var e = _syn_list.GetEnumerator();
            while (e.MoveNext())
            {
                if (e.Current.path.Equals(path)
                    && e.Current.type.Equals(type))
                {
                    Asset asset = e.Current;
                    _syn_list.Remove(asset);
                    return true;
                }
            }
        }
        return false;
    }

    /// <summary>
    /// 加载 GameObject 深复制（Instantiate） 注意卸载
    /// Texture, Material, Audio等是共享的 (不会Instantiate)
    /// </summary>
    public static T Load<T>(string path, AssetType type) where T : Object
    {
        if (_syn_list == null) _syn_list = new List<Asset>();
        Object obt = FindInSynPool(path, type);
        if (obt == null)
        {
            if (useAB && ABManager.singleton.Exist(path, type))
            {
                obt = ABManager.singleton.LoadImm(path, type);
                _syn_list.Add(new Asset() { path = path, type = type, fromAB = true, obj = obt });
            }
            else
            {
                obt = Resources.Load<T>(path);
                _syn_list.Add(new Asset() { path = path, type = type, fromAB = false, obj = obt });
            }
        }
        return Obj2T<T>(obt);
    }


    private static T Obj2T<T>(Object o) where T : Object
    {
        if (typeof(T) == typeof(GameObject)
            || typeof(T) == typeof(Transform))  //从ab拿到obj->Instantiate
        {
            return GameObject.Instantiate(o) as T;
        }
        else if (o is GameObject && typeof(T) == typeof(GameObject))
        {
            return (o as GameObject).GetComponent<T>();
        }
        else //resource.load 直接拿到texture,audio,material
        {
            return o as T;
        }
    }

    public static Stream ReadText(string location, bool error = true)
    {
        TextAsset data = Load<TextAsset>(location,AssetType.Byte);

        if (data == null)
        {
            if (error)
                LoadErrorLog(location);
            else
                return null;
        }
        try
        {
            shareMemoryStream.SetLength(0);
            shareMemoryStream.Write(data.bytes, 0, data.bytes.Length);
            shareMemoryStream.Seek(0, SeekOrigin.Begin);
            return shareMemoryStream;
        }
        catch (System.Exception e)
        {
            Debug.Log(e.Message + location);
            return shareMemoryStream;
        }
        finally
        {
            Resources.UnloadAsset(data);
        }
    }

    public static void ClearStream(Stream s)
    {
        if (s != null)
        {
            if (s == shareMemoryStream)
            {
                shareMemoryStream.SetLength(0);
            }
            else
            {
                s.Close();
            }
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

    public static void UnloadAsset(string path, AssetType type)
    {
        Object o = FindInSynPool(path, type);
        UnloadAsset(o);
        RemoveInSynPool(path, type);
    }

    public static void UnloadAsset(Object assetToUnload)
    {
        if (assetToUnload != null)
        {
            if (assetToUnload is GameObject)
            {
#if UNITY_EDITOR
                // 在编辑器模式下无法卸载go物体，否则会报错让改用DestroyImmediate(obj, true)
                // 但这样做会连文件夹里的原始Asset一并删除
#else
                GameObject.DestroyImmediate(assetToUnload);
#endif
            }
            else
            {
                //当使用Resources.UnloadAsset后，若依然有物体用该图，那么物体就变全黑
                Resources.UnloadAsset(assetToUnload);
            }
        }
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

    public static void LoadErrorLog(string prefab)
    {
        Debug.LogError("Load resource: " + prefab + " error!");
    }

}
