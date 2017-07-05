using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EquipComponent
{
    private CombineMeshTask _combineMeshTask = null;


    public EquipComponent()
    {
        _combineMeshTask = new CombineMeshTask(PartLoaded);
    }

    private void PartLoaded(MountLoadTask mountPart)
    {
    }

    private int m_mainWeaponID = 0;
    private int m_sideWeaponID = 0;
    public void EquipAll(FashionPositionInfo[] fashionList, uint hairID = 0)
    {
        if (fashionList == null)
        {
            Debug.LogError("null fashion list");
            return;
        }
        _combineMeshTask.BeginCombine();
        m_mainWeaponID = 0;
        m_sideWeaponID = 0;
        HashSet<string> loadPath = new HashSet<string>();
        int weapPart = (int)FashionPosition.FashionMainWeapon;
        int sideWeaponPart = (int)FashionPosition.FashionSecondaryWeapon;
        for (int i = 0, imax = fashionList.Length; i < imax; ++i)
        {
            FashionPositionInfo fpi = fashionList[i];
            if (weapPart == i)
                m_mainWeaponID = fpi.fasionID;
            else if (sideWeaponPart == i)
                m_sideWeaponID = fpi.fasionID;
        
            int part = CombineMeshTask.ConvertPart((FashionPosition)i);
            if (part >= 0)
            {
                EquipLoadTask task = _combineMeshTask.parts[part];
                task.Load( ref fpi, loadPath);
            }
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
  
        for (EPartType part = EPartType.ECombinePartEnd; part < EPartType.EMountEnd; ++part)
        {
            MountLoadTask loadPart = _combineMeshTask.parts[(int)part] as MountLoadTask;
            PartLoaded(loadPart);
        }
        for (EPartType part = EPartType.EMountEnd; part <= EPartType.EDecal; ++part)
        {
            DecalLoadTask loadPart = _combineMeshTask.parts[(int)part] as DecalLoadTask;
            loadPart.PostLoad(_combineMeshTask.skin);
        }
    }


    public void CreateRole()
    {
        Object o = Resources.Load<GameObject>("Prefabs/Player");
        GameObject gameObject = Object.Instantiate(o) as GameObject;
        Transform skinmesh = gameObject.transform.Find("CombinedMesh");
     
        SkinnedMeshRenderer skm = skinmesh.GetComponent<SkinnedMeshRenderer>();
        if (skm == null)
            skm = skinmesh.gameObject.AddComponent<SkinnedMeshRenderer>();
        _combineMeshTask.skin = skm;
    }

}
