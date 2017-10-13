using UnityEngine;
using System.Collections.Generic;
using XTable;


public class EquipPart
{
    public string[] partPath = new string[8];
    public string mainWeapon;
    public uint hash = 0;
    public List<string> suitName = new List<string>();
}

public class TempEquipSuit
{
    public uint hash = 0;
    public List<TempEquipData> data = new List<TempEquipData>();
}

public class TempEquipData
{
    public FashionList.RowData row;
    public string path;
}

public class ThreePart
{
    public string[] part = new string[3];
    public int id = 0;
}

public class XEquipUtil
{
    
    public static Material GetRoleMat()
    {
        return GetMaterial(ShaderMgr.skin8);
    }

    private static Material GetMaterial(Shader shader)
    {
        if (shader == ShaderMgr.skin_cutout)
        {
            return XResources.Load<Material>("Materials/Char/RimLightBlendCutout",AssetType.Mat);//mat
        }
        else if (shader == ShaderMgr.skin_nocutout)
        {
            return XResources.Load<Material>("Materials/Char/RimLightBlendNoCutout",AssetType.Mat);
        }
        else if (shader == ShaderMgr.skin_blend)
        {
            return XResources.Load<Material>("Materials/Char/RimLightBlend",AssetType.Mat);
        }
        else if (shader == ShaderMgr.skin8)
        {
            return XResources.Load<Material>("Materials/Char/RimLightBlend8",AssetType.Mat);
        }
        return new Material(shader);
    }
    

    public static void ReturnMaterial(Material mat)
    {
        if (mat != null)
        {
            Shader shader = mat.shader;
            if (shader == ShaderMgr.skin_cutout || shader == ShaderMgr.skin_nocutout)
            {
                mat.SetTexture("_Face", null);
                mat.SetTexture("_Hair", null);
                mat.SetTexture("_Body", null);
                mat.SetTexture("_Alpha", null);
            }
            else if (shader == ShaderMgr.skin_blend)
            {
                mat.SetTexture("_Face", null);
                mat.SetTexture("_Hair", null);
                mat.SetTexture("_Body", null);
            }
            else if (shader == ShaderMgr.skin8)
            {
                mat.SetTexture("_Tex0", null);
                mat.SetTexture("_Tex1", null);
                mat.SetTexture("_Tex2", null);
                mat.SetTexture("_Tex3", null);
                mat.SetTexture("_Tex4", null);
                mat.SetTexture("_Tex5", null);
                mat.SetTexture("_Tex6", null);
                mat.SetTexture("_Tex7", null);
            }
        }
    }

    public static string GetPartOffset(EPartType partType)
    {
        return "_Tex" + (int)partType;
    }

    public static SkinnedMeshRenderer GetSmr(GameObject keyGo)
    {
        Transform skinmesh = keyGo.transform.FindChild("CombinedMesh");
        if (skinmesh == null)
        {
            GameObject skinMeshGo = new GameObject("CombinedMesh");
            skinMeshGo.transform.parent = keyGo.transform;
            skinmesh = skinMeshGo.transform;
        }
        SkinnedMeshRenderer skm = skinmesh.GetComponent<SkinnedMeshRenderer>();
        if (skm == null)
            skm = skinmesh.gameObject.AddComponent<SkinnedMeshRenderer>();
        return skm;
    }

