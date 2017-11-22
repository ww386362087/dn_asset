using UnityEditor;
using System.IO;

public enum TPlatform
{
    None,
    Win32,
    iOS,
    Android,
}


public class XBuild 
{

    public static string Macro
    {
        get
        {
            string str = "TEST";
            string path = typeof(AssetDatabase).Module.FullyQualifiedName;
            string backup = path.Replace("UnityEditor.dll", "Tag.txt");
            if (File.Exists(backup)) str += ";Inject;";
            return str;
        }
    }


    [MenuItem("XBuild/Build/Android")]
    public static void BuildAndroid()
    {
        XBuildEditor.SwitchPlatForm(TPlatform.Android);
        XBuildEditor.Build(false);
        AssetDatabase.Refresh();
    }

    [MenuItem("XBuild/Fast-Build/Android")]
    public static void FastBuildAndroid()
    {
        XBuildEditor.SwitchPlatForm(TPlatform.Android);
        XBuildEditor.Build(true);
        AssetDatabase.Refresh();
    }

    [MenuItem("XBuild/Build/Win32")]
    public static void BuildWin32()
    {
        XBuildEditor.SwitchPlatForm(TPlatform.Win32);
        XBuildEditor.Build(false);
        AssetDatabase.Refresh();
    }

    [MenuItem("XBuild/Fast-Build/Win32")]
    public static void FastBuildWin32()
    {
        XBuildEditor.SwitchPlatForm(TPlatform.Win32);
        XBuildEditor.Build(true);
        AssetDatabase.Refresh();
    }

    [MenuItem(@"XBuild/BuildPanel")]
    private static void BuildWindow()
    {
        EditorWindow.GetWindow(typeof(XBuildEditor));
    }

    [MenuItem("XBuild/Build/IOS")]
    public static void BuildIOS()
    {
        XBuildEditor.SwitchPlatForm(TPlatform.iOS);
        XBuildEditor.Build(false);
        AssetDatabase.Refresh();
    }

    [MenuItem("XBuild/Fast-Build/IOS")]
    public static void FastBuildIOS()
    {
        XBuildEditor.SwitchPlatForm(TPlatform.iOS);
        XBuildEditor.Build(true);
        AssetDatabase.Refresh();
    }

}
