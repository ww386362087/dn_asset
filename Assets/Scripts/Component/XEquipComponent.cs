using UnityEngine;
using System.Collections.Generic;
using XTable;


public class XEquipComponent : XComponent
{
    private List<EquipPart> m_FashionList = null;
    private List<EquipPart> m_EquipList = null;

    public BaseLoadTask[] parts = new BaseLoadTask[(int)EPartType.ENum];
    private PartLoadCallback m_PartLoaded = null;
    public SkinnedMeshRenderer skin = null;
    public MaterialPropertyBlock mpb = null;

    List<FashionPositionInfo> fashionList = null;

    public static int MaxPartCount = 8;
    private List<CombineInstance[]> matCombineInstanceArrayCache = new List<CombineInstance[]>();
    
    public XEquipComponent()
    {
        mpb = new MaterialPropertyBlock();
        for (int i = 0; i < MaxPartCount; ++i)
        {
            matCombineInstanceArrayCache.Add(new CombineInstance[i + 1]);
        }
    }
    
    public override void OnInitial(XObject o)
    {
        base.OnInitial(o);
        XEntity e = o as XEntity;

        //时装
        TempEquipSuit fashions = new TempEquipSuit();
        m_FashionList = new List<EquipPart>();
        var fashionsuit = XTableMgr.GetTable<FashionSuit>();
        for (int i = 0, max = fashionsuit.length; i < max; ++i)
        {
            FashionSuit.RowData row = fashionsuit[i];
            if (row.FashionID != null)
            {
                XEquipUtil.MakeEquip(row.SuitName, row.FashionID, m_FashionList, fashions, (int)row.SuitID);
            }
        }

        //装备
        m_EquipList = new List<EquipPart>();
        var equipsuit = XTableMgr.GetTable<EquipSuit>();
        for (int i = 0, max = equipsuit.length; i < max; ++i)
        {
            EquipSuit.RowData row = equipsuit[i];
            if (row.EquipID != null)
                XEquipUtil.MakeEquip(row.SuitName, row.EquipID, m_EquipList, fashions, -1);
        }

        Transform skinmesh = e.EntityObject.transform;
        skin = skinmesh.GetComponent<SkinnedMeshRenderer>();
        if (skin == null) skin = skinmesh.gameObject.AddComponent<SkinnedMeshRenderer>();
        skin.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On;
        e.skin = skin;

        for (EPartType part = EPartType.ECombinePartStart; part < EPartType.ECombinePartEnd; ++part)
        {
            parts[(int)part] = new PartLoadTask(part, this, m_PartLoaded);
        }
        for (EPartType part = EPartType.ECombinePartEnd; part < EPartType.EMountEnd; ++part)
        {
            parts[(int)part] = new MountLoadTask(part, this);
        }
        RegisterEvent(XEventDefine.XEvent_Detach_Host, OnDetachHost);

        EquipPart(m_FashionList[0]);
    }

    public override void OnUninit()
    {
        m_EquipList.Clear();
        m_FashionList.Clear();
        Object.Destroy(skin);
        mpb = null;
        skin = null;
        m_PartLoaded = null;
        if (fashionList != null)
        {
            fashionList.Clear();
            fashionList = null;
        }
        base.OnUninit();
    }

    public void EquipPart(EquipPart part)
    {
        fashionList = new List<FashionPositionInfo>();
        FashionPositionInfo fpi = new FashionPositionInfo();
        fpi.fasionID = 0;
        fpi.presentName = "0";
        fpi.replace = false;
        fpi.equipName = "";
        for (int i = 0; i < part.partPath.Length; ++i) //length = 8
        {
            string path = part.partPath[i];
            if (string.IsNullOrEmpty(path))
            {
                path = XEquipUtil.GetDefaultPath((EPartType)i, (xobj as XRole).defEquip);
            }
            fpi.equipName = path;
            fashionList.Add(fpi);
        }
        EquipAll(fashionList.ToArray());
    }

    public void AttachWeapon(string path)
    {
        if (fashionList != null)
        {
            FashionPositionInfo fpi = new FashionPositionInfo();
            fpi.equipName = "weapon/" + path;
            if (fashionList.Count > 8)
            {
                fashionList[8] = fpi;
            }
            else
            {
                fashionList.Add(fpi);
            }
            EquipAll(fashionList.ToArray());
        }
    }
    
    public void EquipAll(FashionPositionInfo[] fashionList)
    {
        if (fashionList == null)
        {
            XDebug.LogError("null fashion list");
            return;
        }
        HashSet<string> loadPath = new HashSet<string>();
        for (int i = 0, imax = fashionList.Length; i < imax; ++i)
        {
            FashionPositionInfo fpi = fashionList[i];
            BaseLoadTask task = parts[i];
            task.Load(ref fpi, loadPath);
        }
        Combine();
    }

    public void ChangeHairColor(Color color)
    {
        if (skin != null)
        {
            mpb.SetColor(ShaderMgr.shaderHairColorID, color);
            skin.SetPropertyBlock(mpb);
        }
    }

    private void OnDetachHost(XEventArgs e)
    {
        for (EPartType part = EPartType.ECombinePartStart; part < EPartType.EMountEnd; ++part)
        {
            var p = parts[(int)part];
            if (p != null) p.Reset();
        }
    }
    

    /// <summary>
    /// 处理mesh和tex的对应关系，并处理uv
    /// </summary>
    private bool Combine()
    {
        int partCount = 0;
        for (int i = (int)EPartType.ECombinePartStart; i < (int)EPartType.ECombinePartEnd; ++i)
        {
            PartLoadTask part = parts[i] as PartLoadTask;
            if (part.HasMesh()) partCount++;
        }
        CombineInstance[] combineArray = GetMatCombineInstanceArray(partCount);
        if (combineArray != null)
        {
            //1.mesh collection
            int index = 0;
            for (int i = (int)EPartType.ECombinePartStart; i < (int)EPartType.ECombinePartEnd; ++i)
            {
                PartLoadTask part = parts[i] as PartLoadTask;
                if (part.HasMesh())
                {
                    CombineInstance ci = new CombineInstance();
                    if (part.mesh != null)
                    {
                        ci.mesh = part.mesh;
                    }
                    ci.subMeshIndex = 0;
                    combineArray[index++] = ci;
                }
            }
            //2.combine
            if (skin.sharedMesh == null)
            {
                skin.sharedMesh = new Mesh();
            }
            else
            {
                skin.sharedMesh.Clear(true);
            }
            skin.gameObject.SetActive(false);
            skin.sharedMesh.CombineMeshes(combineArray, true, false);
            skin.gameObject.SetActive(true);

            //3.set material
            if (skin != null)
            {
                XEquipUtil.ReturnMaterial(skin.sharedMaterial);
            }
            skin.sharedMaterial = XEquipUtil.GetRoleMat();
            skin.GetPropertyBlock(mpb);

            //4. postload - set texture
            for (EPartType part = EPartType.ECombinePartStart; part < EPartType.EMountEnd; ++part)
            {
                parts[(int)part].PostLoad();
            }
            return true;
        }
        return false;
    }


    /// <summary>
    /// 根据combineinstantce的长度获取对应的array
    /// </summary>
    private CombineInstance[] GetMatCombineInstanceArray(int partCount)
    {
        int combineArrayIndex = partCount - 1;
        if (combineArrayIndex >= 0)
        {
            if (combineArrayIndex >= matCombineInstanceArrayCache.Count)
            {
                return new CombineInstance[partCount];
            }
            else
            {
                return matCombineInstanceArrayCache[combineArrayIndex];
            }
        }
        return null;
    }

}
