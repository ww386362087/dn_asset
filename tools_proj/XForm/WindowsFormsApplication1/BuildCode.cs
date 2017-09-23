using System.Collections.Generic;
using Microsoft.Build.Evaluation;
using Microsoft.Build.Execution;
using System;
using System.IO;
using System.Windows.Forms;

namespace XForm
{
    public class BuildCode
    {

        private static string _output = string.Empty;

        private static string output
        {
            get
            {
                if(string.IsNullOrEmpty(_output))
                {
                    _output = XCForm.unity_proj_path + @"Assets\Lib\";
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
                    _project = XCForm.unity_proj_path + @"tools_proj\XForm\XForm.sln";
                }
                return _project;
            }
        }


        public static void Build(XCForm form)
        {
            if (!File.Exists(project)) MessageBox.Show("工程不存在 " + project);
            form.PCB("path:" + project);
            ProjectCollection collection = new ProjectCollection();
            Dictionary<string, string> prop = new Dictionary<string, string>();
            prop.Add("Configuration", "Release");//"Debug"
            prop.Add("Platform", "Any CPU");//"x86"
            prop.Add("OutputPath", output);
            foreach (var item in prop) form.PCB("arg:" + item.Key + ":" + item.Value);
            BuildParameters param = new BuildParameters(collection);
            BuildRequestData req = new BuildRequestData(
                project,
                prop,
                "3.5",
                new string[] { "Build" },
                null);

            BuildResult buildResult = BuildManager.DefaultBuildManager.Build(param, req);
            if (buildResult.OverallResult == BuildResultCode.Success)
            {
                Console.WriteLine("make success!");
                form.PCB("Build Success!");
            }
            else
            {
                form.PCB("\n编译失败, result:" + buildResult.OverallResult);
                if (buildResult.Exception != null)
                {
                    form.PCB(buildResult.Exception.Message);
                }
                else
                {
                    form.PCB("end!");
                }
            }
        }

    }
}
