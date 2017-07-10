using UnityEngine;
using System.Collections;

public enum FashionPosition
{
    FASHION_START,
    FashionHeadgear = FASHION_START,
    FashionUpperBody,
    FashionLowerBody,
    FashionGloves,
    FashionBoots,
    FashionMainWeapon,
    FashionSecondaryWeapon,
    FashionWings,
    FashionTail,
    FashionDecal,
    FASHION_END,
    Face = FASHION_END,
    Hair,
    FASHION_ALL_END
}


public class CombineMeshTask
{

    public BaseLoadTask[] parts = new BaseLoadTask[(int)EPartType.ENum];

    public ECombineStatus combineStatus = ECombineStatus.ENotCombine;

    private static int m_FinalLoadStatus = 0;
    private int m_LoadStatus = 0;
    private PartLoadCallback m_PartLoaded = null;
    public bool needCombine = false;
    public uint roleType;

    public SkinnedMeshRenderer skin = null;
    public Material mat = null;

 
    public CombineMeshTask(XEquipComponent ec)
    {
        m_PartLoaded = PartLoadFinish;
        for (EPartType part = EPartType.ECombinePartStart; part < EPartType.ECombinePartEnd; ++part)
        {
            parts[(int)part] = new PartLoadTask(part, m_PartLoaded);
        }
        for (EPartType part = EPartType.ECombinePartEnd; part < EPartType.EMountEnd; ++part)
        {
            parts[(int)part] = new MountLoadTask(part,ec);
        }
        for (EPartType part = EPartType.EMountEnd; part < EPartType.ENum; ++part)
        {
            parts[(int)part] = new DecalLoadTask(part, m_PartLoaded, parts[0] as PartLoadTask);
        }
        if (m_FinalLoadStatus == 0)
        {
            for (EPartType part = EPartType.ECombinePartStart; part < EPartType.ECombinePartEnd; ++part)
            {
                m_FinalLoadStatus |= 1 << (int)part;
            }
            for (EPartType part = EPartType.EDecal; part < EPartType.ENum; ++part)
            {
                m_FinalLoadStatus |= 1 << (int)part;
            }
        }
    }

    public void Reset()
    {
        for (int i = 0; i < parts.Length; ++i)
        {
            BaseLoadTask task = parts[i];
            task.Reset();
        }
    }

    public void BeginCombine()
    {
        m_LoadStatus = 0;
        needCombine = false;
        combineStatus = ECombineStatus.ENotCombine;
    }

    public bool EndCombine()
    {
        combineStatus = ECombineStatus.ECombineing;
        return m_FinalLoadStatus == m_LoadStatus;
    }

    public void AddLoadPart(EPartType part)
    {
        int partEnum = 1 << (int)part;
        m_LoadStatus |= partEnum;
    }

    public void PartLoadFinish(BaseLoadTask part, bool combinePart)
    {
        needCombine |= combinePart;
        AddLoadPart(part.part);
    }

    public bool Process()
    {
        switch (combineStatus)
        {
            case ECombineStatus.ENotCombine:
            case ECombineStatus.ECombined:
                return false;
            case ECombineStatus.ECombineing:
                return m_FinalLoadStatus == m_LoadStatus;
        }
        return false;
    }


    public static int ConvertPart(FashionPosition fp)
    {
        switch (fp)
        {
            case FashionPosition.FashionHeadgear:
                return (int)EPartType.EHeadgear;
            case FashionPosition.FashionUpperBody:
                return (int)EPartType.EUpperBody;
            case FashionPosition.FashionLowerBody:
                return (int)EPartType.ELowerBody;
            case FashionPosition.FashionGloves:
                return (int)EPartType.EGloves;
            case FashionPosition.FashionBoots:
                return (int)EPartType.EBoots;
            case FashionPosition.FashionMainWeapon:
                return (int)EPartType.EMainWeapon;
            case FashionPosition.FashionSecondaryWeapon:
                return (int)EPartType.ESecondaryWeapon;
            case FashionPosition.FashionWings:
                return (int)EPartType.EWings;
            case FashionPosition.FashionTail:
                return (int)EPartType.ETail;
            case FashionPosition.FashionDecal:
                return (int)EPartType.EDecal;
            case FashionPosition.Face:
                return (int)EPartType.EFace;
            case FashionPosition.Hair:
                return (int)EPartType.EHair;
        }
        return -1;
    }

}
