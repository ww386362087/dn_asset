using UnityEngine;
using System.Collections.Generic;


public delegate void PartLoadCallback(BaseLoadTask part, bool needCombine);

public delegate void LoadCallBack(Object obj);

public class PartLoadTask : BaseLoadTask
{
    public GameObject go = null;
    public Mesh mesh = null;
    public Texture tex = null;
    private PartLoadCallback m_PartLoadCb = null;

    public PartLoadTask(EPartType p, PartLoadCallback partLoadCb)
        : base(p)
    {
        m_PartLoadCb = partLoadCb;
    }

    public override void Load(ref FashionPositionInfo newFpi, HashSet<string> loadedPath)
    {
        if (IsSamePart(ref newFpi))
        {
            if (m_PartLoadCb != null)
            {
                m_PartLoadCb(this, false);
            }
        }
        else
        {
            if (MakePath(ref newFpi, loadedPath))
            {
                mesh = XResourceMgr.Load<Mesh>(location,AssetType.Asset);
                tex = XResourceMgr.Load<Texture>(location, AssetType.TGA);
                LoadFinish(go, this);
            }
            else if (m_PartLoadCb != null)
            {
                m_PartLoadCb(this, true);
            }
        }
    }

    private void LoadFinish(UnityEngine.Object obj, System.Object cbOjb)
    {
        if (processStatus == EProcessStatus.EProcessing)
        {
            processStatus = EProcessStatus.EPreProcess;
        }
        if (m_PartLoadCb != null)
        {
            m_PartLoadCb(this, true);
        }
    }

    public void PostLoad(SkinnedMeshRenderer skin)
    {
        if (skin == null || tex == null) return;
        skin.sharedMaterial.SetTexture(XEquipUtil.GetPartOffset(part), tex);
    }

    public override void Reset()
    {
        if (tex != null)
        {
            XResourceMgr.UnloadAsset(tex);
            tex = null;
        }
        if(mesh!=null)
        {
            XResourceMgr.UnloadAsset(mesh);
            mesh = null;
        }
    }

    public bool HasMesh()
    {
        return mesh != null;
    }
    

}
