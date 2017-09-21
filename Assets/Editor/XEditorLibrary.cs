using XTable;
using UnityEngine;
using UnityEditor;


internal class XEditorLibrary
{

    /// <summary>
    /// 帧率 value = 30
    /// </summary>
    public const float FPS = 30.0f;

    public static GameObject GetDummy(uint statictid)
    {
        XEntityStatistics.RowData row = XTableMgr.GetTable<XEntityStatistics>().GetByID((int)statictid);
        if (row != null)
        {
            XEntityPresentation.RowData raw_data = XTableMgr.GetTable<XEntityPresentation>().GetItemID(row.PresentID);
            if (raw_data == null) return null;
            string prefab = raw_data.Prefab;
            int n = prefab.LastIndexOf("_SkinnedMesh");
            int m = prefab.LastIndexOf("Loading");
            return n < 0 || m > 0 ?
                AssetDatabase.LoadAssetAtPath("Assets/Resources/Prefabs/" + prefab + ".prefab", typeof(GameObject)) as GameObject :
                AssetDatabase.LoadAssetAtPath("Assets/Editor/EditorResources/Prefabs/" + prefab.Substring(0, n) + ".prefab", typeof(GameObject)) as GameObject;
        }
        return null;
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


}
