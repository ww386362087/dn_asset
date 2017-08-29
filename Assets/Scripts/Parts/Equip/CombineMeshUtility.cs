using UnityEngine;
using System.Collections.Generic;

public class CombineMeshUtility :XSingleton<CombineMeshUtility>
{

    public CombineMeshUtility()
    {
        for (int i = 0; i < MaxPartCount; ++i)
        {
            matCombineInstanceArrayCache.Add(new CombineInstance[i + 1]);
        }
    }

    public static int MaxPartCount = 8;

    private List<CombineInstance[]> matCombineInstanceArrayCache = new List<CombineInstance[]>();

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

    /// <summary>
    /// 处理mesh和tex的对应关系，并处理uv
    /// </summary>
    public bool Combine(CombineMeshTask combineTask)
    {
        int partCount = 0;
        for (int i = (int)EPartType.ECombinePartStart; i < (int)EPartType.ECombinePartEnd; ++i)
        {
            PartLoadTask part = combineTask.parts[i] as PartLoadTask;
            if (part.HasMesh()) partCount++;
        }
        CombineInstance[] combineArray = GetMatCombineInstanceArray(partCount);
        if (combineArray != null)
        {
            //1.mesh collection
            int index = 0;
            for (int i = (int)EPartType.ECombinePartStart; i < (int)EPartType.ECombinePartEnd; ++i)
            {
                PartLoadTask part = combineTask.parts[i] as PartLoadTask;
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
            if (combineTask.skin.sharedMesh == null)
            {
                combineTask.skin.sharedMesh = new Mesh();
            }
            else
            {
                combineTask.skin.sharedMesh.Clear(true);
            }
            combineTask.skin.gameObject.SetActive(false);
            combineTask.skin.sharedMesh.CombineMeshes(combineArray, true, false);
            combineTask.skin.gameObject.SetActive(true);

            //3.set material
            Material mat = combineTask.mat;
            if (mat != null)
            {
                XEquipUtil.ReturnMaterial(mat);
                mat = null;
            }
            mat = XEquipUtil.GetRoleMat();
            combineTask.mat = mat;
            combineTask.skin.sharedMaterial = mat;
            return true;
        }
        return false;
    } 

}
