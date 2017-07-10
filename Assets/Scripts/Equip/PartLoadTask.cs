using UnityEngine;
using System.Collections.Generic;


public delegate void PartLoadCallback(BaseLoadTask part, bool needCombine);

public delegate void LoadCallBack(UnityEngine.Object obj, System.Object cbOjb);

public class PartLoadTask : BaseLoadTask
{
    public GameObject go = null;
    public XMeshTexData mtd = null;
    private LoadCallBack loadCb = null;
    private PartLoadCallback m_PartLoadCb = null;

    public PartLoadTask(EPartType p, PartLoadCallback partLoadCb)
        : base(p)
    {
        loadCb = LoadFinish;
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
                GameObject go = XResourceMgr.Load<GameObject>(location);
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
            go = obj as GameObject;
            if (go != null)
            {
                mtd = go.GetComponent<XMeshTexData>();
            }
        }
        if (m_PartLoadCb != null)
        {
            m_PartLoadCb(this, true);
        }
    }

    public void PostLoad(SkinnedMeshRenderer skin)
    {
        if (skin == null || mtd == null) return;
        skin.sharedMaterial.SetTexture(mtd._offset, mtd._tex);
    }

    public override void Reset()
    {
        if (mtd != null)
        {
            XResourceMgr.UnloadAsset(go);
            mtd = null;
        }
    }

    public bool HasMesh()
    {
        return mtd != null && mtd.mesh != null;
    }

    public Texture GetTexture()
    {
        return mtd == null ? null : mtd.tex;
    }

}
