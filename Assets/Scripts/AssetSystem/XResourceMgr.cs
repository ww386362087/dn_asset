using UnityEngine;
using System.Collections.Generic;

public class XResourceMgr 
{

    private struct AssetNode
    {
        public string path;
        public List<System.Action<Object>> cb;
    }



    private static List<AssetNode> list = new List<AssetNode>();

    private static Dictionary<uint, Object> asynlist = new Dictionary<uint, Object>();


    public static void Update()
    {

    }

    public static T Load<T>(string path)where T : Object
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

    public static ResourceRequest LoadAsync(string path, System.Action<Object> cb)
    {
        return Resources.LoadAsync(path);
    }


    
    
}
