using UnityEngine;

public class XConfig 
{

    public static string res_path { get; set; }

    public static void Initial()
    {
        res_path = Application.dataPath + "/Resources/";
    }

}
