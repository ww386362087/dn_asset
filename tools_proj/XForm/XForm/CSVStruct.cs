using System.Collections.Generic;

namespace XForm
{

    public struct CSVStruct
    {
        /// <summary>
        /// 解析结构
        /// </summary>
        public ValueParse parse;
        /// <summary>
        /// title
        /// </summary>
        public string title;
        /// <summary>
        /// 注释
        /// </summary>
        public string comment;
        /// <summary>
        /// 内容
        /// </summary>
        public string content;
    }


    public struct CVSSortRow
    {
        /// <summary>
        /// 排序id
        /// </summary>
        public int sortid;
        /// <summary>
        /// 一行的内容
        /// </summary>
        public CSVStruct[] row;
    }

    public class CSVTable
    {
        /// <summary>
        /// 是否排序
        /// </summary>
        public bool isSort;
        /// <summary>
        /// 所有的标题
        /// </summary>
        public string[] titles;
        /// <summary>
        /// 所有的注释
        /// </summary>
        public string[] comments;
        /// <summary>
        /// 字段类型
        /// </summary>
        public string[] types;
        /// <summary>
        /// 所有的解析类型
        /// </summary>
        public ValueParse[] parses;
        /// <summary>
        /// 表格名
        /// </summary>
        public string name;
        /// <summary>
        /// 行数
        /// </summary>
        public int rowCnt;
        /// <summary>
        /// 列数
        /// </summary>
        public int colCnt;
        /// <summary>
        /// 表的内容
        /// </summary>
        public List<CVSSortRow> sortlist;

        public CSVTable(string _name)
        {
            this.name = _name;
        }

        public int Sort(CVSSortRow row1, CVSSortRow row2)
        {
            return row1.sortid - row2.sortid;
        }
    }

}

