using System.IO;
using UnityEditor;

/// <summary>
/// 在打包之后调用
/// </summary>
public class XPostBuild : XBuildArg
{

    /*
    [MenuItem("XBuild/OnBuild/OnPostBuild")]
    private static void TestPost()
    {
        OnPostBuild(EditorUserBuildSettings.activeBuildTarget);
        XDebug.Log("PostBuild Finish");
    }
    */

    public static void OnPostBuild(bool fast, BuildTarget target)
    {
        MoveFolder(ai_editor);
        MoveFolder(ai_xeditor);
        MoveFolder(skill_xeditor);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }

    public static void MoveFolder(string folder)
    {
        string src = Path.Combine(temp_dir, folder);
        string dest = Path.Combine(data_dir, folder);
        if (Directory.Exists(src))
        {  
            // 创建一个深的空父目录
            if (!Directory.Exists(dest)) Directory.CreateDirectory(dest);
            Directory.Delete(dest);

            Directory.Move(src, dest);
        }
        else
        {
            XDebug.LogError("not exist dir ", src);
        }
    }

}
