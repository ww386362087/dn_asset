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
                    _destDir = XCForm.unity_proj_path + @"tools_proj\XCPP\GameCore\";
                }
                return _destDir;
            }
        }

        private string _tempdir = string.Empty;
        private string tempdir
        {
            get
            {
                if (string.IsNullOrEmpty(_tempdir))
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
                GenerateHead(tb, name);
                GenerateCpp(tb, name);
                f.PCB(files[i].FullName);
            }
            f.PCB("generate cpp finish");
        }

        private void HandleSort(bool sort, ref string con)
        {
            string sign = @"[\\]";
            if (sort)
            {
                con = con.Replace(sign, string.Empty);
            }
            else
            {
                string[] arr = con.Split('\n');
                for (int i = 0; i < arr.Length; i++)
                {
                    if (arr[i].Contains(sign))
                    {
                        arr[i] = string.Empty;
                    }
                }
                con = string.Empty;
                for (int i = 0; i < arr.Length; i++)
                {
                    if (!string.IsNullOrEmpty(arr[i])) con += arr[i] + '\n';
                }
            }
        }

        private void GenerateHead(CSVTable tb, string name)
        {
            string new_h = temp_h_content.Replace("[*Table*]", name);
            HandleSort(tb.isSort, ref new_h);
            StringBuilder sb = new StringBuilder();
            for (int i = 0, max = tb.types.Length; i < max; i++)
            {
                if (tb.types[i].Contains("<>")) //Seq
                {
                    sb.Append("\n\tSeq<" + tb.types[i].Replace("<>", "") + "> " + tb.titles[i].ToLower() + ";");
                }
                else if (tb.types[i].ToLower().Equals("string[]")) //stringarray
                {
                    string tp = tb.types[i].Replace("[]", " ");
                    sb.Append("\n\tchar " + tb.titles[i].ToLower() + "[MaxArraySize][MaxStringSize];");
                }
                else if (tb.types[i].Contains("[]"))
                {
                    string tp = tb.types[i].Replace("[]", " ");
                    sb.Append("\n\t" + tp + tb.titles[i].ToLower() + "[MaxArraySize];");
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


        private void GenerateCpp(CSVTable tb, string name)
        {
            string new_c = temp_c_content.Replace("[*Table*]", name);
            HandleSort(tb.isSort, ref new_c);
            new_c = new_c.Replace("[*table*]", name.ToLower());
            new_c = new_c.Replace("[*uid*]",tb.titles[0].ToLower());
            StringBuilder sb = new StringBuilder();
            for (int i = 0, max = tb.types.Length; i < max; i++)
            {
                if (tb.types[i].Contains("<>")) //Seq
                {
                    sb.Append("\n\t\tReadSeq(row->" + tb.titles[i].ToLower() + ");");
                }
                else if (tb.types[i].ToLower().Equals("string[]")) //stringarray
                {
                    sb.Append("\n\t\tReadStringArray(row->" + tb.titles[i].ToLower() + ");");
                }
                else if (tb.types[i].Contains("[]")) //general array
                {
                    string tp = tb.types[i].Replace("[]", "");
                    if (tp == "string") tp = "std::string";
                    sb.Append("\n\t\tReadArray<" + tp + ">(row->" + tb.titles[i].ToLower() + ");");
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
            MergeVcxproj(name);
            MergeProjFilter(name);
        }

        private const int pivot = 20;
        private void MergeVcxproj(string table)
        {
            string vcxproj = destdir + "GameCore.vcxproj";
            if (File.Exists(vcxproj))
            {
                string content = File.ReadAllText(vcxproj);
                string sign_h = "Common.h";
                string sign_c = "Common.cpp";
                string target_h = "<ClInclude Include=\"" + table + ".h\"/>\n\t";
                string target_c = "<ClInclude Include=\"" + table + ".cpp\"/>\n\t";
                if (!content.Contains(table))
                {
                    int point = content.IndexOf(sign_h) - pivot;
                    if (point > 0) content = content.Insert(point, target_h);
                    else throw new Exception("error in merge vcxproj");
                    point = content.IndexOf(sign_c) - pivot;
                    if (point > 0) content = content.Insert(point, target_c);
                    else throw new Exception("error merge vcxproj");
                }
                File.WriteAllText(vcxproj, content);
            }
            else
            {
                throw new Exception("not find vcxproj file in c++ vs proj");
            }
        }


        private void MergeProjFilter(string table)
        {
            string vcxproj = destdir + "GameCore.vcxproj.filters";
            if (File.Exists(vcxproj))
            {
                string content = File.ReadAllText(vcxproj);
                string sign_h = "Common.h";
                string sign_c = "Common.cpp";
                string target_h = "<ClInclude Include=\"" + table + ".h\">\n\t\t";
                string target_c = "<ClInclude Include=\"" + table + ".cpp\">\n\t\t";
                string add_h = "<Filter>Table</Filter>\n\t</ClInclude>\n\t";
                string add_c = "<Filter>Table</Filter>\n\t</ClInclude>\n\t";
                if (!content.Contains(table))
                {
                    int point = content.LastIndexOf(sign_h) - pivot;
                    if (point > 0) content = content.Insert(point, target_h + add_h);
                    else throw new Exception("error in merge vcxproj");
                    point = content.IndexOf(sign_c) - pivot;
                    if (point > 0) content = content.Insert(point, target_c + add_c);
                    else throw new Exception(" merge vcxproj");
                }
                File.WriteAllText(vcxproj, content);
            }
            else
            {
                throw new Exception("not find proj filter file in c++ vs proj");
            }
        }
    }

}
