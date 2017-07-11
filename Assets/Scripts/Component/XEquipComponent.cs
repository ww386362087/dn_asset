using UnityEngine;
using System.Collections.Generic;
using XTable;

public class XEquipComponent : XComponent
{

    private CombineMeshTask _combineMeshTask = null;
   
    List<FashionPositionInfo> fashionList = null;
    public XEquipComponent()
    {
        _combineMeshTask = new CombineMeshTask(this);
    }

    public override void OnInitial(XEntity e)
    {
        base.OnInitial(e);

        Transform skinmesh = e.EntityObject.transform;
        SkinnedMeshRenderer skm = skinmesh.GetComponent<SkinnedMeshRenderer>();
        if (skm == null) skm = skinmesh.gameObject.AddComponent<SkinnedMeshRenderer>();
        _combineMeshTask.skin = skm;
    }



    public void EquipTest(EquipPart part)
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
                path = XEquipUtil.GetDefaultPath((EPartType)i, (entity as XRole).defEquip);
            }
            fpi.equipName = path;
            fashionList.Add(fpi);
        }

        EquipAll(fashionList.ToArray());
    }

    public void AttachWeapon(string path)
    {
        if(fashionList!=null)
        {
            FashionPositionInfo fpi = new FashionPositionInfo();
            fpi.equipName = "weapon/"+path;
            if (fashionList.Count>8)
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
            Debug.LogError("null fashion list");
            return;
        }
        _combineMeshTask.BeginCombine();
        HashSet<string> loadPath = new HashSet<string>();
        for (int i = 0, imax = fashionList.Length; i < imax; ++i)
        {
            FashionPositionInfo fpi = fashionList[i];
            BaseLoadTask task = _combineMeshTask.parts[i];
            task.Load(ref fpi, loadPath);
        }
        CombineMesh();
    }

    private void CombineMesh()
    {
        if (_combineMeshTask.needCombine)
        {
            CombineMeshUtility.singleton.Combine(_combineMeshTask);
        }
        _combineMeshTask.combineStatus = ECombineStatus.ECombined;


        for (EPartType part = EPartType.ECombinePartStart; part < EPartType.ECombinePartEnd; ++part)
        {
            PartLoadTask loadPart = _combineMeshTask.parts[(int)part] as PartLoadTask;
            loadPart.PostLoad(_combineMeshTask.skin);
        }
        for (EPartType part = EPartType.ECombinePartEnd; part < EPartType.EMountEnd; ++part)
        {
            MountLoadTask loadPart = _combineMeshTask.parts[(int)part] as MountLoadTask;
            loadPart.PostLoad();
        }
        for (EPartType part = EPartType.EMountEnd; part <= EPartType.EDecal; ++part)
        {
            DecalLoadTask loadPart = _combineMeshTask.parts[(int)part] as DecalLoadTask;
            loadPart.PostLoad(_combineMeshTask.skin);
        }
    }


}
