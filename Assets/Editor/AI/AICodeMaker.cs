using UnityEditor;
using UnityEngine;
using AI.Runtime;
using System.CodeDom;
using System.IO;
using System.Text;
using System.CodeDom.Compiler;

public class AICodeMaker
{

    [MenuItem("Tools/MakeRuntimeCode")]
    private static void MakeRuntimeCode()
    {
        string path = Application.dataPath + @"\Behavior Designer\AIData\";
        DirectoryInfo dir = new DirectoryInfo(path);
        FileInfo[] files = dir.GetFiles("*.asset");
        for (int i = 0, max = files.Length; i < max; i++)
        {
            Make(files[i].Name.Split('.')[0]);
        }
    }

    private static void Make(string name)
    {
        TextAsset ta = Resources.Load<TextAsset>("Table/AITree/" + name);
        Parse(ta.text, name);
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
        {
            GenerateCode(task);
        }
        if (task.children != null)
        {
            for (int i = 0, max = task.children.Count; i < max; i++)
            {
                ParseTask(task.children[i]);
            }
        }
    }


    private static void GenerateCode(AIRuntimeTaskData task)
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

        //添加属性
        if (task.vars != null)
        {
            for (int i = 0, max = task.vars.Count; i < max; i++)
            {
                CodeMemberField field = new CodeMemberField(task.vars[i].type, task.vars[i].name);
                field.Attributes = MemberAttributes.Public;
                wrapClass.Members.Add(field);
            }
        }

        //为这个类添加一个方法   public override Init 方法名(string str);
        CodeMemberMethod method = new CodeMemberMethod();
        method.Name = "OnTick";
        method.Attributes = MemberAttributes.Override | MemberAttributes.Public;
        method.Parameters.Add(new CodeParameterDeclarationExpression(typeof(XEntity), "entity"));
        method.ReturnType = new CodeTypeReference("AIRuntimeStatus");//返回值
        string state = "return AITreeImpleted." + task.type + "Update(entity";
        if (task.vars != null)
        {
            for (int i = 0, max = task.vars.Count; i < max; i++)
            {
                state += ", " + task.vars[i].name;
            }
        }
        state += ");";
        method.Statements.Add(new CodeSnippetStatement("\t\t\t" + state));

        wrapClass.Members.Add(method);

        //output
        StringBuilder fileContent = new StringBuilder();
        using (StringWriter sw = new StringWriter(fileContent))
        {
            CodeDomProvider.CreateProvider("CSharp").GenerateCodeFromCompileUnit(compunit, sw, new CodeGeneratorOptions());
        }
        string filePath = Application.dataPath + @"/Scripts/Scene/AI/Code/AIRuntime" + task.type + ".cs";
        if (File.Exists(filePath)) File.Delete(filePath);
        File.WriteAllText(filePath, fileContent.ToString());
        AssetDatabase.Refresh();
    }

}
