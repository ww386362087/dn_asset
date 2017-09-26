using Microsoft.CSharp;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IO;

namespace XForm
{
    public class BuildCode
    {

        private static string _output = string.Empty;

        private static string output
        {
            get
            {
                if (string.IsNullOrEmpty(_output))
                {
                    _output = XCForm.unity_proj_path + @"Assets\Lib\XLib.dll";
                }
                return _output;
            }
        }

        private static string _project = string.Empty;

        private static string project
        {
            get
            {
                if (string.IsNullOrEmpty(_project))
                {
                    _project = XCForm.unity_proj_path + @"tools_proj\XLib\XLib\";
                }
                return _project;
            }
        }


        static XCForm mForm;

        static string _u3d = string.Empty;
        static string u3d
        {
            get
            {
                if (string.IsNullOrEmpty(_u3d))
                    _u3d = XCForm.unity_proj_path + @"Library\UnityAssemblies\UnityEngine.dll";
                return _u3d;
            }
        }


        private static CompilerParameters MakeParamters()
        {
            CompilerParameters parameters = new CompilerParameters();
            parameters.WarningLevel = 2;
            parameters.GenerateExecutable = false;
            parameters.GenerateInMemory = false;
            parameters.OutputAssembly = output;
            parameters.ReferencedAssemblies.Add("System.dll");
            parameters.ReferencedAssemblies.Add(u3d);
            return parameters;
        }

        public static void Build()
        {
            CSharpCodeProvider provider = new CSharpCodeProvider();
            CompilerParameters parameters = MakeParamters();
            string[] sourceFile = ScanFiles(project);
            CompilerResults cr = provider.CompileAssemblyFromFile(parameters, sourceFile);
            if (cr.Errors.Count > 0)
            {
                XDebug.Log("Errors building into " + cr.PathToAssembly);
                foreach (var item in cr.Errors)
                {
                    XDebug.Log(item.ToString());
                }
            }
            else
            {
                XDebug.Log("编译成功");
            }
        }

        public static void Build(XCForm form)
        {
            mForm = form;
            CSharpCodeProvider provider = new CSharpCodeProvider();
            CompilerParameters parameters = MakeParamters();
            string[] sourceFile = ScanFiles(project);
            CompilerResults cr = provider.CompileAssemblyFromFile(parameters, sourceFile);
            if (cr.Errors.Count > 0)
            {
                form.PCB("Errors building into " + cr.PathToAssembly);
                foreach (CompilerError ce in cr.Errors)
                {
                    form.PCB(ce.ToString());
                }
            }
            else
            {
                form.PCB("编译成功");
            }
        }

        private static string[] ScanFiles(string path)
        {
            DirectoryInfo dir = new DirectoryInfo(path);
            FileInfo[] files = dir.GetFiles("*.cs", SearchOption.AllDirectories);
            List<string> slist = new List<string>();
            if (mForm != null) mForm.PCB("scan files cnt: " + files.Length + ". as list:");
            XDebug.Log("scan files cnt: " + files.Length);
            for (int i = 0, max = files.Length; i < max; i++)
            {
                string str = files[i].FullName;
                if (!str.Contains("TemporaryGeneratedFile"))
                {
                    slist.Add(str);
                    if (mForm != null) mForm.PCB(str);
                    XDebug.Log(str);
                }
            }
            return slist.ToArray();
        }

    }

}