    public static void MakeEquip(string name, int[] fashionIDs, List<EquipPart> equipList, TempEquipSuit tmpFashionData, int suitID)
    {
        FashionList fashionList = XTableMgr.GetTable<FashionList>();
        if (fashionIDs != null)
        {
            tmpFashionData.hash = 0;
            tmpFashionData.data.Clear();
            bool threePart = false;
            for (int i = 0; i < fashionIDs.Length; ++i)
            {
                int fashionID = fashionIDs[i];
                FashionList.RowData row = fashionList.GetByUID(fashionID);
                if (row != null)
                {
                    List<ThreePart> tpLst = new List<ThreePart>();
                    if (row.EquipPos == 7 || row.EquipPos == 8 || row.EquipPos == 9)
                    {
                        ThreePart tp = FindThreePart(suitID, tpLst);
                        if (row.EquipPos == 9)
                        {
                            tp.part[2] = row.ModelPrefabArcher;
                        }
                        threePart = true;
                    }
                    else
                    {
                        if (row.ReplaceID != null && row.ReplaceID.Length > 1)
                        {
                            FashionList.RowData replace = fashionList.GetByUID(row.ReplaceID[1]);
                            if (replace != null)
                            {
                                if (replace.EquipPos == row.EquipPos) row = replace;
                            }
                        }
                        string path = row.ModelPrefabArcher;
                        if (!string.IsNullOrEmpty(path))
                        {
                            Hash(ref tmpFashionData.hash, path);
                            TempEquipData data = new TempEquipData();
                            data.row = row;
                            data.path = path;
                            tmpFashionData.data.Add(data);
                        }
                    }
                }
            }
            if (threePart) return;

            bool findSame = false;
            TempEquipSuit suit = tmpFashionData;
            if (suit.hash == 0) return;
            for (int j = 0; j < equipList.Count; ++j)
            {
                EquipPart part = equipList[j];
                if (part != null && part.hash == suit.hash)
                {
                    part.suitName.Add(name);
                    findSame = true;
                    break;
                }
            }
            if (!findSame)
            {
                EquipPart part = new EquipPart();
                part.hash = suit.hash;
                part.suitName.Add(name);
                for (int j = 0; j < suit.data.Count; ++j)
                {
                    TempEquipData data = suit.data[j];
                    int partPos = ConvertPart(data.row.EquipPos);
                    if (partPos < part.partPath.Length)
                    {
                        part.partPath[partPos] = data.path;
                    }
                    else if (partPos == part.partPath.Length)
                    {
                        part.mainWeapon = data.path;
                    }
                }
                equipList.Add(part);
            }
        }
    }

    private static void Hash(ref uint hash, string str)
    {
        for (int i = 0; i < str.Length; i++)
        {
            hash = (hash << 5) + hash + str[i];
        }
    }


    private static ThreePart FindThreePart(int id, List<ThreePart> lst)
    {
        for (int i = 0; i < lst.Count; ++i)
        {
            ThreePart tp = lst[i];
            if (tp.id == id) return tp;
        }
        ThreePart newTp = new ThreePart();
        newTp.id = id;
        lst.Add(newTp);
        return newTp;
    }

    private static int ConvertPart(int pos)
    {
        switch (pos)
        {
            case 0:
                return (int)EPartType.EHeadgear;
            case 1:
                return (int)EPartType.EUpperBody;
            case 2:
                return (int)EPartType.ELowerBody;
            case 3:
                return (int)EPartType.EGloves;
            case 4:
                return (int)EPartType.EBoots;
            case 5:
                return (int)EPartType.EMainWeapon;
            case 6:
                return (int)EPartType.ESecondaryWeapon;
            case 7:
                return (int)EPartType.EWings;
            case 8:
                return (int)EPartType.ETail;
            case 9:
                return (int)EPartType.EDecal;
            case 10:
                return (int)EPartType.EFace;
            case 11:
                return (int)EPartType.EHair;
        }
        return -1;
    }

    public static string GetDefaultPath(EPartType part, DefaultEquip.RowData data)
    {
        string partPath = "";
        switch (part)
        {
            case EPartType.EFace:
                partPath = data.Face;
                break;
            case EPartType.EHair:
                partPath = data.Hair;
                break;
            case EPartType.EUpperBody:
                partPath = data.Body;
                break;
            case EPartType.ELowerBody:
                partPath = data.Leg;
                break;
            case EPartType.EGloves:
                partPath = data.Glove;
                break;
            case EPartType.EBoots:
                partPath = data.Boots;
                break;
            case EPartType.ESecondaryWeapon:
                partPath = data.SecondWeapon;
                break;
            case EPartType.EMainWeapon:
                partPath = data.Weapon;
                break;
            case EPartType.EWings:
                partPath = data.WingPoint;
                break;
            case EPartType.ETail:
                partPath = data.TailPoint;
                break;
        }
        return partPath;
    }


}
