using UnityEngine;
using System.Collections;

public class XEquipUtil
{
    public static CombineMeshUtility _CombineMeshUtility = null;

    public static readonly Shader _flow_Spec = Shader.Find("Custom/Skin/FlowTexSpec");
    public static readonly Shader _flow_Diffuse = Shader.Find("Custom/Skin/FlowTexDiff");
    public static readonly Shader _skin_cube = Shader.Find("Custom/Skin/Cube");
    public static readonly Shader _skin_cube_nomask = Shader.Find("Custom/Skin/CubeNoMask");
    public static readonly Shader _skin_cutout = Shader.Find("Custom/Skin/RimlightBlendCutout");
    public static readonly Shader _skin_nocutout = Shader.Find("Custom/Skin/RimlightBlendNoCutout");
    public static readonly Shader _skin_blend = Shader.Find("Custom/Skin/RimlightBlend");
    public static readonly Shader _skin8 = Shader.Find("Custom/Skin/RimlightBlend8");

    private static Shader _EquipShader = null;


    public static Material GetRoleMat(bool isOnepart, bool hasAlpha, uint roleType = 0)
    {
        if (isOnepart)
        {
            if (hasAlpha)
            {
                return GetMaterial(_skin_cutout);
            }
            else
            {
                return GetMaterial(_skin_blend);
            }
        }
        else
        {
            if (roleType > 0)
                return Resources.Load<Material>(GetRoleMaterial(roleType));
            else
            {
                return GetMaterial(_skin8);
            }
        }
    }


    private static string GetRoleMaterial(uint roleType)
    {
        //XProfessionSkillMgr.singleton.GetRoleMaterial((uint)roleType)
        return string.Empty;
    }


    private static Material GetMaterial(Shader shader)
    {
        if (shader == _skin_cutout)
        {
            return Resources.Load<Material>("Materials/Char/RimLightBlendCutout");//mat
        }
        else if (shader == _skin_nocutout)
        {
            return Resources.Load<Material>("Materials/Char/RimLightBlendNoCutout");
        }
        else if (shader == _skin_blend)
        {
            return Resources.Load<Material>("Materials/Char/RimLightBlend");
        }
        else if (shader == _skin8)
        {
            return Resources.Load<Material>("Materials/Char/RimLightBlend8");
        }
        return new Material(shader);
    }

    public static Shader GetEquipShader()
    {
        if (_EquipShader == null)
            _EquipShader = Shader.Find("Custom/Skin/RimlightBlend8");
        return _EquipShader;
    }


    public static void ReturnMaterial(Material mat)
    {
        if (mat != null)
        {
            Shader shader = mat.shader;
            if (shader == _skin_cutout || shader == _skin_nocutout)
            {
                mat.SetTexture("_Face", null);
                mat.SetTexture("_Hair", null);
                mat.SetTexture("_Body", null);
                mat.SetTexture("_Alpha", null);
            }
            else if (shader == _skin_blend)
            {
                mat.SetTexture("_Face", null);
                mat.SetTexture("_Hair", null);
                mat.SetTexture("_Body", null);
            }
            else if (shader == _skin8)
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
            UnityEngine.Object.Destroy(mat);
        }
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



    public static void Test()
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
        EquipComponent com = new EquipComponent();
        com.CreateRole();
        com.EquipAll(fashionList);

    }


}
