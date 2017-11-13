using System;
using System.Windows.Forms;

namespace XForm
{
    static class Program
    {

        static readonly string make_byte = "-t";
        static readonly string make_code = "-c";


        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        static void Main(String[] args)
        {
            XDebug.Begin();
            if (ProcessArgs(args))
            {
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                Application.Run(new XCForm());
            }
        }


        static bool ProcessArgs(string[] args)
        {
            if (args != null && args.Length > 0)
            {
                XDebug.Log("arg len:" + args.Length.ToString() + " arg0: " + args[0]);
                for (int i = 1; i < args.Length; i++)
                {
                    if (args[0].Equals(make_byte))
                    {
                        CheckException(args[i]);
                        XDebug.Log("gennerate bytes: " + args[i]);
                        GenerateBytes.sington.WriteByte(args[i]);
                    }
                    else if (args[0].Equals(make_code))
                    {
                        CheckException(args[i]);
                        XDebug.Log("gennerate code: " + args[i]);
                        GenerateCode.sington.GenerateTCode(args[i]);
                        BuildCode.Build();
                    }
                    else
                    {
                        XDebug.LogError("exception " + make_code.Length + " args:" + args[0].Length);
                    }
                }
                return false;
            }
            return true;
        }


        static void CheckException(string path)
        {
            if (string.IsNullOrEmpty(path))
                throw new Exception("路径为null");
            else if (!path.EndsWith(".csv"))
                throw new Exception("不是csv表格");
        }
    }
}
