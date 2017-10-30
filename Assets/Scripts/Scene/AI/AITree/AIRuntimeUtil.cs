using BehaviorDesigner.Runtime;
using System.Collections.Generic;
using UnityEngine;

namespace AI.Runtime
{

    public class AIRuntimeUtil
    {
        static string Children = "Children";

        public static AIRuntimeTreeData Load(string name)
        {
            TextAsset ta = XResources.Load<TextAsset>("AITree/" + name, AssetType.Text);
            return Parse(ta.text, name);
        }

        public static AIRuntimeTreeData Parse(string json, string name)
        {
            XDebug.Log(json);
            var obj = MiniJSON.Deserialize(json) as Dictionary<string, object>;
            var root = obj as Dictionary<string, object>;
            AIRuntimeTreeData tree = new AIRuntimeTreeData();

            //Variables
            if (root.ContainsKey("Variables"))
            {
                var list = root["Variables"] as List<object>;
                for (int i = 0, max = list.Count; i < max; i++)
                {
                    if (tree.vars == null) tree.vars = new List<AITreeVar>();
                    AITreeVar v = ParseTreeVar(list[i] as Dictionary<string, object>);
                    tree.vars.Add(v);
                }
            }
            
            //task
            var dic_task = root["RootTask"] as Dictionary<string, object>;
            AIRuntimeTaskData task = ParseTask(dic_task);
            tree.task = task;
          
            return tree;
        }


        private static AITreeVar ParseTreeVar(Dictionary<string, object> arg)
        {
            return new AITreeVar()
            {
                isShared = bool.Parse(arg["IsShared"].ToString()),
                type = arg["Type"].ToString(),
                name = arg["Name"].ToString()
            };
        }

        private static Mode Type2Mode(string type)
        {
            if (type == "Sequence") return Mode.Sequence;
            if (type == "Selector") return Mode.Selector;
            if (type == "Inverter") return Mode.Inverter;
            return Mode.Custom;
        }

        private static AIRuntimeTaskData ParseTask(Dictionary<string, object> arg)
        {
            AIRuntimeTaskData t = new AIRuntimeTaskData();
            var type = arg["Type"].ToString();
            t.type = ParseType(type);
            t.mode = Type2Mode(t.type);
            foreach (var item in arg)
            {
                if (item.Key.StartsWith("Shared"))
                {
                    try
                    {
                        AIVar v = ParseSharedVar(item.Key, item.Value as Dictionary<string, object>);
                        if (t.vars == null) t.vars = new List<AIVar>();
                        t.vars.Add(v);
                    }
                    catch (System.Exception e)
                    {
                        Debug.LogError(item.Key + " ************ " + e.StackTrace);
                    }
                }
                else
                {
                    AIVar v = ParseCustomVar(item.Key, item.Value);
                    if (v != null)
                    {
                        if (t.vars == null) t.vars = new List<AIVar>();
                        t.vars.Add(v);
                    }
                }
            }
            if (arg.ContainsKey(Children))
            {
                List<object> list = arg[Children] as List<object>;
                for (int i = 0, max = list.Count; i < max; i++)
                {
                    Dictionary<string, object> child = list[i] as Dictionary<string, object>;
                    if (t.children == null) t.children = new List<AIRuntimeTaskData>();
                    AIRuntimeTaskData tt = ParseTask(child);
                    t.children.Add(tt);
                }
            }
            return t;
        }

        private static AITreeSharedVar ParseSharedVar(string key, Dictionary<string, object> dic)
        {
            AITreeSharedVar v = new AITreeSharedVar();
            v.name = key;
            dic.TryGetValue("Name",out v.bindName);
            dic.TryGetValue("IsShared",out v.isShared);
            v.type = ParseType(dic["Type"].ToString());
            if (key.StartsWith(v.type)) v.name = key.Replace(v.type, string.Empty);
            v.type = v.type.Replace("Shared", string.Empty);
            string[] arr = { "Float", "Int", "Bool", "String" };
            string[] arr2 = { "System.Single", "System.Int32", "System.Boolean", " System.String" };
            for (int i = 0, max = arr.Length; i < max; i++)
            {
                if (v.type == arr[i])
                    v.type = arr2[i];
            }
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


        private static AIVar ParseCustomVar(string key,object val)
        {
            string[] arr = { "Single", "Int32", "Boolean", "String" };
            for(int i=0,max= arr.Length;i<max;i++)
            {
                if(key.StartsWith(arr[i]))
                {
                    AIVar v = new AIVar();
                    v.type = "System." + arr[i];
                    v.name = key.Replace(arr[i], string.Empty);
                    v.val = val;
                    return v;
                }
            }
            return null;
        }

        private static string ParseType(string str)
        {
            int index = str.LastIndexOf(".");
            return index == -1 ? str : str.Substring(index + 1);
        }
    }

}