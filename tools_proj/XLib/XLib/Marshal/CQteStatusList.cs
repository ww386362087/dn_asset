namespace XTable
{
    using System.Collections.Generic;
    using System.Runtime.InteropServices;


    public class CQteStatusList
    {

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
        public struct RowData
        {
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 64)]
            string comment;

            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 64)]
            string name;

            int value;

            public string Comment { get { return comment; } }

            public string Name { get { return name; } }

            public int Value { get { return value; } }
        }


        [DllImport("XTable")]
        static extern void iGetQteStatusListRow(int val, ref RowData row);

        [DllImport("XTable")]
        static extern int iGetQteStatueListLength();

        static RowData m_data;

        public static int length { get { return iGetQteStatueListLength(); } }

        public static RowData GetRow(int val)
        {
            iGetQteStatusListRow(val, ref m_data);
            return m_data;
        }
    }
}