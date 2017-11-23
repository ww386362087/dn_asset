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
            CodeNamespace sample = new CodeNamespace("XTable");
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
            field2.Attributes = MemberAttributes.Static|MemberAttributes.Private;
            wrapClass.Members.Add(field2);


            //属性 public static int length { get { return iGetEquipSuitLength(); } }
            CodeMemberProperty prop = new CodeMemberProperty();
            prop.Name = "length";
            prop.HasGet = true;
            prop.HasSet = false;
            prop.Attributes = MemberAttributes.Public | MemberAttributes.Static;
            prop.Type = new CodeTypeReference(typeof(int));
            var exp = new CodeFieldReferenceExpression(new CodeThisReferenceExpression(), "iGet" + name + "Length()");
            prop.GetStatements.Add(new CodeMethodReturnStatement(exp));
            wrapClass.Members.Add(prop);

            // public static RowData GetRow(int val)
            CodeMemberMethod method = new CodeMemberMethod();
            method.Name = "GetRow";
            method.Attributes = MemberAttributes.Static | MemberAttributes.Public;
            method.Parameters.Add(new CodeParameterDeclarationExpression(typeof(int), "val"));
            method.ReturnType = new CodeTypeReference("RowData");//返回值
            method.Statements.Add(new CodeSnippetStatement("\t\t\tiGet"+name+"Row(val, ref m_data);"));
            method.Statements.Add(new CodeSnippetStatement("\t\t\treturn m_data;"));
            wrapClass.Members.Add(method);

            StringBuilder fileContent = new StringBuilder();
            using (StringWriter sw = new StringWriter(fileContent))
            {
                CodeDomProvider.CreateProvider("CSharp").GenerateCodeFromCompileUnit(compunit, sw, new CodeGeneratorOptions());
            }

            StringBuilder content2 = new StringBuilder();
            content2.Append("[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]");
            content2.Append("\n\t\tpublic struct RowData {");
            for (int i = 0, max = tb.types.Length; i < max; i++)
            {
                if (tb.types[i].Contains("<>")) //Seq
                {
                    content2.Append("\n\t\t\tCSeq<" + tb.types[i].Replace("<>", "") + "> " + tb.titles[i].ToLower() + ";");
                }
                else if(tb.types[i].Contains("[]"))
                {
                    content2.Append("\n\t\t\t[MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]");
                    content2.Append("\n\t\t\t" + tb.types[i] + " " + tb.titles[i].ToLower() + ";");
                }
                else if(tb.types[i].ToLower().Equals("string"))
                {
                    content2.Append("\n\t\t\t[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 64)]");
                    content2.Append("\n\t\t\tstring " + tb.titles[i].ToLower() + ";");
                }
                else
                {
                    content2.Append("\n\t\t\t" + tb.types[i] + " " + tb.titles[i].ToLower() + ";");
                }
            }

            for (int i = 0, max = tb.types.Length; i < max; i++)
            {
                if (tb.types[i].Contains("<>")) //Seq
                {
                    content2.Append("\n\n\t\t\tpublic CSeq<" + tb.types[i].Replace("<>", "") + "> " + FirstUpperStr(tb.titles[i]) + "{ get { return " + tb.titles[i].ToLower() + "; } }");
                }
                else if (tb.types[i].Contains("[]"))
                {
                    string tn = tb.titles[i].ToLower();
                    content2.Append("\n\n\t\t\t" + tb.types[i]  +" "+ FirstUpperStr(tb.titles[i]) + " { ");
                    content2.Append("\n\t\t\t\tget { ");
                    content2.Append("\n\t\t\t\t\tif (" + tn + ".Length == 16) {");
                    content2.Append("\n\t\t\t\t\tList<int> list = new List<int>();");
                    content2.Append("\n\t\t\t\t\tfor (int i = " + tn + ".Length - 1; i >= 0; i--)");
                    content2.Append("\n\t\t\t\t\t{");
                    content2.Append("\n\t\t\t\t\t\tif (" + tn + "[i] != -1) list.Add(" + tn + "[i]);");
                    content2.Append("\n\t\t\t\t\t}");
                    content2.Append("\n\t\t\t\t\t" + tn + " = list.ToArray();");
                    content2.Append("\n\t\t\t\t\t}");
                    content2.Append("\n\t\t\t\t\t return " + tn + ";");
                    content2.Append("\n\t\t\t\t}");
                    content2.Append("\n\t\t\t}");
                }
                else
                {
                    content2.Append("\n\n\t\t\tpublic " + tb.types[i] + " " + FirstUpperStr(tb.titles[i]) + " { get { return " + tb.titles[i].ToLower() + "; } }");
                }
            }

            content2.Append("\r\n\t\t}\r\n");

            content2.Append("\n\n\t\t[DllImport(\"XTable\")]");
            content2.Append("\r\n\t\tstatic extern void iGet"+name+"Row(int val, ref RowData row);");

            content2.Append("\n\n\t\t[DllImport(\"XTable\")]");
            content2.Append("\r\n\t\tstatic extern int iGet" + name + "Length();");

            fileContent.Replace("public int replace;", content2.ToString());
            string filePath = destdir + name + ".cs";
            File.WriteAllText(filePath, fileContent.ToString());
        }

        private string FirstUpperStr(string s)
        {
            if (!string.IsNullOrEmpty(s))
            {
                s = s.ToLower();
                if (s.Length > 1)
                {
                    return char.ToUpper(s[0]) + s.Substring(1);
                }
                return char.ToUpper(s[0]).ToString();
            }
            return null;
        }

    }
}
