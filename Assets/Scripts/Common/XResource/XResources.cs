using UnityEngine;
using System.IO;
using System.Collections.Generic;

/// <summary>
/// 资源加载管理
/// 同步+异步 
/// Resources+AssetBundle
/// </summary>
public class XResources
{
   
    /// <summary>
    /// 资源映射列表 - 主要为了删除时快读定位
    /// key Clone-Object的InstanceID, value是ABManager或者XResourceMgr的hash值
    /// </summary>
    private static Dictionary<int, uint> _asset_map = new Dictionary<int, uint>();
    /// <summary>
    /// 缓冲池（预加载）
    /// </summary>
    private static Dictionary<uint, Stack<GameObject>> _cache_pool = new Dictionary<uint, Stack<GameObject>>();

    private static MemoryStream _share_stream = new MemoryStream(8192);//512k
    private static float far = 1 << 10;

    private static XResController _res;
    private static XABController _ab;


    public static XABController ab { get { if (_ab == null) Init(); return _ab; } }

    public static void Init()
    {
        _res = new XResController();
        _ab = new XABController();
        _ab.Initial();
    }


    public static void Update()
    {
        _ab.Update();
        _res.Update();
    }

    /// <summary>
    /// 加载 GameObject 深复制（Instantiate） 注意卸载
    /// Texture, Material, Mesh, Audio等是共享的 (不会Instantiate)
    /// </summary>
    public static T Load<T>(string path, AssetType type) where T : Object
    {
        uint hash = 0;
        Object obt = LoadAsset<T>(path, type, out hash);
        T t = Obj2T<T>(obt);
        if (t != null)
        {
            int instance = t.GetInstanceID();
            _asset_map[instance] = hash;
        }
        return t;
    }

    /// <summary>
    /// 拿到的Asset, 方法不对外
    /// </summary>
    private static Object LoadAsset<T>(string path, AssetType type,out uint hash) where T: Object
    {
        Object obt; 
        if (_ab.Exist(path, type))
        {
            obt = _ab.Load<T>(path, type, out hash);
        }
        else
        {
            obt = _res.Load<T>(path, type, out hash);
        }
        return obt;
    }

    private static T Obj2T<T>(Object o) where T : Object
    {
        if (typeof(T) == typeof(GameObject)
            || typeof(T) == typeof(Transform))  //从ab拿到obj->Instantiate
        {
            return GameObject.Instantiate(o) as T;
        }
        else if (o is GameObject)
        {
            return (o as GameObject).GetComponent<T>();
        }
        else //resource.load 直接拿到texture,audio,material
        {
            return o as T;
        }
    }


    public static void SetAsynAssetIndex(int key, uint hash)
    {
        _asset_map[key] = hash;
    }

    /// <summary>
    /// 只能编辑器使用 
    /// 这个接口不走ab
    /// </summary>
    public static T[] LoadAll<T>(string path) where T : Object
    {
        return Resources.LoadAll<T>(path);
    }


    public static void LoadAsync<T>(string path, AssetType type, System.Action<Object> cb) where T : Object
    {
        if (_ab.Exist(path, type))
        {
            _ab.LoadAsyn<T>(path, type, cb);
        }
        else
        {
            _res.AsynLoad<T>(path, type, cb);
        }
    }


    public static void CreateInAdvance(string path, int cnt)
    {
        uint hash = XCommon.singleton.XHash(path);
        if (!_cache_pool.ContainsKey(hash))
            _cache_pool.Add(hash, new Stack<GameObject>());
        for (int i = 0; i < cnt; i++)
        {
            GameObject go = Load<GameObject>(path, AssetType.Prefab);
            go.transform.position = new Vector3(far, 0, far);
            if (go != null) _cache_pool[hash].Push(go);
        }
    }

