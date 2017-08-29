using UnityEditor;


public class XEditorPath
{

    public static readonly string Sce = "Assets/XScene/";
    public static readonly string Cts = "Assets/Resources/CutScene/";
    public static readonly string Skp = "Assets/Resources/SkillPackage/";
    public static readonly string Crv = "Assets/Editor/EditorResources/Curve/";
    public static readonly string Cfg = "Assets/Editor/EditorResources/SkillPackage/";
    public static readonly string Sc = "Assets/Resources/Table/Scene/";
    public static readonly string Lev = "Assets/Resources/Table/Level/";

    private static readonly string _root = "Assets/Resources";
    private static readonly string _editor_root = "Assets/Editor";
    private static readonly string _editor_res_root = "Assets/Editor/EditorResources";

    public static string GetCfgFromSkp(string skp, string suffix = ".config")
    {
        skp = skp.Replace("/Resources/", "/Editor/EditorResources/");
        int m = skp.LastIndexOf('.');

        return skp.Substring(0, m) + suffix;
    }

    private static void RootPath()
    {
        if (!System.IO.Directory.Exists(_root))
        {
            AssetDatabase.CreateFolder("Assets", "Resources");
        }
    }

    private static void EditorRootPath()
    {
        if (!System.IO.Directory.Exists(_editor_root))
        {
            AssetDatabase.CreateFolder("Assets", "Editor");
        }

        if (!System.IO.Directory.Exists(_editor_res_root))
        {
            AssetDatabase.CreateFolder("Assets/Editor", "EditorResources");
        }
    }

    public static string BuildPath(string dictionary, string root)
    {
        string[] splits = dictionary.Split('/');
        string _base = root;

        foreach (string s in splits)
        {
            string path = _base + "/" + s + "/";

            if (!System.IO.Directory.Exists(path))
            {
                AssetDatabase.CreateFolder(_base, s);
            }

            _base = path.Substring(0, path.Length - 1);
        }

        return _base + "/";
    }

    public static string GetEditorBasedPath(string dictionary)
    {
        EditorRootPath();

        return BuildPath(dictionary, _editor_res_root);
    }

    public static string GetPath(string dictionary)
    {
        RootPath();

        return BuildPath(dictionary, _root);
    }
}
