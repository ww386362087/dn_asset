using UnityEngine;
using System.Collections.Generic;

public class XEquipComponent : XComponent
{

    private CombineMeshTask _combineMeshTask = null;

    public XEquipComponent()
    {
        _combineMeshTask = new CombineMeshTask(PartLoaded);
    }

    public override void OnInitial(XEntity e)
    {
        base.OnInitial(e);

        Transform skinmesh = e.EntityObject.transform.Find("CombinedMesh");
        SkinnedMeshRenderer skm = skinmesh.GetComponent<SkinnedMeshRenderer>();
        if (skm == null)
            skm = skinmesh.gameObject.AddComponent<SkinnedMeshRenderer>();
        _combineMeshTask.skin = skm;

        EquipTest();
    }

    private void PartLoaded(MountLoadTask mountPart)
    {
    }

    public void EquipTest()
    {
        FashionPositionInfo[] fashionList = new FashionPositionInfo[(int)FashionPosition.FASHION_ALL_END];
        FashionPositionInfo fpi = new FashionPositionInfo();
        fpi.fasionID = 0;
        fpi.presentName = "0";
        fpi.replace = false;
        fpi.equipName = "";
        fashionList[0] = fpi;
        fpi.equipName = "Player_archer_body";
        fashionList[1] = fpi;
        fpi.equipName = "Player_archer_leg";
        fashionList[2] = fpi;
        fpi.equipName = "Player_archer_glove";
        fashionList[3] = fpi;
        fpi.equipName = "Player_archer_boots";
        fashionList[4] = fpi;
        fpi.equipName = "weapon/Player_archer_weapon_archer";
        fashionList[5] = fpi;
        fpi.equipName = "Player_archer_quiver";
        fashionList[6] = fpi;
        fpi.equipName = "Player_archer_head";
        fashionList[10] = fpi;
        fpi.equipName = "Player_archer_hair";
        fashionList[11] = fpi;
        EquipAll(fashionList);
    }

    public void EquipAll(FashionPositionInfo[] fashionList, uint hairID = 0)
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
            int part = CombineMeshTask.ConvertPart((FashionPosition)i);
            if (part >= 0)
            {
                BaseLoadTask task = _combineMeshTask.parts[part];
                task.Load(ref fpi, loadPath);
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


        for (EPartType part = EPartType.ECombinePartStart; part < EPartType.ECombinePartEnd; ++part)
        {
            PartLoadTask loadPart = _combineMeshTask.parts[(int)part] as PartLoadTask;
            loadPart.PostLoad(_combineMeshTask.skin);
        }
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


}
