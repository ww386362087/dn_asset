using System.IO;
using UnityEditor;

/// <summary>
/// 在打包之后调用
/// </summary>
public class XPostBuild : XBuildArg
{
    [MenuItem("XBuild/OnBuild/OnPostBuild")]
    private static void TestPost()
    {
        OnPostBuild(EditorUserBuildSettings.activeBuildTarget);
        AssetDatabase.Refresh();
        XDebug.Log("PostBuild Finish");
    }


    public static void OnPostBuild(BuildTarget target)
    {
        MoveFolder(ai_editor);
        MoveFolder(ai_xeditor);
    }

    public static void MoveFolder(string folder)
    {
        string src = Path.Combine(temp_dir, folder);
        string dest = Path.Combine(data_dir, folder);
        if (Directory.Exists(src))
        {
            Directory.Move(src, dest);
        }
        else
        {
            XDebug.Log("not exist dir ", src);
        }
    }

}
