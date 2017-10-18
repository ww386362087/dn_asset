using UnityEngine;

public class ShaderMgr
{
    public static Shader flow_Spec;
    public static Shader flow_Diffuse;
    public static Shader skin_cube;
    public static Shader skin_cube_nomask;
    public static Shader skin_cutout;
    public static Shader skin_nocutout;
    public static Shader skin_blend;
    public static Shader skin8;


    public static int[] shaderSkinID = new int[8];
    public static int shaderFaceID;
    public static int shaderHairID;
    public static int shaderBodyID;
    public static int shaderAlphaID;
    public static int shaderHairColorID;


    public static void Init()
    {
        flow_Spec = Shader.Find("Custom/Skin/FlowTexSpec");
        flow_Diffuse = Shader.Find("Custom/Skin/FlowTexDiff");
        skin_cube = Shader.Find("Custom/Skin/Cube");
        skin_cube_nomask = Shader.Find("Custom/Skin/CubeNoMask");
        skin_cutout = Shader.Find("Custom/Skin/RimlightBlendCutout");
        skin_nocutout = Shader.Find("Custom/Skin/RimlightBlendNoCutout");
        skin_blend = Shader.Find("Custom/Skin/RimlightBlend");
        skin8 = Shader.Find("Custom/Skin/RimlightBlend8");

        shaderFaceID = Shader.PropertyToID("_Face");
        shaderHairID = Shader.PropertyToID("_Hair");
        shaderBodyID = Shader.PropertyToID("_Body");
        shaderAlphaID = Shader.PropertyToID("_Alpha");
        shaderHairColorID = Shader.PropertyToID("_HairColor");
        for (int i = 0, max = shaderSkinID.Length; i < max; i++)
        {
            shaderSkinID[i] = Shader.PropertyToID("_Tex" + i);
        }
    }


    public static int GetPartOffset(EPartType partType)
    {
        return shaderSkinID[(int)partType];
    }
}

