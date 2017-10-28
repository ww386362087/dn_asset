using UnityEngine;
using UnityEditor;
using System.IO;
using System.Collections.Generic;
using BehaviorDesigner.Runtime;

public class AIExport
{
    private const string Children = "Children";
    private const string RootTask = "RootTask";
    private static string[] nodes = {
        "NodeData",
        "ID",
        "AbortTypeabortType",
        "Instant",
        "Name"
    };

    [MenuItem("Assets/AI/Export")]
    public static void ExportSelect()
    {
        Object[] objects = Selection.GetFiltered(typeof(Object), SelectionMode.Assets);
        for (int i = 0, max = objects.Length; i < max; i++)
        {
            string path = AssetDatabase.GetAssetPath(objects[i]);
            FileInfo file = new FileInfo(path);
            Export(file);
        }
        AssetDatabase.Refresh();
        Debug.Log("Export Finish!");
    }

    [MenuItem("Tools/ExportAll")]
    public static void ExportAll()
    {
        string path = Application.dataPath + @"\Behavior Designer\AIData\";
        DirectoryInfo dir = new DirectoryInfo(path);
        FileInfo[] files = dir.GetFiles("*.asset");
        for (int i = 0, max = files.Length; i < max; i++)
        {
            Export(files[i]);
        }
        AssetDatabase.Refresh();
        Debug.Log("Export Finish!");
    }

    private static void Export(FileInfo file)
    {
        string name = file.Name.Split('.')[0];
        Debug.Log(name);
        string cont = File.ReadAllText(file.FullName);
        string tag1 = "JSONSerialization:";
        string tag2 = "fieldSerializationData:";
        int index1 = cont.IndexOf(tag1);
        int index2 = cont.IndexOf(tag2);
        string json = cont.Substring(index1, index2 - index1).Substring(tag1.Length).Trim();
        json = json.Substring(1, json.Length - 2);
        BuildJson(json, name);
    }

    private static void BuildJson(string json, string name)
    {
        json = json.Replace("\\t", "");
        json = json.Replace("\\n", "");
        json = json.Replace("\\", "");
        //Debug.Log(json);
        var obj = MiniJSON.Deserialize(json) as Dictionary<string, object>;
        var root = obj[RootTask] as Dictionary<string, object>;
        object ob = SimplyTask(root);
        json = MiniJSON.Serialize(ob);
        Build(json, name);
    }


    private static void Build(string json, string name)
    {
        string path = XEditorLibrary.Ai + name + ".txt";
        File.WriteAllText(path, json);
    }

    private static object SimplyTask(Dictionary<string, object> dic)
    {
        for (int i = 0, max = nodes.Length; i < max; i++)
        {
            if (dic.ContainsKey(nodes[i])) dic.Remove(nodes[i]);
        }
        if (dic.ContainsKey(Children))
        {
            List<object> list = dic[Children] as List<object>;
            for (int i = 0, max = list.Count; i < max; i++)
            {
                Dictionary<string, object> d = list[i] as Dictionary<string, object>;
                SimplyTask(d);
            }
        }
        foreach (var item in dic)
        {
            if (item.Key.StartsWith("Shared"))
                SimplyVar(item.Value as Dictionary<string, object>);
        }
        return dic;
    }

    private static void SimplyVar(Dictionary<string, object> dic)
    {
        string key = "Name";
        if (dic.ContainsKey(key)) dic.Remove(key);
    }
}
