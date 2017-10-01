using System.IO;
using UnityEngine;


public class XAssetBundle
{

    private AssetBundle bundle;

    /// <summary>
    /// 生成时间
    /// </summary>
    private float born_time;

    /// <summary>
    /// 生命周期 默认2s
    /// 在销毁前 再次引用 生命周期+1s
    /// </summary>
    private float life_cycle;

    /// <summary>
    /// ab 路径 ex:D:\projects\dn_asset\Assets\StreamingAssets\update\AssetBundles\1792139362.ab
    /// </summary>
    private string ab_apth;

    /// <summary>
    /// data.assetpath ex:Assets\Resources\UI\Canvas2.prefab
    /// </summary>
    private AssetBundleData ab_data;

    public uint hash;

    public XAssetBundle(AssetBundleData data)
    {
        Init(data);
    }


    //异步加载的构造函数 
    public XAssetBundle(AssetBundleData data, AssetBundle b)
    {
        Init(data);
        born_time = Time.time;
        bundle = b;
        ABManager.singleton.CacheBundle(this);
    }

    private void Init(AssetBundleData data)
    {
        ab_apth = Path.Combine(AssetBundlePathResolver.BundleCacheDir, data.hash + ".ab");
        life_cycle = 2f;
        ab_data = data;
        hash = data.hash;
    }

    public Object LoadAsset(string loadName)
    {
        if (bundle == null)
        { 
            bundle = AssetBundle.LoadFromFile(ab_apth);
            born_time = Time.time;
            ABManager.singleton.CacheBundle(this);
           // XDebug.Log("ab load name: " + loadName , " path: " + ab_apth , " assetpath: " + ab_data.assetpath);
        }
        return bundle.LoadAsset(loadName);
    }


    public bool Unload(bool unloadall, bool force)
    {
        if (bundle != null)
        {
            //默认公共资源不会卸载 除非force=true强制卸载（切场景时候用）
            if (ab_data.compositeType != AssetBundleExportType.Standalone || force)
            {
                //XDebug.LogGreen("ab unload ",ab_data.assetpath);
                ABManager.singleton.RemvBundle(this);
                bundle.Unload(unloadall);
                bundle = null;
                return true;
            }
        }
        return false;
    }


    public void CheckUnload()
    {
        if (Time.time - born_time >= life_cycle)
        {
           // ABManager.singleton.Debug();
            Unload(false,false);
            //ABManager.singleton.Debug();
        }
    }

    
    public void Update()
    {
        if (bundle != null)
        {
            CheckUnload();
        }
    }

    public void OnReuse()
    {
        if(bundle!=null)
        {
            life_cycle += 1f;
        }
    }

}
