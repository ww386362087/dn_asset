using System.Collections.Generic;
using UnityEngine;

namespace AI.Runtime
{

    public class AIRuntimeUtil
    {
        static string Children = "Children";
        static string Type = "Type";

        public static AIRuntimeTreeData Load(string name)
        {
            TextAsset ta = XResources.Load<TextAsset>("Table/AITree/" + name, AssetType.Text);
            return Parse(ta.text, name);
        }

        public static AIRuntimeTreeData Parse(string json, string name)
        {
            //  XDebug.Log(json);
            var obj = MiniJSON.Json.Deserialize(json) as Dictionary<string, object>;
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

        private static AIRuntimeTaskData ParseTask(Dictionary<string, object> task)
        {
            AIRuntimeTaskData t = new AIRuntimeTaskData();
            foreach (var item in task)
            {
                if (item.Key == Type)
                {
                    t.type = task[Type].ToString();
                    t.mode = Type2Mode(t.type);
                }
                else if (item.Key == Children)
                {
                    List<object> list = task[Children] as List<object>;
                    for (int i = 0, max = list.Count; i < max; i++)
                    {
                        Dictionary<string, object> child = list[i] as Dictionary<string, object>;
                        if (t.children == null) t.children = new List<AIRuntimeTaskData>();
                        AIRuntimeTaskData tt = ParseTask(child);
                        t.children.Add(tt);
                    }
                }
                else if (item.Value is Dictionary<string, object>)
                {
                    AIVar v = ParseSharedVar(item.Key, item.Value as Dictionary<string, object>);
                    if (t.vars == null) t.vars = new List<AIVar>();
                    t.vars.Add(v);
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
            return t;
        }

        private static AITreeSharedVar ParseSharedVar(string key, Dictionary<string, object> dic)
        {
            AITreeSharedVar v = new AITreeSharedVar();
            v.name = key;
            dic.TryGetValue("Name", out v.bindName);
            dic.TryGetValue("IsShared", out v.isShared);
            v.type = TransfType(dic["Type"]);
            v.name = key;
            foreach (var item in dic)
            {
                if (item.Key.Contains("Value"))
                {
                    ParseVarValue(v, item.Value);
                    break;
                }
            }
            return v;
        }


        private static AIVar ParseCustomVar(string key, object val)
        {
            string[] arr = { "Single", "Int32", "Boolean", "String", "Vector3", "Vector2", "Vector4", "GameObject", "Transform" };
            for (int i = 0, max = arr.Length; i < max; i++)
            {
                if (key.StartsWith(arr[i]))
                {
                    AIVar v = new AIVar();
                    v.type = i <= 4 ? "System." + arr[i] : arr[i];
                    v.name = key;
                    ParseVarValue(v, val);
                    return v;
                }
            }
            return null;
        }

        private static void ParseVarValue(AIVar var, object val)
        {
            switch (var.type)
            {
                case "System.Boolean":
                    var.val = val;
                    break;
                case "System.String":
                    var.val = val.ToString().Replace("\n", "").Replace("\t", "").Replace("\r", "");
                    break;
                case "System.Single":
                    var.val = float.Parse(val.ToString());
                    break;
                case "System.Int32":
                    var.val = int.Parse(val.ToString());
                    break;
                case "Vector3":
                case "Vector2":
                case "Vector4":
                    var.val = ParseVector(val.ToString());
                    break;
                default:
                    var.val = val;
                    break;
            }
        }

        private static string TransfType(object type)
        {
            string[] arr = { "float", "Int32", "bool", "string" };
            string[] arr2 = { "System.Single", "System.Int32", "System.Boolean", "System.String" };
            for (int i = 0, max = arr.Length; i < max; i++)
            {
                if (type.Equals(arr[i]))
                    return arr2[i];
            }
            return type.ToString();
        }

        private static object ParseVector(string str)
        {
            str = str.Trim().Replace("(", string.Empty).Replace(")", string.Empty);
            string[] ss = str.Split(',');
            int cnt = ss.Length;
            float[] arr = new float[cnt];
            for (int i = 0; i < cnt; i++)
            {
                arr[i] = float.Parse(ss[i]);
            }
            if (cnt == 2) return new Vector2(arr[0], arr[1]);
            if (cnt == 3) return new Vector3(arr[0], arr[1], arr[2]);
            if (cnt == 4) return new Vector4(arr[0], arr[1], arr[2], arr[3]);
            throw new System.Exception("Error vector format");
        }

    }

}