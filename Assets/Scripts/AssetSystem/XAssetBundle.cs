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
    /// ab 路径
    /// </summary>
    private string ab_apth;

    public uint hash;

    public XAssetBundle(string path)
    {
        ab_apth = path;
        life_cycle = 2f;
        hash = XCommon.singleton.XHash(path);
    }


    public Object LoadAsset(string loadName)
    {
        if (bundle == null)
        {
            //XDebug.Log("load name: " + loadName);
            bundle = AssetBundle.LoadFromFile(ab_apth);
            born_time = Time.time;
            ABManager.singleton.CacheBundle(this);
        }
        return bundle.LoadAsset(loadName);
    }


    public bool Unload(bool unloadall)
    {
        if (bundle != null)
        {
            ABManager.singleton.RemvBundle(this);
            bundle.Unload(unloadall);
            bundle = null;
            return true;
        }
        return false;
    }


    public void CheckUnload()
    {
        if (Time.time - born_time >= life_cycle)
        {
            Unload(false);
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
