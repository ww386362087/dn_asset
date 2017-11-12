using Mono.Cecil;
using Mono.Cecil.Cil;
using System;
using System.IO;
using System.Reflection;
using UnityEditor;
using UnityEngine;

[InitializeOnLoad]
public class LogRedirect
{
    static LogRedirect()
    {
        SetSccriptDefine();
#if Inject
        AssetDatabase.onDoubleClick -= OnConsoleDoubleClick;
        AssetDatabase.onDoubleClick += OnConsoleDoubleClick;
#endif
    }
    
    static void OnConsoleDoubleClick(int row)
    {
        /*
           LogEntry log = new LogEntry();
           LogEntries.GetEntryInternal(row, log);
           OnHandleCondition(log.condition);
        */

        Assembly unityEditorAssembly = Assembly.GetAssembly(typeof(EditorWindow));
        Type logEntriesType = unityEditorAssembly.GetType("UnityEditorInternal.LogEntries");
        var s_LogEntriesGetEntry = logEntriesType.GetMethod("GetEntryInternal", BindingFlags.Static | BindingFlags.Public);
        Type logEntryType = unityEditorAssembly.GetType("UnityEditorInternal.LogEntry");
        var s_LogEntry = Activator.CreateInstance(logEntryType);
        var s_LogEntryCondition = logEntryType.GetField("condition", BindingFlags.Instance | BindingFlags.Public);
        s_LogEntriesGetEntry.Invoke(null, new object[] { row, s_LogEntry });
        string condition = s_LogEntryCondition.GetValue(s_LogEntry) as string;
        OnHandleCondition(condition);
    }


    private static void OnHandleCondition(string condition)
    {
        string[] lines = condition.Split('\n');
        //第一行是打印的内容 第二行是UnityEngine.Debug:Log 
        bool isXDebug = lines[2].Trim().StartsWith("XDebug");
        if (isXDebug)
        {
            for (int i = 3, max = lines.Length; i < max; i++)
            {
                if (!lines[i].StartsWith("XDebug"))
                {
                    //eg: "GameEnine:OnApplicationQuit() (at Assets/Scripts/Main/GameEnine.cs:54)"
                    var str = lines[i];
                    int start = str.LastIndexOf(":");
                    int end = str.LastIndexOf(")");
                    int line = -1;
                    int.TryParse(str.Substring(start + 1, end - start - 1), out line);
                    end = start;
                    start = str.IndexOf("at ");
                    var mono = str.Substring(start + 3, end - start - 3);
                    if (!string.IsNullOrEmpty(mono) && line > 0) AssetDatabase.OpenAsset(AssetDatabase.LoadAssetAtPath<MonoScript>(mono), line);
                    break;
                }
            }
        }
    }
    

    [MenuItem("Tools/Inject-Log", false, 1)]
    private static void InjectUnityEditor()
    {
        string unityEditorPath = typeof(AssetDatabase).Module.FullyQualifiedName;
        if (MakeInjectTag())
        {
            AssemblyDefinition ad = GetAssemblyDefination(unityEditorPath);
            TypeDefinition console = FindAssemlyType(ad, "ConsoleWindow");
            TypeDefinition asset = FindAssemlyType(ad, "AssetDatabase");
            if (console != null && asset != null)
            {
                /* 注入内容如下:
                 * public static Action<int> onDoubleClick;
                 * 
                 * public static void OnDoubleClick(int row)
                 * {
                 *   if (onDoubleClick != null)
                 *   {
                 *       onDoubleClick(row);
                 *   }
                 * }
                 */

                //public static Action<int> onDoubleClick
                var action = new FieldDefinition("onDoubleClick", Mono.Cecil.FieldAttributes.Public | Mono.Cecil.FieldAttributes.Static, ad.MainModule.ImportReference(typeof(Action<int>)));
                asset.Fields.Add(action);

                //给ConsoleWindow类添加一个无返回值一个int类型形参row的函数OnDoubleClick
                var method = new MethodDefinition("OnDoubleClick", Mono.Cecil.MethodAttributes.Public, ad.MainModule.TypeSystem.Void);
                method.Parameters.Add(new ParameterDefinition("row", Mono.Cecil.ParameterAttributes.None, ad.MainModule.TypeSystem.Int32));
                asset.Methods.Add(method);

                //给OnDoubleClick函数添加函数体
                var ilProcessor = method.Body.GetILProcessor();
                var il1 = ilProcessor.Create(OpCodes.Ldsfld, action);
                var il2 = ilProcessor.Create(OpCodes.Callvirt, ad.MainModule.ImportReference(typeof(Action<int>).GetMethod("Invoke", BindingFlags.Instance | BindingFlags.Public)));
                method.Body.Instructions.Add(il1);
                method.Body.Instructions.Add(il1);
                method.Body.Instructions.Add(ilProcessor.Create(OpCodes.Ldarg_1));
                method.Body.Instructions.Add(il2);
                method.Body.Instructions.Add(ilProcessor.Create(OpCodes.Ret));
                ilProcessor.InsertAfter(method.Body.Instructions[1], ilProcessor.Create(OpCodes.Brfalse, FindLast(method)));

                //在OnGUI函数中调用LogEntries.RowGotDoubleClicked函数的后面添加对OnDoubleClick函数的调用
                var ongui = GetMethod(console, "OnGUI");
                var onguiProcess = ongui.Body.GetILProcessor();
                var clickRef = method.GetElementMethod();
                for (int i = 0; i < ongui.Body.Instructions.Count; i++)
                {
                    if (ongui.Body.Instructions[i].ToString().Contains("RowGotDoubleClicked"))
                    {
                        onguiProcess.InsertAfter(ongui.Body.Instructions[i], onguiProcess.Create(OpCodes.Call, clickRef));
                        onguiProcess.InsertAfter(ongui.Body.Instructions[i], ongui.Body.Instructions[i - 1]);
                        onguiProcess.InsertAfter(ongui.Body.Instructions[i], ongui.Body.Instructions[i - 2]);
                        onguiProcess.InsertAfter(ongui.Body.Instructions[i], ongui.Body.Instructions[i - 3]);
                        onguiProcess.InsertAfter(ongui.Body.Instructions[i], ongui.Body.Instructions[i - 3]);
                        i += 5;
                    }
                }
            }
            AfterInject(ad);
        }
        else
        {
            EditorUtility.DisplayDialog("Inject", "UnityEditor.dll has Injected", "OK");
        }
    }
    
