using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public struct FashionPositionInfo
{
    public int fasionID;

    public string equipName;
    public string presentName;
    public bool replace;


    public FashionPositionInfo(int id)
    {
        fasionID = id;
        equipName = "";
        presentName = "";
        replace = false;
    }

    public void Reset()
    {
        fasionID = 0;
        equipName = "";
        presentName = "";
        replace = false;
    }

    public bool Equals(ref FashionPositionInfo fpi)
    {
        return fasionID == fpi.fasionID && equipName == fpi.equipName && presentName == fpi.presentName;
    }
}


public enum ECombineStatus
{
    ENotCombine,
    ECombineing,
    ECombined,
}

public enum EProcessStatus
{
    ENotProcess,
    EProcessing,
    EPreProcess,
    EProcessed,
}

public enum EPartLoadType
{
    EPart,
    ESingle,
    EMount,
    EDecal,
}

public class BaseLoadTask
{

    public BaseLoadTask(EPartType p)
    {
        part = p;
        fpi.Reset();
    }

    public EPartType part = EPartType.ENum;
    public FashionPositionInfo fpi;
    public EProcessStatus processStatus = EProcessStatus.ENotProcess;

    public string location = "";

    public bool IsSamePart(ref FashionPositionInfo newFpi)
    {
        return fpi.Equals(newFpi) && processStatus >= EProcessStatus.EPreProcess;
    }

    public bool MakePath(ref FashionPositionInfo newFpi, HashSet<string> loadedPath)
    {
        fpi = newFpi;
        if (!string.IsNullOrEmpty(fpi.equipName))
        {
            location = "Equipments/" + fpi.equipName;
            processStatus = EProcessStatus.EProcessing;
        }
        if (loadedPath != null)
        {
            if (!string.IsNullOrEmpty(location) && !loadedPath.Contains(location))
            {
                loadedPath.Add(location);
                return true;
            }
            else
            {
                processStatus = EProcessStatus.EProcessed;
                return false;
            }
        }
        return !string.IsNullOrEmpty(location);
    }

    public virtual void Load(ref FashionPositionInfo newFpi, HashSet<string> loadedPath)
    {
        if (loadedPath != null)
        {
            if (!string.IsNullOrEmpty(location) && !loadedPath.Contains(location))
            {
                loadedPath.Add(location);
            }
            else
            {
                processStatus = EProcessStatus.EProcessed;
            }
        }
    }

    public virtual void Reset()
    {
        processStatus = EProcessStatus.ENotProcess;
        location = "";
    }

}
