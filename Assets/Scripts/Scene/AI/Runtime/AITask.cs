using BehaviorDesigner.Runtime;
using System.Collections.Generic;
using UnityEngine;

public struct ShareVar
{
    public string type;
    public string name;
    public object val;
}


public class AITask  
{
    public string Name;
    public string Type;
    public List<ShareVar> vars;
    public List<AITask> children;
}


public class AITaskUtil
{
    static string Children = "Children";
    
    public static AITask Load(string name)
    {
        TextAsset ta = XResources.Load<TextAsset>("AITree/"+name, AssetType.Text);
        return Parse(ta.text, name);
    }

    private static AITask Parse(string json, string name)
    {
        XDebug.Log(json);
        var obj = MiniJSON.Deserialize(json) as Dictionary<string, object>;
        var root = obj as Dictionary<string, object>;
        return ParseTask(root);
    }

    private static AITask ParseTask(Dictionary<string, object> arg)
    {
        AITask t = new AITask();
        t.Name = arg["Name"].ToString();
        var type = arg["Type"].ToString();
        t.Type = ParseType(type);
        foreach (var item in arg)
        {
            if (item.Key.StartsWith("Shared"))
            {
                ShareVar v = ParseVar(item.Key, item.Value as Dictionary<string, object>);
                if (t.vars == null) t.vars = new List<ShareVar>();
                t.vars.Add(v);
            }
        }
        if (arg.ContainsKey(Children))
        {
            List<object> list = arg[Children] as List<object>;
            for (int i = 0, max = list.Count; i < max; i++)
            {
                Dictionary<string, object> child = list[i] as Dictionary<string, object>;
                if (t.children == null) t.children = new List<AITask>();
                AITask tt = ParseTask(child);
                t.children.Add(tt);
            }
        }
        return t;
    }


    private static ShareVar ParseVar(string key, Dictionary<string, object> dic)
    {
        ShareVar v = new ShareVar();
        v.name = key;
        v.type = ParseType(dic["Type"].ToString());
        foreach (var item in dic)
        {
            if (item.Key.Contains("Value"))
            {
                v.val = item.Value;
                break;
            }
        }
        return v;
    }


    private static string ParseType(string str)
    {
        int index = str.LastIndexOf(".");
        return index == -1 ? str : str.Substring(index + 1);
    }

}
