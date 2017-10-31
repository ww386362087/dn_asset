using UnityEditor;
using UnityEngine;
using AI.Runtime;
using System.CodeDom;
using System.IO;
using System.Text;
using System.CodeDom.Compiler;
using System.Collections.Generic;

public class AICodeMaker
{

    public static string unity_AI_path
    {
        get { return Application.dataPath + @"\Resources\Table\AITree\"; }
    }

    public static string unity_AI_code
    {
        get { return Application.dataPath + @"\Scripts\Scene\AI\Runtime\"; }
    }

    static List<string> maker_list = new List<string>();

    [MenuItem("Tools/MakeRuntimeCode")]
    private static void MakeRuntimeCode()
    {
        DirectoryInfo dir = new DirectoryInfo(unity_AI_path);
        FileInfo[] files = dir.GetFiles("*.txt");
        maker_list.Clear();
        for (int i = 0, max = files.Length; i < max; i++)
        {
            EditorUtility.DisplayProgressBar(string.Format("{0}-{1}/{2}", "ai auto code", (i + 1), max), files[i].FullName, (float)(i + 1) / max);
            string name = files[i].Name.Split('.')[0];
            string content = File.ReadAllText(files[i].FullName);
            Parse(content, name);
        }
        GenerateFactoryCode();
        EditorUtility.ClearProgressBar();
        AssetDatabase.Refresh();
        EditorUtility.DisplayDialog("AI Auto Code", "AI Code Make Finish!", "OK");
    }

    [MenuItem("Tools/CleanRuntimeCode")]
    private static void CleanRuntimeCode()
    {
        maker_list.Clear();
        DirectoryInfo dir = new DirectoryInfo(unity_AI_code);
        FileInfo[] files = dir.GetFiles();
        for (int i = 0, max = files.Length; i < max; i++)
        {
            File.Delete(files[i].FullName);
        }
        AssetDatabase.Refresh();
        EditorUtility.DisplayDialog("AI Auto Code", "AI Code Clean Finish!", "OK");
    }


    private static void Parse(string json, string name)
    {
        AIRuntimeTreeData data = AIRuntimeUtil.Parse(json, name);
        AIRuntimeTaskData task = data.task;
        ParseTask(task);
    }


    private static void ParseTask(AIRuntimeTaskData task)
    {
        if (task.mode == Mode.Custom)
        { //避免不同task 生成同一份代码 加速生成
            if (!maker_list.Contains(task.type))
            {
                GenerateTaskCode(task);
            }
        }
        if (task.children != null)
        {
            for (int i = 0, max = task.children.Count; i < max; i++)
            {
                ParseTask(task.children[i]);
            }
        }
    }


    private static void GenerateTaskCode(AIRuntimeTaskData task)
    {
        //声明代码的部分
        CodeCompileUnit compunit = new CodeCompileUnit();
        CodeNamespace sample = new CodeNamespace("AI.Runtime");
        //引用命名空间
        sample.Imports.Add(new CodeNamespaceImport("UnityEngine"));
        compunit.Namespaces.Add(sample);

        //在命名空间下添加一个类
        CodeTypeDeclaration wrapClass = new CodeTypeDeclaration("AIRuntime" + task.type + " : AIRunTimeBase");
        sample.Types.Add(wrapClass);

        //添加成员
        if (task.vars != null)
        {
            for (int i = 0, max = task.vars.Count; i < max; i++)
            {
                CodeTypeReference type = new CodeTypeReference(task.vars[i].type);
                CodeMemberField field = new CodeMemberField(type, task.vars[i].name);
                field.Attributes = MemberAttributes.Public;
                wrapClass.Members.Add(field);
            }
        }

        //为这个类添加一个方法   Init()
        CodeMemberMethod method = new CodeMemberMethod();
        method.Name = "Init";
        method.Attributes = MemberAttributes.Override | MemberAttributes.Public;
        method.Parameters.Add(new CodeParameterDeclarationExpression(typeof(AIRuntimeTaskData), "data"));
        method.ReturnType = new CodeTypeReference(typeof(void));//返回值
        AddState(method, "base.Init(data);");
        if (task.vars != null)
        {
            for (int i = 0, max = task.vars.Count; i < max; i++)
            {
                if (task.vars[i] is AITreeSharedVar)
                {
                    AITreeSharedVar var = task.vars[i] as AITreeSharedVar;
                 
                    if(!var.IsShared)
                    {
                        if (var.val != null)
                        {
                            string st = var.name + " = " + var.val + ";";
                            AddState(method, st);
                        }
                    }
                }
                else
                {
                    AIVar var = task.vars[i];
                    if (var.val != null)
                    {
                        string st = var.name + " = " + var.val + ";";
                        AddState(method, st);
                    }
                }
            }
        }

        //OnTick()
        CodeMemberMethod method2 = new CodeMemberMethod();
        method2.Name = "OnTick";
        method2.Attributes = MemberAttributes.Override | MemberAttributes.Public;
        method2.Parameters.Add(new CodeParameterDeclarationExpression(typeof(XEntity), "entity"));
        method2.ReturnType = new CodeTypeReference("AIRuntimeStatus");//返回值
        string state = "return AITreeImpleted." + task.type + "Update(entity";
        if (task.vars != null)
        {
            for (int i = 0, max = task.vars.Count; i < max; i++)
            {
                state += ", " + task.vars[i].name;

                if (task.vars[i] is AITreeSharedVar)
                {
                    AITreeSharedVar var = task.vars[i] as AITreeSharedVar;
                    if (var.IsShared)
                    {
                        string st = var.name + " = (" + var.type + ")_tree.GetVariable(\"" + var.BindName + "\"); ";
                        AddState(method2, st);
                    }
                }
            }
        }
        state += ");";
        method2.Statements.Add(new CodeSnippetStatement("\t\t\t" + state));
        wrapClass.Members.Add(method);
        wrapClass.Members.Add(method2);

        //output
        StringBuilder fileContent = new StringBuilder();
        using (StringWriter sw = new StringWriter(fileContent))
        {
            CodeDomProvider.CreateProvider("CSharp").GenerateCodeFromCompileUnit(compunit, sw, new CodeGeneratorOptions());
        }
        string filePath = unity_AI_code + "AIRuntime" + task.type + ".cs";
        if (File.Exists(filePath)) File.Delete(filePath);
        File.WriteAllText(filePath, fileContent.ToString());
        maker_list.Add(task.type);
    }

    private static void GenerateFactoryCode()
    {
        string[] sys = AIRunTimeTree.composites;

        CodeCompileUnit compunit = new CodeCompileUnit();
        CodeNamespace sample = new CodeNamespace("AI.Runtime");
        compunit.Namespaces.Add(sample);

        CodeTypeDeclaration wrapClass = new CodeTypeDeclaration("AIRuntimeFactory");
        sample.Types.Add(wrapClass);

        CodeMemberMethod method = new CodeMemberMethod();
        method.Name = "MakeRuntime";
        method.Attributes = MemberAttributes.Public | MemberAttributes.Static;
        method.Parameters.Add(new CodeParameterDeclarationExpression("AIRuntimeTaskData", "data"));
        method.Parameters.Add(new CodeParameterDeclarationExpression(typeof(AIRunTimeTree), "tree"));
        method.ReturnType = new CodeTypeReference("AIRunTimeBase");//返回值
        AddState(method, "AIRunTimeBase rst = null;");
        AddState(method, "switch (data.type)");
        AddState(method, "{");
        for (int i = 0, max = sys.Length; i < max; i++)
        {
            AddState(method, "\tcase \"" + sys[i] + "\":");
            AddState(method, "\t\trst = new AIRuntime" + sys[i] + "();");
            AddState(method, "\t\tbreak;");
        }
        for (int i = 0, max = maker_list.Count; i < max; i++)
        {
            AddState(method, "\tcase \"" + maker_list[i] + "\":");
            AddState(method, "\t\trst = new AIRuntime" + maker_list[i] + "();");
            AddState(method, "\t\tbreak;");
        }
        AddState(method, "}");
        AddState(method, "if (rst != null) { rst.SetTree(tree); rst.Init(data); }");
        AddState(method, "return rst;");

        wrapClass.Members.Add(method);

        //output
        StringBuilder fileContent = new StringBuilder();
        using (StringWriter sw = new StringWriter(fileContent))
        {
            CodeDomProvider.CreateProvider("CSharp").GenerateCodeFromCompileUnit(compunit, sw, new CodeGeneratorOptions());
        }
        string filePath = unity_AI_code + "AIRuntimeFactory.cs";
        if (File.Exists(filePath)) File.Delete(filePath);
        File.WriteAllText(filePath, fileContent.ToString());
        maker_list.Clear();
    }

    private static void AddState(CodeMemberMethod method, string state)
    {
        method.Statements.Add(new CodeSnippetStatement("\t\t\t" + state));
    }

}