    private static bool MakeInjectTag()
    {
        string path = EditorApplication.applicationContentsPath + "/Managed/editor_inject.txt";
        if (!File.Exists(path))
        {
            File.WriteAllText(path, "UnityEditor.dll is overite by others!");
            XDebug.Log("Make Tag Finish!");
            return true;
        }
        return false;
    }

    private static void AfterInject(AssemblyDefinition ad)
    {
        string unityEditorPath = typeof(AssetDatabase).Module.FullyQualifiedName;
        string path = Path.Combine(Path.GetDirectoryName(Application.dataPath), "UnityEditor.dll");
        if (File.Exists(path)) File.Delete(path);
        ad.Write(path);
        try
        {
            string dest = Path.Combine(Path.GetDirectoryName(Application.dataPath), "Library/UnityAssemblies/");
            File.Copy(path, dest + "UnityEditor.dll");
            File.Copy(path, unityEditorPath, true);
        }
        catch (Exception e)
        {
            XDebug.LogWarning(e.Message);
            if (EditorUtility.DisplayDialog("Error", "move folder error. move folder manually please!", "OK"))
            {
                HelperEditor.Open(unityEditorPath);
                HelperEditor.Open(path);
            }
        }
        if (EditorUtility.DisplayDialog("Inject", "UnityEditor.dll Inject Finish, restart your Unity, Yes or Not", "OK", "NO"))
        {
            SetSccriptDefine();
            RestartUnity();
        }
    }

    public static void RestartUnity()
    {
#if UNITY_EDITOR_WIN
        string install = Path.GetDirectoryName(EditorApplication.applicationContentsPath);
        string path = Path.Combine(install, "Unity.exe");
        string[] args = path.Split('\\');
        System.Diagnostics.Process po = new System.Diagnostics.Process();
        Debug.Log("install: " + install + " path: " + path);
        po.StartInfo.FileName = path;
        po.Start();

        System.Diagnostics.Process[] pro = System.Diagnostics.Process.GetProcessesByName(args[args.Length - 1].Split('.')[0]);//Unity
        foreach (var item in pro)
        {
            item.Kill();
        }
#endif
    }

    public static void SetSccriptDefine()
    {
        string macro = XBuild.Macro;
        PlayerSettings.SetScriptingDefineSymbolsForGroup(BuildTargetGroup.Standalone, macro);
        PlayerSettings.SetScriptingDefineSymbolsForGroup(BuildTargetGroup.Android, macro);
        PlayerSettings.SetScriptingDefineSymbolsForGroup(BuildTargetGroup.iOS, macro);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }

    private static Instruction FindLast(MethodDefinition method)
    {
        int len = method.Body.Instructions.Count;
        return method.Body.Instructions[len - 1];
    }

    private static MethodDefinition GetMethod(TypeDefinition type, string methodName)
    {
        foreach (var item in type.Methods)
        {
            if (item.Name == methodName)
            {
                return item;
            }
        }
        return null;
    }

    private static TypeDefinition FindAssemlyType(AssemblyDefinition ad,string type)
    {
        var types = ad.MainModule.Types;
        for (int i = 0, max = types.Count; i < max; i++)
        {
            if (types[i].Name.Equals(type))
            {
                return types[i];
            }
        }
        return null;
    }


    public static AssemblyDefinition GetAssemblyDefination(string assemblyPath)
    {
        AssemblyDefinition ad = AssemblyDefinition.ReadAssembly(assemblyPath);
        DefaultAssemblyResolver dar = ad.MainModule.AssemblyResolver as DefaultAssemblyResolver;
        if (dar != null)
        {
            dar.AddSearchDirectory(Path.GetDirectoryName(assemblyPath));
        }
        return ad;
    }

}