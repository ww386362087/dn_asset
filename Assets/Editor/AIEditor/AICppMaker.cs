using System.IO;
using System.Collections.Generic;
using UnityEngine;
using AI.Runtime;
using AI;

/// <summary>
/// author: alexpeng
/// date:   2017-12-29
/// 这个类主要用于生成ai的c++代码
/// </summary>


public class AICppMaker
{

    static string path_ori_h, path_ori_c, path_dest;
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
        File.WriteAllText(dest_c, ai_n_c);
    }


    private static void GenerateFactoryCode()
    {
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
        }
        return t;
    }


}
