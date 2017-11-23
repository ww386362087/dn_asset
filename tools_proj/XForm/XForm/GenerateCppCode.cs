using System;
using System.IO;
using System.Text;

namespace XForm
{
    public class GenerateCppCode
    {
        private static GenerateCppCode _s = null;
        public static GenerateCppCode sington { get { if (_s == null) _s = new GenerateCppCode(); return _s; } }

        XCForm form;

        private static string temp_h_content;
        private static string temp_c_content;

        public string originDir
        {
            get { return XCForm.unity_table_path; }
        }

        private string _destDir = string.Empty;
        private string destdir
        {
            get
            {
                if (string.IsNullOrEmpty(_destDir))
                {
                    _destDir = XCForm.unity_proj_path + @"tools_proj\XCPP\XTable\";
                }
                return _destDir;
            }
        }

        private string _tempdir = string.Empty;
        private string tempdir
        {
            get
            {
                if(string.IsNullOrEmpty(_tempdir))
                {
                    _tempdir = XCForm.unity_proj_path + @"tools_proj\XForm\XForm\Template\";
                }
                return _tempdir;
            }
        }

        public void GenerateAll(XCForm f)
        {
            form = f;

            Console.WriteLine("dest: " + destdir + " origin: " + originDir);
            if (string.IsNullOrEmpty(temp_h_content)) temp_h_content = File.ReadAllText(tempdir + "Template.h");
            if (string.IsNullOrEmpty(temp_c_content)) temp_c_content = File.ReadAllText(tempdir + "Template.cpp");

            DirectoryInfo dinfo = new DirectoryInfo(originDir);
            FileInfo[] files = dinfo.GetFiles("*.csv");
            for (int i = 0, max = files.Length; i < max; i++)
            {
                CSVTable tb = CSVUtil.sington.UtilCsv(files[i]);
                string name = tb.name.Replace(".csv", "");
                GenerateHead(tb,name);
                GenerateCpp(tb,name);
                f.PCB(files[i].FullName);
            }
            f.PCB("generate cpp finish");
        }

        private void GenerateHead(CSVTable tb,string name)
        {
            string new_h = temp_h_content.Replace("[*Table*]", name);
            StringBuilder sb = new StringBuilder();
            for (int i = 0, max = tb.types.Length; i < max; i++)
            {
                if (tb.types[i].Contains("<>")) //Seq
                {
                    sb.Append("\n\tSeq<" + tb.types[i].Replace("<>", "") + "> " + tb.titles[i].ToLower() + ";");
                }
                else if (tb.types[i].Contains("[]"))
                {
                    sb.Append("\n\t" + tb.types[i].Replace("[]", " ") + tb.titles[i].ToLower() + "[MaxArraySize];");
                }
                else if (tb.types[i].ToLower().Equals("string"))
                {
                    sb.Append("\n\tchar " + tb.titles[i].ToLower() + "[MaxStringSize];");
                }
                else
                {
                    sb.Append("\n\t" + tb.types[i] + " " + tb.titles[i].ToLower() + ";");
                }
            }
            new_h = new_h.Replace("[**var**]", sb.ToString());
            string filePath = destdir + name + ".h";
            File.WriteAllText(filePath, new_h);
        }


        private void GenerateCpp(CSVTable tb,string name)
        {
            string new_c = temp_c_content.Replace("[*Table*]", name);
            new_c = new_c.Replace("[*table*]", name.ToLower());
            StringBuilder sb = new StringBuilder();
            for (int i = 0, max = tb.types.Length; i < max; i++)
            {
                if (tb.types[i].Contains("<>")) //Seq
                {
                    sb.Append("\n\t\tReadSeq(row->" + tb.titles[i].ToLower() + ");");
                }
                else if (tb.types[i].Contains("[]"))
                {
                    sb.Append("\n\t\tReadArray<" + tb.types[i].Replace("[]", ">(row->") + tb.titles[i].ToLower() + ");");
                }
                else if (tb.types[i].ToLower().Equals("string"))
                {
                    sb.Append("\n\t\tReadString(row->" + tb.titles[i].ToLower() + ");");
                }
                else
                {
                    sb.Append("\n\t\tRead(&(row->" + tb.titles[i].ToLower() + "));");
                }
            }
            new_c = new_c.Replace("[**read**]", sb.ToString());
            string filePath = destdir + name + ".cpp";
            File.WriteAllText(filePath, new_c);
        }
    }
}
