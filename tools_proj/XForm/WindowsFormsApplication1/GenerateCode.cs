using System.IO;
using System;
using System.CodeDom;
using Microsoft.CSharp;
using System.Text;
using System.CodeDom.Compiler;


namespace XForm
{
    public class GenerateCode
    {
        private static GenerateCode _s = null;
        public static GenerateCode sington { get { if (_s == null)_s = new GenerateCode(); return _s; } }

        XCForm form;

        private string _originDir = string.Empty;
        public string originDir
        {
            get
            {
                if (string.IsNullOrEmpty(_originDir))
                {
                    _originDir = XCForm.unity_table_path;
                }
                return _originDir;
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

        private string _destDir = string.Empty;
        private string destdir
        {
            get
            {
                if (string.IsNullOrEmpty(_destDir))
                {
                    _destDir = XCForm.unity_proj_path + @"tools_proj\XLib\XLib\Table\";
                }
                return _destDir;
            }
        }


        private string _tableMgr = string.Empty;
        private string tableMgr
        {
            get
            {
                if(string.IsNullOrEmpty(_tableMgr))
                {
                    _tableMgr = XCForm.unity_proj_path + @"tools_proj\XLib\XLib\Common\XTableMgr.cs";
                }
                return _tableMgr;
            }
        }


        public void CleanAll(XCForm f)
        {
            form = f;
            DirectoryInfo dir = new DirectoryInfo(destdir);
            FileInfo[] files = dir.GetFiles();
            for (int i = 0, max = files.Length; i < max; i++)
            {
                File.Delete(files[i].FullName);
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
        }


        public void GenerateTCode(string path)
        {
            FileInfo file = new FileInfo(path);
            GenerateTable(file);
        }

        private void GenerateTable(FileInfo file)
        {
            string[] titles = null;
            string[] types = null;
            ValueParse[] parses = null;
            CSVUtil.sington.UtilType(file, out titles, out types, out parses);
            string name = file.Name.Substring(0, file.Name.LastIndexOf('.'));

            //声明代码的部分
            CodeCompileUnit compunit = new CodeCompileUnit();
            CodeNamespace sample = new CodeNamespace("XTable");
            //引用命名空间
            //sample.Imports.Add(new CodeNamespaceImport("UnityEngine"));

            compunit.Namespaces.Add(sample);
            //在命名空间下添加一个类
            CodeTypeDeclaration wrapClass = new CodeTypeDeclaration(name + " : CVSReader");
            sample.Types.Add(wrapClass);

            //加一个标记 用来替换
            CodeMemberField field = new CodeMemberField(typeof(int), "replace");
            field.Attributes = MemberAttributes.Public;
            wrapClass.Members.Add(field);

            //为这个类添加一个方法   public override int 方法名(string str);
            CodeMemberMethod method = new CodeMemberMethod();
            method.Name = "OnClear";
            method.Attributes = MemberAttributes.Override | MemberAttributes.Public;
            method.Parameters.Add(new CodeParameterDeclarationExpression(typeof(int), "lineCount"));
            method.ReturnType = new CodeTypeReference(typeof(void));//返回值
            method.Statements.Add(new CodeSnippetStatement("\t\t\tif (lineCount > 0) table = new RowData[lineCount];"));
            method.Statements.Add(new CodeSnippetStatement("\t\t\telse table = null;"));

            CodeMemberMethod method2 = new CodeMemberMethod();
            method2.Name = "ReadLine";
            method2.Attributes = MemberAttributes.Override | MemberAttributes.Public;
            method2.Parameters.Add(new CodeParameterDeclarationExpression(typeof(BinaryReader), "reader")); //参数
            method2.ReturnType = new CodeTypeReference(typeof(void));//返回值
            method2.Statements.Add(new CodeSnippetStatement("\t\t\tRowData row = new RowData();"));
            for (int i = 0; i < types.Length; i++)
            {
                if (parses[i].type == ValueType.Atom)
                {
                    method2.Statements.Add(new CodeSnippetStatement("\t\t\tRead<" + types[i] + ">(reader, ref row." + titles[i] + ", " + types[i] + "Parse); columnno = " + i + ";"));
                }
                else if (parses[i].type == ValueType.Array)
                {
                    method2.Statements.Add(new CodeSnippetStatement("\t\t\tReadArray<" + types[i].Replace("[]", "") + ">(reader, ref row." + titles[i] + ", " + types[i].Replace("[]", "") + "Parse); columnno = " + i + ";"));
                }
            }
            method2.Statements.Add(new CodeSnippetStatement("\t\t\tTable[lineno] = row;"));
            method2.Statements.Add(new CodeSnippetStatement("\t\t\tcolumnno = -1;"));


            wrapClass.Members.Add(method);
            wrapClass.Members.Add(method2);

            CSharpCodeProvider cprovider = new CSharpCodeProvider();
            ICodeGenerator gen = cprovider.CreateGenerator();
            StringBuilder fileContent = new StringBuilder();
            using (StringWriter sw = new StringWriter(fileContent))
            {
                gen.GenerateCodeFromCompileUnit(compunit, sw, new CodeGeneratorOptions());
            }

            StringBuilder content2 = new StringBuilder();
            content2.Append("public class RowData{");
            for (int i = 0, max = types.Length; i < max; i++)
            {
                content2.Append("\n\t\t\tpublic " + types[i] + " " + titles[i] + ";");
            }
            content2.Append("\r\n\t\t}\r\n");

            content2.Append("\r\n\r\n\t\tpublic RowData[] Table { get { return table; } }");
            content2.Append("\r\n\r\n\t\tprivate RowData[] table = null;");
            content2.Append("\r\n\r\n\t\tpublic override string bytePath { get { return \"Table/" + name + "\"; } }");

            fileContent.Replace("public int replace;", content2.ToString());
            string filePath = destdir + name + ".cs";
            //Console.WriteLine("make:" + filePath);
            File.WriteAllText(filePath, fileContent.ToString());
            MergeCsproj(name);
            MergeTableMgr(name);
        }


        private void MergeCsproj(string table)
        {
            if (File.Exists(csproj))
            {
                string content = File.ReadAllText(csproj);
                if (!content.Contains(table))
                {
                    int point = content.LastIndexOf("</ItemGroup>");
                    string target = "\t<Compile Include=\"Table\\" + table + ".cs\" />\n\t";
                    if (point != -1)
                    {
                        content = content.Insert(point, target);
                      //  Console.WriteLine("make:" + point + " with table: " + table);
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


        private void MergeTableMgr(string table)
        {
            if(File.Exists(tableMgr))
            {
                string content = File.ReadAllText(tableMgr);
                if (!content.Contains(table))
                {
                    int point = content.LastIndexOf("loadFinish = false;");
                    string target = "Add<" + table + ">();\n\t\t";
                    if (point != -1)
                    {
                        content = content.Insert(point, target);
                        Console.WriteLine("make:" + point + " with table: " + table);
                    }
                    else throw new Exception("not find csproj item <itemgroup> in lib project");
                }
                File.WriteAllText(tableMgr, content);
            }
            else
            {
                throw new Exception("not find XTableMgr.cs");
            }
        }

    }

}
