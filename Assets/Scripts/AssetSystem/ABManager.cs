using System.Collections.Generic;
using System.IO;
using UnityEngine;

public sealed class ABManager : XSingleton<ABManager>
{

    public AssetBundleDataReader depInfoReader;

    /// <summary>
    /// ab加载出来的对象
    /// </summary>
    private Dictionary<uint, Object> map;

    /// <summary>
    /// bundle的引用
    /// </summary>
    private List<XAssetBundle> bundles;

    private bool m_update = true;
    private int bundle_cnt = 0;
    private float update_time = 0;
    private float update_frequency = 0.5f;

    public void Initial()
    {
        map = new Dictionary<uint, Object>();
        LoadDepInfo();
    }

    public void Update()
    {
        if (bundle_cnt > 0 && m_update)
        {
            if (Time.time - update_time > update_frequency)
            {
                UpdateBundles();
                update_time = Time.time;
            }
        }
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
                XDebug.LogError("depFile not exist! ", depFile);
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
            else
            {
                XDebug.LogError("not find dep");
            }
        }
        depStream.Close();
    }


    public bool Exist(uint hash)
    {
        return depInfoReader.GetAssetBundleInfo(hash) != null;
    }

    public bool Exist(string location, AssetType type)
    {
        string path = location.Replace("/", "\\");
        AssetBundleData data = MakePath(path, type);
        return data != null && Exist(data.hash);
    }


    private AssetBundleData MakePath(string location, AssetType type)
    {
        string path = @"Assets\Resources\" + location.Replace("/", "\\") + type;
        return depInfoReader.GetAssetBundleInfoByAssetpath(path);
    }

    public Object LoadImm(string location, AssetType type)
    {
        AssetBundleData data = MakePath(location, type);
        Loader loader = new Loader(data);
        return loader.LoadImm();
    }

    public void LoadImm(string location, AssetType type, System.Action<Object> cb)
    {
        AssetBundleData data = MakePath(location, type);
        AsyncLoader loader = new AsyncLoader(data);
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


    private void UpdateBundles()
    {
        for (int i = bundle_cnt - 1; i >= 0; i--)
        {
            bundles[i].Update();
        }
    }

    private bool ContainsBundle(uint hash)
    {
        for (int i = 0; i < bundle_cnt; i++)
        {
            if (bundles[i].hash == hash) return true;
        }
        return false;
    }

    public bool CacheBundle(XAssetBundle bundle)
    {
        if (bundles == null) bundles = new List<XAssetBundle>();
        if (bundle != null && !ContainsBundle(bundle.hash))
        {
            bundles.Add(bundle);
            bundle_cnt++;
            return true;
        }
        return false;
    }


    public bool RemvBundle(XAssetBundle bundle)
    {
        if (bundle_cnt > 0 && ContainsBundle(bundle.hash))
        {
            //先减后删
            --bundle_cnt;
            bundles.Remove(bundle);
            return true;
        }
        return false;
    }

    public XAssetBundle GetBundle(string path)
    {
        uint hash = XCommon.singleton.XHash(path);
        if (bundle_cnt > 0)
        {
            for (int i = 0; i < bundle_cnt; i++)
            {
                if (bundles[i].hash == hash)
                {
                    XAssetBundle b = bundles[i];
                    b.OnReuse();
                    return b;
                }
            }
        }
        return new XAssetBundle(path);
    }

}
