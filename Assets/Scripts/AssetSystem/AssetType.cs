using UnityEngine;

public class AssetType
{

    public static string TGA = ".tga";

    public static string Prefab = ".prefab";

    public static string PNG = ".png";

    public static string Asset = ".asset";

    public static string Anim = ".anim";

    public static string Mat = ".mat";

    public static string Shader = ".shader";

    public static string Byte = ".bytes";

    public static string Controller = ".controller";


    private string V;

    public AssetType(string aa) { V = aa; }

    public static implicit operator string(AssetType id)
    {
        return id.ToString();
    }

    public static implicit operator AssetType(string id)
    {
        return new AssetType(id);
    }

    public override string ToString()
    {
        return V;
    }

    
}

