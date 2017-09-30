using System.Collections.Generic;
using System.IO;
using UnityEngine;

public sealed class ABManager : XSingleton<ABManager>
{

    public AssetBundleDataReader depInfoReader;

    /// <summary>
    /// ab加载出来的对象
    /// UnityEngine.Object，但是Asset-Object和Cloned-Object本质是不同的
    /// 例如，如果对Asset-Object执行Destroy 会报错：Destroying assets is not permitted to avoid data loss
    /// 加载完cache 切场景卸载
    /// </summary>
    private Dictionary<uint, Asset> map;

    /// <summary>
    /// bundle的引用 
    /// bundle是加载完即刻卸载的 和map里的正好相反
    /// </summary>
    private List<XAssetBundle> bundles;

    private bool m_update = true;
    private int bundle_cnt = 0;
    private float update_time = 0;
    private float update_frequency = 0.5f;

    public void Initial()
    {
        map = new Dictionary<uint, Asset>();
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

    /// <summary>
    /// 一般是切换场景的时候调用 如：OnLeaveScene
    /// </summary>
    public void UnloadAll()
    {
        if (bundle_cnt > 0)
        {
            for (int i = 0; i < bundle_cnt; i++)
            {
                bundles[i].Unload(true, true);
            }
            bundle_cnt = 0;
            bundles.Clear();
        }
        if (map != null)
        {
            var e = map.GetEnumerator();
            while (e.MoveNext())
            {
                XResources.UnloadAsset(e.Current.Value.obt);
            }
            e.Dispose();
            map.Clear();
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

    public void Unload(string path, AssetType type)
    {
        AssetBundleData data = MakePath(path, type);
        if (map != null && map.ContainsKey(data.hash))
        {
            map[data.hash].ref_cnt--;
            if (map[data.hash].ref_cnt <= 0)
            {
                XResources.UnloadAsset(map[data.hash].obt);
                map[data.hash].obt = null;
                map.Remove(data.hash);
            }
        }
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

    public void LoadAsyn(string location, AssetType type, System.Action<Object> cb)
    {
        AssetBundleData data = MakePath(location, type);
        AsyncLoader loader = new AsyncLoader(data);
        loader.LoadAsyn(cb);
    }


    public void CacheObject(uint hash, Object obj)
    {
        if (!IsCached(hash))
        {
            map.Add(hash, new Asset() { obt = obj, ref_cnt = 1 });
        }
        else
        {
            throw new System.Exception("the asset has cached!");
        }
    }
    
    public bool IsCached(uint hash)
    {
        return map.ContainsKey(hash);
    }


    public Asset GetCache(uint hash)
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
            --bundle_cnt;
            bundles.Remove(bundle);
            return true;
        }
        return false;
    }

    public XAssetBundle GetBundle(AssetBundleData data)
    {
        if (bundle_cnt > 0)
        {
            for (int i = 0; i < bundle_cnt; i++)
            {
                if (bundles[i].hash == data.hash)
                {
                    XAssetBundle b = bundles[i];
                    b.OnReuse();
                    return b;
                }
            }
        }
        return new XAssetBundle(data);
    }

}
