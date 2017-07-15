using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class ABManager:XSingleton<ABManager>
{

    private MonoBehaviour mono;
    public AssetBundleDataReader depInfoReader;
    private Dictionary<uint, Object> map;

    public void Init(MonoBehaviour entraince)
    {
        mono = entraince;
        map = new Dictionary<uint, Object>();
        LoadDepInfo();
    }

    void LoadDepInfo()
    {
        string depFile = string.Format("{0}/{1}", AssetBundlePathResolver.BundleCacheDir, AssetBundlePathResolver.DependFileName);
        if (File.Exists(depFile))
        {
            FileStream fs = new FileStream(depFile, FileMode.Open, FileAccess.Read);
            Init(fs);
            fs.Close();
        }
        else
        {
            TextAsset o = Resources.Load<TextAsset>("dep");
            if (o != null)
            {
                Init(new MemoryStream(o.bytes));
            }
            else
            {
                Debug.LogError("depFile not exist! "+depFile);
            }
        }
    }
    
    public void Init(Stream depStream)
    {
        if (depStream.Length > 4)
        {
            BinaryReader br = new BinaryReader(depStream);
            if (br.ReadChar() == 'A' && br.ReadChar() == 'B' && br.ReadChar() == 'D')
            {
                if (br.ReadChar() == 'T')
                    depInfoReader = new AssetBundleDataReader();
                else
                    depInfoReader = new AssetBundleDataBinaryReader();
                depStream.Position = 0;
                depInfoReader.Read(depStream);
            }
        }
        depStream.Close();
    }




    public bool Exist(uint hash)
    {
        return depInfoReader.GetAssetBundleInfo(hash) != null;
    }

    public bool Exist(string location, string suffix)
    {
        string path = location.Replace("/", "\\");
        AssetBundleData data = MakePath(path, suffix);
        return data != null && Exist(data.hash);
    }


    private AssetBundleData MakePath(string location, string suffix)
    {
        string path = @"Assets\Resources\" + location.Replace("/", "\\") + suffix;
       // Debug.Log("path: " + path);
        return depInfoReader.GetAssetBundleInfoByAssetpath(path);
    }

    public Object LoadImm(string location, string suffix) 
    {
        AssetBundleData data = MakePath(location, suffix);
        SyncLoader loader = new SyncLoader(data, mono);
        return loader.LoadImm();
    }

    public void LoadImm(string location, string suffix, System.Action<Object> cb)
    {
        AssetBundleData data = MakePath(location, suffix);
        AsyncLoader loader = new AsyncLoader(data, mono);
        loader.LoadImm(cb);
    }
    

    public void CacheObject(uint hash, Object obj)
    {
        if (!map.ContainsKey(hash))
        {
            map.Add(hash, obj);
        }
        else
        {
            map[hash] = obj;
        }
    }


    public bool IsCached(uint hash)
    {
        return map.ContainsKey(hash);
    }


    public Object GetCache(uint hash)
    {
        if (IsCached(hash)) return map[hash];
        return null;
    }

}
