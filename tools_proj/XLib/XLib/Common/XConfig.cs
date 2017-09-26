using UnityEngine;
using System.IO;

public class XConfig 
{

    public static string res_path;
    public static string cache_path;

    public static void Initial(LogLevel print, LogLevel file)
    {
        res_path = Application.dataPath + "/Resources/";
        cache_path = Application.temporaryCachePath + "/Log";

        if (!Directory.Exists(cache_path))
        {
            Directory.CreateDirectory(cache_path);
        }
        XDebug.Init(print, file);
    }

}