    public static GameObject LoadInPool(string path)
    {
        uint hash = XCommon.singleton.XHash(path);
        if (_cache_pool.ContainsKey(hash))
        {
            var obj = _cache_pool[hash].Pop();
            if (_cache_pool[hash].Count <= 0)
                _cache_pool.Remove(hash);
            return obj;
        }
        else
            return Load<GameObject>(path, AssetType.Prefab);
    }

    public static void RecyleInPool(GameObject go, string path)
    {
        uint hash = XCommon.singleton.XHash(path);
        if (_cache_pool.ContainsKey(hash))
        {
            go.transform.position = new Vector3(far, 0, far);
            _cache_pool[hash].Push(go);
        }
        else
        {
            _cache_pool.Add(hash, new Stack<GameObject>());
            _cache_pool[hash].Push(go);
        }
    }

    public static void DestroyInPool(string path)
    {
        uint hash = XCommon.singleton.XHash(path);
        if (_cache_pool.ContainsKey(hash))
        {
            while (_cache_pool[hash].Count > 0)
            {
                Destroy(_cache_pool[hash].Pop());
            }
            _cache_pool.Remove(hash);
        }
    }

    /// <summary>
    /// 返回true 表示没有引用了
    /// </summary>
    private static bool UnloadAsset(uint hash)
    {
        if (_ab.ExistLoadBundle(hash))
        {
            return _ab.Unload(hash);
        }
        else
        {
            return _res.Unload(hash);
        }
    }

    /// <summary>
    /// 针对的是Asset-Object 而非Cloned-Object
    /// </summary>
    public static void UnloadAsset(Asset asset)
    {
        if (asset != null)
        {
            if (asset.is_clone_asset)
            {
#if UNITY_EDITOR
                // 在编辑器模式下无法卸载go物体，否则会报错让改用DestroyImmediate(obj, true)
                // 但这样做会连文件夹里的原始Asset一并删除
#else
                GameObject.DestroyImmediate(asset.obt);
#endif
            }
            else
            {
                //当使用Resources.UnloadAsset后，若依然有物体用该图，那么物体就变全黑 (异步执行的) 谨慎使用
                Resources.UnloadAsset(asset.obt);
            }
        }
    }

    /// <summary>
    /// 一般是切换场景的时候调用 如：OnLeaveScene
    /// </summary>
    public void UnloadAll()
    {
        _res.UnloadAll();
        _ab.UnloadAll();
    }


    /// <summary>
    /// 先根据GameObject的intanceid 确定AB引用次数 引用为0卸载asset-object
    /// 然后再销毁close-object
    /// </summary>
    public static void Destroy(Object obj)
    {
        if (obj == null) return;
        uint hash = 0;
        _asset_map.TryGetValue(obj.GetInstanceID(), out hash);
        if (hash != 0) UnloadAsset(hash);
        if (IsCloneAsset(obj))
        {
            GameObject.Destroy(obj);
            obj = null;
        }
    }

    public static void Destroy(Object obj, float delay)
    {
        XTimerMgr.singleton.SetTimer(delay, (param) => Destroy(obj));
    }

    public static bool IsCloneAsset<T>()
    {
        return typeof(T) == typeof(GameObject) || typeof(T) == typeof(Transform);
    }

    private static bool IsCloneAsset(Object obj)
    {
        return obj is GameObject || obj is Transform;
    }

    public static Stream ReadText(string location, bool error = true)
    {
        TextAsset data = Load<TextAsset>(location, AssetType.Text);
        if (data == null)
        {
            if (error) XDebug.LogError("Load resource: ", location, " error!");
            return null;
        }
        try
        {
            _share_stream.SetLength(0);
            _share_stream.Write(data.bytes, 0, data.bytes.Length);
            _share_stream.Seek(0, SeekOrigin.Begin);
            return _share_stream;
        }
        catch (System.Exception e)
        {
            XDebug.Log(e.Message, location);
            return _share_stream;
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
            if (s == _share_stream)
            {
                _share_stream.SetLength(0);
            }
            else
            {
                s.Close();
            }
        }
    }

}
