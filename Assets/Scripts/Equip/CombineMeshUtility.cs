using UnityEngine;
using System.Collections;
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

    private string[] matTexName = new string[] { 
            string.Intern("_Tex0") ,
            string.Intern("_Tex1") ,
            string.Intern("_Tex2") ,
            string.Intern("_Tex3") ,
            string.Intern("_Tex4") ,
            string.Intern("_Tex5") ,
            string.Intern("_Tex6") ,
            string.Intern("_Tex7") ,
        };

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
            if (part.HasMesh())
            {
                partCount++;
            }
        }

        CombineInstance[] combineArray = GetMatCombineInstanceArray(partCount);
        if (combineArray != null)
        {
            //1.mesh collection
            bool isOnepart = false;
            bool haAlpha = false;
            PartLoadTask onePart = null;
            //bool isCutoutBlend = false;
            int index = 0;
            for (int i = (int)EPartType.ECombinePartStart; i < (int)EPartType.ECombinePartEnd; ++i)
            {
                PartLoadTask part = combineTask.parts[i] as PartLoadTask;
                if (part.HasMesh())
                {
                    CombineInstance ci = new CombineInstance();
                    if (part.mmtd != null)
                    {
                        ci.mesh = part.mmtd.mesh;
                        isOnepart = true;
                        haAlpha = part.mmtd.tex1 != null;
                        onePart = part;
                    }
                    else if (part.mtd != null)
                    {
                        ci.mesh = part.mtd.mesh;
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
            Material mainBody = combineTask.mainBody;
            if (mainBody != null)
            {
                XEquipUtil.ReturnMaterial(mainBody);
                mainBody = null;
            }
            if (mainBody == null)
            {
                if (CombineMeshTask.s_CombineMatType == ECombineMatType.ECombined)
                {
                    mainBody = XEquipUtil.GetRoleMat(isOnepart, haAlpha);
                }
                else
                {
                    mainBody = XEquipUtil.GetRoleMat(isOnepart, haAlpha, combineTask.roleType);
                }
            }

            combineTask.mainBody = mainBody;
            combineTask.skin.sharedMaterial = mainBody;

            //换脸
            //PartLoadTask facePart = combineTask.parts[XFastEnumIntEqualityComparer<EPartType>.ToInt(EPartType.EFace)];
            //PartLoadTask decalPart = combineTask.parts[XFastEnumIntEqualityComparer<EPartType>.ToInt(EPartType.EDecal)];
            //combineTask.faceTex = facePart.mtd != null ? facePart.mtd.tex : null;
            //Texture faceTex = (decalPart.processStatus == EProcessStatus.EProcessed && decalPart.t != null) ? decalPart.t : combineTask.faceTex;
            if (mainBody != null)
            {

                //mainBody.SetTexture(matTexName[0], faceTex);
                //int texIndex = XFastEnumIntEqualityComparer<EPartType>.ToInt(EPartType.EHair);
                if (isOnepart)
                {
                    PartLoadTask face = combineTask.parts[(int)EPartType.EFace] as PartLoadTask;
                    PartLoadTask hair = combineTask.parts[(int)EPartType.EHair] as PartLoadTask;
                    if (face.mtd != null)
                        mainBody.SetTexture("_Face", face.mtd.tex);
                    if (hair.mtd != null)
                        mainBody.SetTexture("_Hair", hair.mtd.tex);
                    if (onePart != null)
                    {
                        mainBody.SetTexture("_Body", onePart.mmtd.tex0);
                        if (onePart.mmtd.tex1 != null)
                            mainBody.SetTexture("_Alpha", onePart.mmtd.tex1);
                    }

                }
                else
                {
                    for (int i = 0, imax = (int)EPartType.ECombinePartEnd; i < imax; ++i)
                    {
                        PartLoadTask part = combineTask.parts[i] as PartLoadTask;
                        if (part.HasMesh())
                        {

                            mainBody.SetTexture(matTexName[i], part.mtd.tex);
                        }
                        else
                        {
                            mainBody.SetTexture(matTexName[i], null);
                        }
                    }
                }

            }
            return true;
        }
        return false;
    } 

}
