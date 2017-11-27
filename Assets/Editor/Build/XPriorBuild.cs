using UnityEditor;
using System.IO;


/// <summary>
/// 在打包之前调用
/// </summary>
public class XPriorBuild : XBuildArg
{
    /*
    [MenuItem("XBuild/OnBuild/OnPriorBuild")]
    private static void TestPrior()
    {
        OnPriorBuild(EditorUserBuildSettings.activeBuildTarget);
        XDebug.Log("PriorBuild Finish");
    }
    */


    public static void OnPriorBuild(bool fast, BuildTarget target)
    {
        if (Directory.Exists(temp_dir))
            Directory.Delete(temp_dir, true);
        Directory.CreateDirectory(temp_dir);

        MoveFolder(ai_editor);
        MoveFolder(ai_xeditor);
        MoveFolder(skill_xeditor);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }


    public static void MoveFolder(string folder)
    {
        string src = Path.Combine(data_dir, folder);
        string dest = Path.Combine(temp_dir, folder);
        if (Directory.Exists(src))
        {
            // 创建一个深的空父目录
            if (!Directory.Exists(dest)) Directory.CreateDirectory(dest);
            Directory.Delete(dest);
            
            Directory.Move(src, dest);
        }
        else
        {
            XDebug.LogError("not exit dir ", src);
        }
    }

}
