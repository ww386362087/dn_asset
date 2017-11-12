using System.IO;
using UnityEngine;


public class XBuildArg
{

    protected static string bpath
    {
        get { return Path.GetDirectoryName(Application.dataPath); }
    }


    protected static string temp_dir
    {
        get {  return Path.Combine(bpath, temp); }
    }

    protected static string data_dir
    {
        get { return Application.dataPath; }
    }


    protected static string temp = "BuildTemp/";

    protected static string ai_editor = "Behavior Designer/";

    protected static string ai_xeditor = "Scripts/Scene/AI/XEditor/";

    protected static string skill_xeditor = "Scripts/Scene/Skill/XEditor/";


}
