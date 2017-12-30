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

        public readonly char[] SequenceSeparator = new char[] { '=' };
        public readonly char[] ListSeparator = new char[] { '|' };
        public readonly char[] AllSeparators = new char[] { '|', '=' };
        public static readonly char[] SpaceSeparator = new char[] { ' ' };
        public readonly char[] eof = { '\r', '\n' };

        public static int max_string_size = 64;
        public static int max_array_size = 16;

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
        private SequenceUintParse seqUintParse;
        private SequenceIntParse seqIntParse;
        private SequenceFloatParse seqFloatParse;
        private SequenceBoolParse seqBoolParse;
        private SequenceStringParse seqStringParse;

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
            seqFloatParse = new SequenceFloatParse();
            seqIntParse = new SequenceIntParse();
            seqUintParse = new SequenceUintParse();
            seqBoolParse = new SequenceBoolParse();
            seqStringParse = new SequenceStringParse();
        }


        public CSVTable UtilCsv(FileInfo file)
        {
            //针对ANSI编码的csv 不要用Unicode编码
            StreamReader sr = new StreamReader(file.FullName, Encoding.Default);

            CSVTable table = new CSVTable(file.Name);
            string attachmsg = string.Empty;
            bool isSort = true;

            //字段
            string tile = sr.ReadLine();
            if (string.IsNullOrEmpty(tile.TrimEnd(eof)))
            {
                attachmsg = "标题为null";
                throw new Exception("非法的表格:" + file.Name + " " + attachmsg);
            }
            //注释
            string comment = sr.ReadLine();
            if (string.IsNullOrEmpty(comment.TrimEnd(eof)))
            {
                attachmsg = "注释为null";
                throw new Exception("非法的表格:" + file.Name + " " + attachmsg);
            }
            //client使用还是server使用
            string mode = sr.ReadLine();
            if (string.IsNullOrEmpty(mode.TrimEnd(eof)))
            {
                attachmsg = "读表地方null";
                throw new Exception("非法的表格:" + file.Name + " " + attachmsg);
            }
            string[] modes = UtilLine(mode);
            bool[] useList = new bool[modes.Length];
            int useColumeCnt = 0;
            for (int i = 0; i < modes.Length; i++)
            {
                useList[i] = modes[i].Equals("A") || modes[i].Equals("C");
                if (useList[i])
                {
                    useColumeCnt++;
                }
            }
            //类型
            string tp = sr.ReadLine();
            if (string.IsNullOrEmpty(tp.TrimEnd(eof)))
            {
                attachmsg = "类型为null";
                throw new Exception("非法的表格:" + file.Name + " " + attachmsg);
            }

            string[] titles = RemoveUnuseless(UtilLine(tile.TrimEnd(eof)), useList);
            if (titles.Length > 0) isSort &= titles[0].Contains("ID");
            string[] comments = RemoveUnuseless(UtilLine(comment), useList);
            string[] tps = RemoveUnuseless(UtilLine(tp), useList);

            ValueParse[] parses = new ValueParse[tps.Length];
            for (int i = 0, max = parses.Length; i < max; i++)
            {
                parses[i] = TransParse(tps[i], file.Name);
            }
            if (tps.Length > 0) isSort &= (tps[0].Equals("int") || tps[0].Equals("uint"));
            int lineCnt = 0;
            table.sortlist = new List<CVSSortRow>();
            while (true)
            {
                string line = sr.ReadLine();
                if (string.IsNullOrEmpty(line)) break;
                string[] colums = RemoveUnuseless(UtilLine(line), useList);
                if (colums == null || tp == null)
                {
                    attachmsg = "内容有null";
                    throw new Exception("非法的表格:" + file.Name + " " + attachmsg);
                }
                else if (colums.Length != tps.Length || colums.Length != titles.Length)
                {
                    attachmsg = "字段不等长 内容：" + colums.Length + " tpye:" + tps.Length + "\n" + line;
                    throw new Exception("非法的表格:" + file.Name + " " + attachmsg);
                }
                CVSSortRow sortRow = new CVSSortRow();
                sortRow.row = new CSVStruct[useColumeCnt];
                for (int i = 0, max = colums.Length; i < max; i++)
                {
                    CSVStruct sct = new CSVStruct();
                    sct.title = titles[i];
                    sct.comment = comments[i];
                    sct.parse = TransParse(tps[i], file.Name);
                    sct.content = string.Intern(colums[i]);
                    if (i == 0) sortRow.sortid = isSort ? int.Parse(colums[i]) : 0;
                    sortRow.row[i] = sct;
                }
                table.sortlist.Add(sortRow);
                lineCnt++;
            }
            table.isSort = isSort;
            table.rowCnt = lineCnt;
            table.colCnt = titles.Length;
            table.titles = titles;
            table.comments = comments;
            table.types = tps;
            table.parses = parses;
            if (isSort) table.sortlist.Sort(table.Sort);
            return table;
        }


        private string[] UtilLine(string line)
        {
            line = line.TrimEnd(eof);
            return line.Split(',');
        }

        private string[] RemoveUnuseless(string[] arr, bool[] uselist)
        {
            List<string> list = new List<string>();
            for (int i = 0, max = arr.Length; i < max; i++)
            {
                if (uselist.Length > i && uselist[i])
                    list.Add(arr[i]);
            }
            return list.ToArray();
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
                case "uint<>": t = seqUintParse; break;
                case "int<>": t = seqIntParse; break;
                case "float<>": t = seqFloatParse; break;
                case "bool<>": t = seqBoolParse; break;
                case "string<>": t = seqStringParse; break;
                default: throw new Exception("非法的数据类型:" + str + " from " + table);
            }
            return t;
        }

    }

}
