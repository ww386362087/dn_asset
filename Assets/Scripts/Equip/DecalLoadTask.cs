using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DecalLoadTask : EquipLoadTask
{
    public Texture2D decalTex = null;
    protected LoadCallBack loadCb = null;
    private PartLoadTask faceTask = null;
    private PartLoadCallback m_PartLoadCb = null;
    public DecalLoadTask(EPartType p, PartLoadCallback partLoadCb, PartLoadTask face)
        : base(p)
    {
        loadCb = LoadFinish;
        faceTask = face;
        m_PartLoadCb = partLoadCb;
    }


    public override void Load( ref FashionPositionInfo newFpi,  HashSet<string> loadedPath)
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
                Texture2D tex = XResourceMgr.Load<Texture2D>(location);// ".tga"
                    LoadFinish(tex, this);   
            }
            else if (m_PartLoadCb != null)
            {
                m_PartLoadCb(this, false);
            }
        }
    }

    private void LoadFinish(UnityEngine.Object obj, System.Object cbOjb)
    {
        if (processStatus == EProcessStatus.EProcessing)
        {
            processStatus = EProcessStatus.EPreProcess;
            decalTex = obj as Texture2D;
            if (m_PartLoadCb != null)
            {
                m_PartLoadCb(this, false);
            }
        }
    }

    public override void Reset()
    {
        if (decalTex != null)
        {
            GameObject.DestroyObject(decalTex);
            decalTex = null;
        }
    }

    public void PostLoad(SkinnedMeshRenderer skin)
    {
        if (skin == null) return;
        if (decalTex != null)
        {
            skin.sharedMaterial.SetTexture("_Tex0", decalTex);
        }
        else
        {
            skin.sharedMaterial.SetTexture("_Tex0", faceTask.GetTexture());
        }
        processStatus = EProcessStatus.EProcessed;
    }
}
