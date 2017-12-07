using System;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.IO;
using System.Text;

namespace XForm
{
    public class GenerateMarshalCode
    {
        private static GenerateMarshalCode _s = null;
        public static GenerateMarshalCode sington { get { if (_s == null) _s = new GenerateMarshalCode(); return _s; } }

        XCForm form;
        
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
                    _destDir = XCForm.unity_proj_path + @"tools_proj\XLib\XLib\Marshal\";
                }
                return _destDir;
            }
        }

        private string _csproj = string.Empty;
        private string csproj
        {
            get
            {
                if (string.IsNullOrEmpty(_csproj))
                {
                    _csproj = XCForm.unity_proj_path + @"tools_proj\XLib\XLib\XLib.csproj";
                }
                return _csproj;
            }
        }

        public void CleanAll(XCForm f)
        {
            form = f;
            DirectoryInfo dir = new DirectoryInfo(destdir);
            FileInfo[] files = dir.GetFiles();
            for (int i = 0, max = files.Length; i < max; i++)
            {
                if (files[i].Name != "CCommon")
                {
                    File.Delete(files[i].FullName);
                }
            }
        }

        public void GenerateAll(XCForm f)
        {
            form = f;
            Console.WriteLine("dest: " + destdir + " origin: " + originDir);
            DirectoryInfo dinfo = new DirectoryInfo(originDir);
            FileInfo[] files = dinfo.GetFiles("*.csv");
            for (int i = 0, max = files.Length; i < max; i++)
            {
                GenerateTable(files[i]);
                f.PCB(files[i].FullName);
            }
            f.PCB("finish");
        }

        private void GenerateTable(FileInfo file)
        {
            CSVTable tb = CSVUtil.sington.UtilCsv(file);
            string name = file.Name.Substring(0, file.Name.LastIndexOf('.'));

            //声明代码的部分
            CodeCompileUnit compunit = new CodeCompileUnit();
            CodeNamespace sample = new CodeNamespace("GameCore");
            //引用命名空间
            sample.Imports.Add(new CodeNamespaceImport("System.Collections.Generic"));
            sample.Imports.Add(new CodeNamespaceImport("System.Runtime.InteropServices"));

            compunit.Namespaces.Add(sample);
            //在命名空间下添加一个类
            CodeTypeDeclaration wrapClass = new CodeTypeDeclaration("C" + name);
            sample.Types.Add(wrapClass);

            //加一个标记 用来替换
            CodeMemberField field = new CodeMemberField(typeof(int), "replace");
            field.Attributes = MemberAttributes.Public;
            wrapClass.Members.Add(field);

            CodeMemberField field2 = new CodeMemberField("RowData", "m_data");
            field2.Attributes = MemberAttributes.Static | MemberAttributes.Private;
            wrapClass.Members.Add(field2);


            //属性 public static int length { get { return iGetEquipSuitLength(); } }
            CodeMemberProperty prop = new CodeMemberProperty();
            prop.Name = "length";
            prop.HasGet = true;
            prop.HasSet = false;
            prop.Attributes = MemberAttributes.Public | MemberAttributes.Static;
            prop.Type = new CodeTypeReference(typeof(int));
            prop.GetStatements.Add(new CodeSnippetStatement("\t\t\t\treturn iGet" + name + "Length();"));
            wrapClass.Members.Add(prop);

            // public static RowData GetRow(int val)
            CodeMemberMethod method = new CodeMemberMethod();
            method.Name = "GetRow";
            method.Attributes = MemberAttributes.Static | MemberAttributes.Public;
            method.Parameters.Add(new CodeParameterDeclarationExpression(typeof(int), "idx"));
            method.ReturnType = new CodeTypeReference("RowData");//返回值
            method.Statements.Add(new CodeSnippetStatement("\t\t\tiGet" + name + "Row(idx, ref m_data);"));
            method.Statements.Add(new CodeSnippetStatement("\t\t\treturn m_data;"));
            wrapClass.Members.Add(method);

            StringBuilder fileContent = new StringBuilder();
            using (StringWriter sw = new StringWriter(fileContent))
            {
                CodeDomProvider.CreateProvider("CSharp").GenerateCodeFromCompileUnit(compunit, sw, new CodeGeneratorOptions());
            }

            StringBuilder sb = new StringBuilder();
            sb.Append("[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]");
            sb.Append("\n\t\tpublic struct RowData {");
            for (int i = 0, max = tb.types.Length; i < max; i++)
            {
                if (tb.types[i].Contains("<>")) //Seq
                {
                    sb.Append("\n\t\t\t[MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]");
                    sb.Append("\n\t\t\t" + tb.types[i].Replace("<>", "[] ") +  tb.titles[i].ToLower() + ";");
                }
                else if (tb.types[i].Contains("[]"))
                {
                    sb.Append("\n\t\t\t[MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]");
                    sb.Append("\n\t\t\t" + tb.types[i] + " " + tb.titles[i].ToLower() + ";");
                }
                else if (tb.types[i].ToLower().Equals("string"))
                {
                    sb.Append("\n\t\t\t[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 64)]");
                    sb.Append("\n\t\t\tstring " + tb.titles[i].ToLower() + ";");
                }
                else
                {
                    sb.Append("\n\t\t\t" + tb.types[i] + " " + tb.titles[i].ToLower() + ";");
                }
            }

            for (int i = 0, max = tb.types.Length; i < max; i++)
            {
                if (tb.types[i].Contains("<>")) //Seq
                {
                    string ctype = "CSeq< " + tb.types[i].Replace("<", "");
                    sb.Append("\n\n\t\t\tpublic " + ctype + " " + FirstUpperStr(tb.titles[i]) + "{ get { return new " + ctype + "(ref " + tb.titles[i].ToLower() + "); } }");
                }
                else if (tb.types[i].Contains("[]"))
                {
                    string tn = tb.titles[i].ToLower();
                    string tp = tb.types[i].Replace("[", string.Empty).Replace("]", string.Empty);
                    sb.Append("\n\n\t\t\t" + tp + "[] " + FirstUpperStr(tn) + " { ");
                    sb.Append("\n\t\t\t\tget { ");
                    sb.Append("\n\t\t\t\t\tif (" + tn + ".Length == 16) {");
                    sb.Append("\n\t\t\t\t\tList<" + tp + "> list = new List<" + tp + ">();");
                    sb.Append("\n\t\t\t\t\tfor (int i = " + tn + ".Length - 1; i >= 0; i--)");
                    sb.Append("\n\t\t\t\t\t{");
                    string invalid = "-1";
                    if (tp == "string") invalid = "\"-1\"";
                    sb.Append("\n\t\t\t\t\t\tif (" + tn + "[i] != " + invalid + ") list.Add(" + tn + "[i]);");
                    sb.Append("\n\t\t\t\t\t}");
                    sb.Append("\n\t\t\t\t\t" + tn + " = list.ToArray();");
                    sb.Append("\n\t\t\t\t\t}");
                    sb.Append("\n\t\t\t\t\t return " + tn + ";");
                    sb.Append("\n\t\t\t\t}");
                    sb.Append("\n\t\t\t}");
                }
                else
                {
                    sb.Append("\n\n\t\t\tpublic " + tb.types[i] + " " + FirstUpperStr(tb.titles[i]) + " { get { return " + tb.titles[i].ToLower() + "; } }");
                }
            }

            sb.Append("\r\n\t\t}\r\n");

            AppendExtendAttr(sb);
            sb.Append("\r\n\t\tstatic extern void iGet" + name + "Row(int idx, ref RowData row);");
            AppendExtendAttr(sb);
            sb.Append("\r\n\t\tstatic extern int iGet" + name + "Length();");

            fileContent.Replace("public int replace;", sb.ToString());
            string filePath = destdir + "C" + name + ".cs";
            File.WriteAllText(filePath, fileContent.ToString());
            MergeCsproj(name);
        }


        private void AppendExtendAttr(StringBuilder sb)
        {
            sb.Append("\n\n#if UNITY_IPHONE || UNITY_XBOX360");
            sb.Append("\n\t\t[DllImport(\"__Internal\")]");
            sb.Append("\n#else");
            sb.Append("\n\t\t[DllImport(\"GameCore\")]");
            sb.Append("\n#endif");
        }

        private string FirstUpperStr(string s)
        {
            if (!string.IsNullOrEmpty(s))
            {
                if (s.Length > 1)
                {
                    return char.ToUpper(s[0]) + s.Substring(1);
                }
                return char.ToUpper(s[0]).ToString();
            }
            return null;
        }

        private void MergeCsproj(string table)
        {
            if (File.Exists(csproj))
            {
                string content = File.ReadAllText(csproj);
                string sign = @"Marshal\C" + table;
                if (!content.Contains(sign))
                {
                    int point = content.LastIndexOf("</ItemGroup>");
                    string target = "<Compile Include=\"Marshal\\C" + table + ".cs\" />\n\t";
                    if (point != -1)
                    {
                        content = content.Insert(point, target);
                    }
                    else throw new Exception("not find csproj item <itemgroup> in lib project");
                }
                File.WriteAllText(csproj, content);
            }
            else
            {
                throw new Exception("not find csproj file in lib project");
            }
        }

    }
}
