using XTable;
using UnityEngine;
using UnityEditor;


internal class XEditorLibrary
{

    /// <summary>
    /// 帧率 value = 30
    /// </summary>
    public const float FPS = 30.0f;

    public static readonly string Sce = "Assets/Scenes/";
    public static readonly string Cts = "Assets/Resources/Table/CutScene/";
    public static readonly string Skp = "Assets/Resources/Table/Skill/";
    public static readonly string Crv = "Assets/Editor/EditorResources/Curve/";
    public static readonly string Cfg = "Assets/Editor/EditorResources/Skill/";
    public static readonly string Sc = "Assets/Resources/Table/Map/";
    public static readonly string Lev = "Assets/Resources/Table/Level/";
    public static readonly string Ai = "Assets/Resources/Table/AITree/";
    public static readonly string Comb = "Assets/Editor/EditorResources/ImporterData/CombineConfig.prefab";

    private static readonly string _root = "Assets/Resources";
    private static readonly string _editor_root = "Assets/Editor";
    private static readonly string _editor_res_root = "Assets/Editor/EditorResources";


    public static GameObject GetStatics(uint statictid)
    {
        XEntityStatistics.RowData row = XTableMgr.GetTable<XEntityStatistics>().GetByID((int)statictid);
        if (row != null) return GetDummy(row.PresentID);
        return null;
    }


    public static GameObject GetDummy(uint presentid)
    {
        XEntityPresentation.RowData raw_data = XTableMgr.GetTable<XEntityPresentation>().GetItemID(presentid);
        if (raw_data == null) return null;
        string prefab = raw_data.Prefab;
        int n = prefab.LastIndexOf("_SkinnedMesh");
        int m = prefab.LastIndexOf("Loading");
        return n < 0 || m > 0 ?
            AssetDatabase.LoadAssetAtPath("Assets/Resources/Prefabs/" + prefab + ".prefab", typeof(GameObject)) as GameObject :
            AssetDatabase.LoadAssetAtPath("Assets/Editor/EditorResources/Prefabs/" + prefab.Substring(0, n) + ".prefab", typeof(GameObject)) as GameObject;
    }


    public static bool CheckPrefab(GameObject obj)
    {
        if (obj == null) return false;
        string path = AssetDatabase.GetAssetPath(obj);
        int last = path.LastIndexOf('.');
        string subfix = path.Substring(last, path.Length - last).ToLower();
        if (subfix != ".prefab")
        {
            EditorUtility.DisplayDialog("Confirm your selection.",
                "Please select a prefab file for this skill!",
                "Ok");
            return false;
        }
        return true;
    }


    public static string GetCfgFromSkp(string skp, string suffix = ".config")
    {
        skp = skp.Replace("/Resources/Table/", "/Editor/EditorResources/");
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
        return BuildPath(dictionary, _root + "/Table");
    }


}
