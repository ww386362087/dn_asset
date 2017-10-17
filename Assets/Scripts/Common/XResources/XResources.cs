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
    private static MemoryStream shareMemoryStream = new MemoryStream(8192);//512k

    /// <summary>
    /// 资源映射列表 - 主要为了删除时快读定位
    /// key Clone-Object的InstanceID, value是ABManager或者XResourceMgr的hash值
    /// </summary>
    private static Dictionary<int, uint> all_asset_map = new Dictionary<int, uint>();

    public static void Init()
    {
        ABManager.singleton.Initial();
    }


    public static void Update()
    {
        ABManager.singleton.Update();
        XResourceMgr.singleton.Update();
    }

    /// <summary>
    /// 加载 GameObject 深复制（Instantiate） 注意卸载
    /// Texture, Material, Mesh, Audio等是共享的 (不会Instantiate)
    /// </summary>
    public static T Load<T>(string path, AssetType type) where T : Object
    {
        Object obt; uint hash = 0;
        if (ABManager.singleton.Exist(path, type))
        {
            obt = ABManager.singleton.Load<T>(path, type, out hash);
        }
        else
        {
            obt = XResourceMgr.singleton.Load<T>(path, type, out hash);
        }
        T t = Obj2T<T>(obt);
        if (t != null)
        {
            int instance = t.GetInstanceID();
            all_asset_map[instance] = hash;
        }
        return t;
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


    public static void SetAsynAssetIndex(int key,uint hash)
    {
        all_asset_map[key] = hash;
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
        if (ABManager.singleton.Exist(path, type))
        {
            ABManager.singleton.LoadAsyn<T>(path, type, cb);
        }
        else
        {
            XResourceMgr.singleton.AsynLoad<T>(path, type, cb);
        }
    }

    /// <summary>
    /// 返回true 表示没有引用了
    /// </summary>
    private static bool UnloadAsset(uint hash)
    {
        if (ABManager.singleton.ExistLoadBundle(hash))
        {
            return ABManager.singleton.Unload(hash);
        }
        else
        {
            return XResourceMgr.singleton.Unload(hash);
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
        XResourceMgr.singleton.UnloadAll();
        ABManager.singleton.UnloadAll();
    }


    /// <summary>
    /// 先根据GameObject的intanceid 确定AB引用次数 引用为0卸载asset-object
    /// 然后再销毁close-object
    /// </summary>
    public static void SafeDestroy(Object obj)
    {
        if (obj == null) return;
        uint hash = 0;
        all_asset_map.TryGetValue(obj.GetInstanceID(), out hash);
        if (hash != 0) UnloadAsset(hash);
        if (obj is GameObject || obj is Transform)
        {
            GameObject.Destroy(obj);
            obj = null;
        }
    }


    public static bool IsCloneAsset<T>()
    {
        return typeof(T) == typeof(GameObject) 
            || typeof(T) == typeof(Transform);
    }


    public static Stream ReadText(string location, bool error = true)
    {
        TextAsset data = Load<TextAsset>(location, AssetType.Byte);
        if (data == null)
        {
            if (error) XDebug.LogError("Load resource: ", location, " error!");
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
            XDebug.Log(e.Message, location);
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
}
