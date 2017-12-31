using System.IO;
using UnityEngine;
using AI.Runtime;

/// <summary>
/// author: alexpeng
/// date:   2017-12-29
/// 这个类主要用于生成ai的c++代码
/// </summary>


public class AICppMaker
{

    static string path_ori_h, path_ori_c, path_dest,path_fact;
    static string ai_h, ai_c;

    public static void GenerateTaskCode(AIRuntimeTaskData task)
    {
        Init(task.type);
        Generate_head_file(task);
        Generate_cpp_file(task);
    }


    private static void Init(string name)
    {
        if (string.IsNullOrEmpty(path_ori_h))
        {
            var dir = Path.GetDirectoryName(Application.dataPath);
            path_ori_h = Path.Combine(dir, "Shell/AITemplate.h");
            path_ori_c = Path.Combine(dir, "Shell/AITemplate.cpp");
            path_dest = Path.Combine(dir, "tools_proj/XCPP/GameCore/runtime");
            path_fact = Path.Combine(dir, "tools_proj/XCPP/GameCore/AIFactory.cpp");
            ai_h = File.ReadAllText(path_ori_h);
            ai_c = File.ReadAllText(path_ori_c);
        }
    }

    private static void Generate_head_file(AIRuntimeTaskData task)
    {
        string ai_n_h = ai_h;
        ai_n_h = ai_n_h.Replace("[*Name*]", task.type);

        string s_var = string.Empty;
        if (task.vars != null)
        {
            for (int i = 0, max = task.vars.Count; i < max; i++)
            {
                s_var += TransCppType(task.vars[i].type) + " " + task.vars[i].name + ";\n\t";
            }
        }
        ai_n_h = ai_n_h.Replace("[*Arg*]", s_var);
        string dest_h = Path.Combine(path_dest, "AIRuntime" + task.type + ".h");
        File.WriteAllText(dest_h, ai_n_h);
    }


    private static void Generate_cpp_file(AIRuntimeTaskData task)
    {
        string ai_n_c = ai_c;
        ai_n_c = ai_n_c.Replace("[*Name*]", task.type);
        string dest_c = Path.Combine(path_dest, "AIRuntime" + task.type + ".cpp");

        if (task.vars != null)
        {
            string rel_str = string.Empty;
            for (int i = 0, max = task.vars.Count; i < max; i++)
            {
                AIVar var = task.vars[i];
                if (var.type == "GameObject" || var.type == "Transform")
                {
                    rel_str += "delete " + var.name + ";";
                }
            }
            ai_n_c = ai_n_c.Replace("[*arg-0*]", rel_str);

            //Init(AITaskData* data)
            string init_str = string.Empty;
            for (int i = 0, max = task.vars.Count; i < max; i++)
            {
                string val = "data->vars[\"" + task.vars[i].name + "\"]->val";
                string op = GetOpByType(task.vars[i].type, val);
                if (!string.IsNullOrEmpty(op))
                {
                    if (task.vars[i] is AITreeSharedVar)
                    {
                        AITreeSharedVar var = task.vars[i] as AITreeSharedVar;
                        if (!var.IsShared)
                        {
                            init_str += var.name + " = " + op + " \n\t";
                        }
                    }
                    else
                    {
                        init_str += task.vars[i].name + " = " + op + " \n\t";
                    }
                }
            }
            ai_n_c = ai_n_c.Replace("[*arg-1*]", init_str);

            //OnTick()
            string tick_str = string.Empty;
            string imp_str = string.Empty;
            if (task.vars != null)
            {
                for (int i = 0, max = task.vars.Count; i < max; i++)
                {
                    imp_str += "," + task.vars[i].name;
                    if (task.vars[i] is AITreeSharedVar)
                    {
                        AITreeSharedVar var = task.vars[i] as AITreeSharedVar;
                        if (var.IsShared)
                        {
                            tick_str += var.name + " = " + GetTreeVarCode(var.type, var.BindName) + "\n\t";
                        }
                    }
                }
            }
            ai_n_c = ai_n_c.Replace("[*arg-2*]", tick_str);
            ai_n_c = ai_n_c.Replace("[*tickarg*]", imp_str);
        }
        else
        {
            ai_n_c = ai_n_c.Replace("[*arg-0*]", "");
            ai_n_c = ai_n_c.Replace("[*arg-1*]", "");
            ai_n_c = ai_n_c.Replace("[*arg-2*]", "");
            ai_n_c = ai_n_c.Replace("[*tickarg*]", "");
        }

        File.WriteAllText(dest_c, ai_n_c);
    }
    
