using System.IO;
using System;
using System.CodeDom;
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
            XDebug.Log("gene code: " + path);
            FileInfo file = new FileInfo(path);
            GenerateTable(file);
        }

        private void GenerateTable(FileInfo file)
        {
            CSVTable tb = CSVUtil.sington.UtilCsv(file);
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
            method.Statements.Add(new CodeSnippetStatement("\t\t\tif (lineCount > 0) Table = new RowData[lineCount];"));
            method.Statements.Add(new CodeSnippetStatement("\t\t\telse Table = null;"));

            CodeMemberMethod method2 = new CodeMemberMethod();
            method2.Name = "ReadLine";
            method2.Attributes = MemberAttributes.Override | MemberAttributes.Public;
            method2.Parameters.Add(new CodeParameterDeclarationExpression(typeof(BinaryReader), "reader")); //参数
            method2.ReturnType = new CodeTypeReference(typeof(void));//返回值
            method2.Statements.Add(new CodeSnippetStatement("\t\t\tRowData row = new RowData();"));
            for (int i = 0; i < tb.types.Length; i++)
            {
                if (tb.parses[i].type == ValueType.Atom)
                {
                    method2.Statements.Add(new CodeSnippetStatement("\t\t\tRead<" + tb.types[i] + ">(reader, ref row." + tb.titles[i] + ", " + tb.types[i] + "Parse); columnno = " + i + ";"));
                }
                else if (tb.parses[i].type == ValueType.Array)
                {
                    method2.Statements.Add(new CodeSnippetStatement("\t\t\tReadArray<" + tb.types[i].Replace("[]", "") + ">(reader, ref row." + tb.titles[i] + ", " + tb.types[i].Replace("[]", "") + "Parse); columnno = " + i + ";"));
                }
                else if (tb.parses[i].type == ValueType.Sequence)
                {
                    method2.Statements.Add(new CodeSnippetStatement("\t\t\tReadSequence<" + tb.types[i].Replace("<>", "") + ">(reader, ref row." + tb.titles[i] + ", " + tb.types[i].Replace("<>", "") + "Parse); columnno = " + i + ";"));
                }
            }
            if (tb.isSort) method2.Statements.Add(new CodeSnippetStatement("\t\t\trow.sortID = (int)row." + tb.titles[0] + ";"));
            method2.Statements.Add(new CodeSnippetStatement("\t\t\tTable[lineno] = row;"));
            method2.Statements.Add(new CodeSnippetStatement("\t\t\tcolumnno = -1;"));

            if (tb.isSort)
            {
                CodeMemberMethod method3 = new CodeMemberMethod();
                method3.Name = "GetByUID";
                method3.Attributes = MemberAttributes.Public;
                method3.Parameters.Add(new CodeParameterDeclarationExpression(typeof(int), "id")); //参数
                method3.ReturnType = new CodeTypeReference("RowData");//返回值
                method3.Comments.Add(new CodeCommentStatement("二分法查找"));
                method3.Statements.Add(new CodeSnippetStatement("\t\t\treturn BinarySearch(Table, 0, Table.Length - 1, id) as RowData;"));
                wrapClass.Members.Add(method3);
            }

            wrapClass.Members.Add(method);
            wrapClass.Members.Add(method2);
           
            StringBuilder fileContent = new StringBuilder();
            using (StringWriter sw = new StringWriter(fileContent))
            {
                CodeDomProvider.CreateProvider("CSharp").GenerateCodeFromCompileUnit(compunit, sw, new CodeGeneratorOptions());
            }

            StringBuilder content2 = new StringBuilder();
            content2.Append(tb.isSort ? "public class RowData :BaseRow {" : "public class RowData {");
            for (int i = 0, max = tb.types.Length; i < max; i++)
            {
                if (tb.types[i].Contains("<>"))
                {
                    content2.Append("\n\t\t\tpublic Sequence<" + tb.types[i].Replace("<>", "") + "> " + tb.titles[i] + ";");
                }
                else
                {
                    content2.Append("\n\t\t\tpublic " + tb.types[i] + " " + tb.titles[i] + ";");
                }
            }
            content2.Append("\r\n\t\t}\r\n");

            content2.Append("\r\n\r\n\t\tprivate RowData[] Table;");
            content2.Append("\r\n\r\n\t\tpublic override int length { get { return Table.Length; } }");
            content2.Append("\r\n\r\n\t\tpublic RowData this[int index] { get { return Table[index]; } }");
            content2.Append("\r\n\r\n\t\tpublic override string bytePath { get { return \"Table/" + name + "\"; } }");

            fileContent.Replace("public int replace;", content2.ToString());
            string filePath = destdir + name + ".cs";
            File.WriteAllText(filePath, fileContent.ToString());
            //XDebug.Log("make code: " + fileContent);
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
