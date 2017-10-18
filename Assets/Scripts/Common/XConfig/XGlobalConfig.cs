using UnityEngine;

public class XGlobalConfig 
{

    public static readonly char[] SequenceSeparator = new char[] { '=' };
    public static readonly char[] ListSeparator = new char[] { '|' };
    public static readonly char[] AllSeparators = new char[] { '|', '=' };
    public static readonly char[] SpaceSeparator = new char[] { ' ' };
    public static readonly char[] TabSeparator = new char[] { ' ', '\t' };

    public static string res_path { get; set; }

    public static void Initial()
    {
        res_path = Application.dataPath + "/Resources/";
    }

}
