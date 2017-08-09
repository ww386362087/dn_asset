using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace XForm
{
    public class CSVUtil
    {

        private static CSVUtil _s = null;
        public static CSVUtil sington
        {
            get { if (_s == null) _s = new CSVUtil(); return _s; }
        }


        public readonly char[] ListSeparator = new char[] { '|' };
        public readonly char[] eof = { '\r', '\n' };

        private IntParse intParse;
        private UintParse uintParse;
        private BoolParse boolParse;
        private FloatParse floatParse;
        private StringParse stringParse;
        private ArrIntParse arrIntParse;
        private ArrUintParse arruintParse;
        private ArrFloatParse arrFloatParse;
        private ArrBoolParse arrBoolParse;
        private ArrStringParse arrStringParse;

        public CSVUtil()
        {
            intParse = new IntParse();
            uintParse = new UintParse();
            boolParse = new BoolParse();
            floatParse = new FloatParse();
            stringParse = new StringParse();
            arrIntParse = new ArrIntParse();
            arruintParse = new ArrUintParse();
            arrBoolParse = new ArrBoolParse();
            arrFloatParse = new ArrFloatParse();
            arrStringParse = new ArrStringParse();
        }


        public void UtilType(FileInfo file,out string[] titles,out string[] types, out ValueParse[] parses)
        {
            StreamReader sr = new StreamReader(file.FullName);

            CSVTable table = new CSVTable(file.Name);
            table.list = new List<CSVStruct>();
            string attachmsg = string.Empty;

            //字段
            string tile = sr.ReadLine();
            if (string.IsNullOrEmpty(tile.TrimEnd(eof)))
            {
                attachmsg = "标题为null";
                throw new Exception("非法的表格:" + file.Name + " " + attachmsg);
            }
            titles = UtilLine(tile);

            //注释
            string comment = sr.ReadLine();
            if (string.IsNullOrEmpty(comment.TrimEnd(eof)))
            {
                attachmsg = "注释为null";
                throw new Exception("非法的表格:" + file.Name + " " + attachmsg);
            }

            //类型
            string tp = sr.ReadLine();
            if (string.IsNullOrEmpty(tp.TrimEnd(eof)))
            {
                attachmsg = "类型为null";
                throw new Exception("非法的表格:" + file.Name + " " + attachmsg);
            }
            types = UtilLine(tp); 
            parses=new ValueParse[types.Length];
            for (int i = 0, max = parses.Length; i < max; i++)
            {
                parses[i] = TransParse(types[i], file.Name);
            }
        }

        public CSVTable UtilCsv(FileInfo file)
        {
            //针对ANSI编码的csv 不要用Unicode编码
            StreamReader sr = new StreamReader(file.FullName, Encoding.Default);

            CSVTable table = new CSVTable(file.Name);
            table.list = new List<CSVStruct>();
            string attachmsg = string.Empty;

            //字段
            string tile = sr.ReadLine();
            if (string.IsNullOrEmpty(tile.TrimEnd(eof)))
            {
                attachmsg = "标题为null";
                throw new Exception("非法的表格:" + file.Name + " " + attachmsg);
            }
            string[] titles = UtilLine(tile.TrimEnd(eof));

            //注释
            string comment = sr.ReadLine();
            if (string.IsNullOrEmpty(comment.TrimEnd(eof)))
            {
                attachmsg = "注释为null";
                throw new Exception("非法的表格:" + file.Name + " " + attachmsg);
            }
            string[] comments = UtilLine(comment);

            //类型
            string tp = sr.ReadLine();
            if (string.IsNullOrEmpty(tp.TrimEnd(eof)))
            {
                attachmsg = "类型为null";
                throw new Exception("非法的表格:" + file.Name + " " + attachmsg);
            }
            string[] tps = UtilLine(tp);
            uint lineCnt = 0;
            while (true)
            {
                string line = sr.ReadLine();
                if (string.IsNullOrEmpty(line)) break;
                string[] rows = UtilLine(line);
                if (rows == null || tp == null)
                {
                    attachmsg = "内容有null";
                    throw new Exception("非法的表格:" + file.Name + " " + attachmsg);
                }
                else if (rows.Length != tps.Length ||
                rows.Length != titles.Length)
                {
                    attachmsg = "字段不等长 内容：" + rows.Length + " tpye:" + tps.Length+"\n"+line;
                    throw new Exception("非法的表格:" + file.Name + " " + attachmsg);
                }

                for (int i = 0, max = rows.Length; i < max; i++)
                {
                    CSVStruct sct = new CSVStruct();
                    sct.title = titles[i];
                    sct.comment = comments[i];
                    sct.parse = TransParse(tps[i], file.Name);
                    sct.content = string.Intern(rows[i]);
                    table.list.Add(sct);
                }
                lineCnt++;
            }
            table.lineCnt = lineCnt;
            return table;
        }


        private string[] UtilLine(string line)
        {
            line = line.TrimEnd(eof);
            return line.Split(',');
        }


        private ValueParse TransParse(string str, string table)
        {
            ValueParse t = null;
            switch (str.ToLower())
            {
                case "bool": t = boolParse; break;
                case "int": t = intParse; break;
                case "string": t = stringParse; break;
                case "uint": t = uintParse; break;
                case "float": t = floatParse; break;
                case "int[]": t = arrIntParse; break;
                case "uint[]": t = arruintParse; break;
                case "float[]": t = arrFloatParse; break;
                case "string[]": t = arrStringParse; break;
                case "bool[]": t = arrBoolParse; break;
                default: throw new Exception("非法的数据类型:" + str + " from " + table);
            }
            return t;
        }



    }

}
