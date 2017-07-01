using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XForm
{

    public struct CSVStruct
    {
        public ValueParse parse;
        public string title;
        public string comment;
        public string content;
    }


    public class CSVTable
    {
        public string name;
        public uint lineCnt;
        public List<CSVStruct> list;

        public CSVTable(string _name)
        {
            this.name = _name;
        }
    }
}