    public static void GenerateFactoryCode()
    {
        var dirr = Path.GetDirectoryName(Application.dataPath);
        path_dest = Path.Combine(dirr, "tools_proj/XCPP/GameCore/runtime");
        path_fact = Path.Combine(dirr, "tools_proj/XCPP/GameCore/AIFactory.cpp");

        string txt = File.ReadAllText(path_fact);
        DirectoryInfo dir = new DirectoryInfo(path_dest);

        string str_h = string.Empty, str_c = string.Empty;
        FileInfo[] files = dir.GetFiles("*.h");
        for (int i = 0, max = files.Length; i < max; i++)
        {
            string name = files[i].Name;
            string tname = name.Substring(9, name.Length - 11);
            if (!txt.Contains(name))
            {
                str_h += "\n#include \"runtime/" + name + "\"";
                str_c += "\n\telse if (data->type == \"" + tname + "\")";
                str_c += "\n\t{";
                str_c += "\n\t\trst = new AIRuntime" + tname + "();";
                str_c += "\n\t}";
            }
        }
        string head_h = "AIFactory.h";
        int index_h = txt.IndexOf(head_h) + head_h.Length + 2;
        txt = txt.Insert(index_h, str_h);

        string cont_h = "if (rst != NULL)";
        int index_c = txt.IndexOf(cont_h) - 2;
        txt = txt.Insert(index_c, str_c);
        File.WriteAllText(path_fact, txt);
    }

    private static string GetTreeVarCode(string type,string bindname)
    {
        string t = type;
        switch (type)
        {
            case "GameObject":
                t = "_tree->GetGoVariable(\"" + bindname + "\");";
                break;
            case "System.Boolean":
            case "bool":
                t = "_tree->GetBoolVariable(\"" + bindname + "\");";
                break;
            case "System.UInt32":
            case "uint":
                t = "_tree->GetUintVariable(\"" + bindname + "\");";
                break;
            case "System.Single":
            case "float":
                t = "_tree->GetFloatVariable(\"" + bindname + "\");";
                break;
            case "System.Int32":
            case "int":
                t = "_tree->GetIntVariable(\"" + bindname + "\");";
                break;
            default:
                XDebug.LogError("make cpp code err, not release type:" + type);
                break;
        }
        return t;
    }
    
    private static string GetOpByType(string type,string obj)
    {
        string t = type;
        switch (type)
        {
            case "Vector3":
                t = "Obj2Vector(" + obj + ");";
                break;
            case "System.String":
            case "string":
                t = obj + ".get<std::string>();";
                break;
            case "System.Int32":
            case "int":
                t = "(int)" + obj + ".get<double>();";
                break;
            case "System.UInt32":
            case "uint":
                t = "(uint)" + obj + ".get<double>();";
                break;
            case "System.Single":
            case "float":
                t = "(float)"+obj + ".get<double>();";
                break;
            case "System.Boolean":
            case "bool":
                t = obj + ".get<bool>();";
                break;
            case "GameObject":
            case "Transform":
                t = string.Empty;
                XDebug.LogWarning("make cpp code err, gameobject or transform can't initial by editor ");
                break;
            default:
                t = obj + ";";
                break;
        }
        return t;
    }

    private static string TransCppType(string type)
    {
        string t = type;
        switch (t)
        {
            case "System.String":
                t = "std::string";
                break;
            case "System.Boolean":
                t = "bool";
                break;
            case "System.Single":
                t = "float";
                break;
            case "System.Int32":
                t = "int";
                break;
            case "System.UInt32":
                t = "uint";
                break;
            case "GameObject":
                t = "GameObject*";
                break;
            case "Transform":
                t = "Transform*";
                break;
        }
        return t;
    }


}
