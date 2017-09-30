using UnityEngine;
using System.IO;

/// <summary>
/// 资源加载管理
/// 同步+异步 
/// Resources+AssetBundle
/// </summary>
public class XResources
{
    private static MemoryStream shareMemoryStream = new MemoryStream(8192);//512k
    

    public static void Update()
    {
        ABManager.singleton.Update();
        XResourceMgr.singleton.Update();
    }


    /// <summary>
    /// 加载 GameObject 深复制（Instantiate） 注意卸载
    /// Texture, Material, Audio等是共享的 (不会Instantiate)
    /// </summary>
    public static T Load<T>(string path, AssetType type) where T : Object
    {
        Object obt = null;
        if ( ABManager.singleton.Exist(path, type))
        {
            obt = ABManager.singleton.LoadImm(path, type);
        }
        else
        {
            obt = Resources.Load<T>(path);
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
        else if (o is GameObject)
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
             XDebug.Log(e.Message , location);
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
        if ( ABManager.singleton.Exist(path, type))
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
        if (ABManager.singleton.Exist(path, type))
        {
             ABManager.singleton.Unload(path, type);
        }
        else
        {
             XResourceMgr.singleton.Unload(path, type);
        }
    }

    /// <summary>
    /// 针对的是Asset-Object 而非Cloned-Object
    /// </summary>
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
                //当使用Resources.UnloadAsset后，若依然有物体用该图，那么物体就变全黑 谨慎使用
                Resources.UnloadAsset(assetToUnload);
            }
        }
    }

    public static void LoadAsync<T>(string path, AssetType type, System.Action<Object> cb) where T : Object
    {
        AddSysnLoad<T>(path, type, cb);
    }
    

    private static void AddSysnLoad<T>(string path, AssetType type, System.Action<Object> cb) where T : Object
    {
        if (ABManager.singleton.Exist(path, type))
        {
            ABManager.singleton.LoadAsyn(path, type, cb);
        }
        else
        {
            XResourceMgr.singleton.AsynLoad(path, type, cb);
        }
    }
    
    public static void SafeDestroy(ref GameObject obj)
    {
        Object.Destroy(obj);
        obj = null;
    }
}
