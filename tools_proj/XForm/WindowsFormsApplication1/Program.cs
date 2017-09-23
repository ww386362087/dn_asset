using System;
using System.Windows.Forms;
using System.IO;

namespace XForm
{
    static class Program
    {

        static readonly string make_byte = "-t";
        static readonly string make_code = "-c";
        static readonly string make_all = "-a";

        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        static void Main(String[] args)
        {
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
                string path = XCForm.unity_proj_path;
                using (FileStream fs = new FileStream(path + "args.txt", FileMode.Create))
                {
                    StreamWriter writer = new StreamWriter(fs);
                    string head = args[0];
                    writer.WriteLine(head);

                    for (int i = 1; i < args.Length; i++)
                    {
                        writer.WriteLine(args[i]);
                        if (head.Equals(make_byte))
                        {
                            CheckException(args[i]);
                            GenerateByte.sington.WriteByte(args[i]);
                        }
                        else if (head.Equals(make_code))
                        {
                            CheckException(args[i]);
                            GenerateCode.sington.GenerateTCode(args[i]);
                        }
                        else if (head.Equals(make_all))
                        {
                            CheckException(args[i]);
                            GenerateByte.sington.WriteByte(args[i]);
                            GenerateCode.sington.GenerateTCode(args[i]);
                        }
                    }
                    writer.Close();
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
