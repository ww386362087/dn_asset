using UnityEngine;
using System.IO;

public class XConfig 
{

    public static string res_path;
    public static string cache_path;
    public static string stream_path;

    public static void Initial(LogLevel print, LogLevel file)
    {
        InitPath();
        if (!Directory.Exists(cache_path))
        {
            Directory.CreateDirectory(cache_path);
        }
        XDebug.Init(print, file);
    }


    public static void InitPath()
    {
        res_path = Application.dataPath + "/Resources/";
        cache_path = Application.temporaryCachePath + "/Log";
        stream_path = Application.streamingAssetsPath;
    }
}
