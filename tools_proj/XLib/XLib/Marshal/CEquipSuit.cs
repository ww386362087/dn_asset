namespace XTable
{
    using System.Collections.Generic;
    using System.Runtime.InteropServices;

    public class CEquipSuit
    {

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
        public struct RowData
        {
            int suitid;

            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 64)]
            string suitname;

            int level;
            int profid;
            int suitquality;
            bool iscreate;

            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
            int[] euipid;

            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 64)]
            string effect1;

            CSeq<int> effect2;
            CSeq<int> effect3;
            CSeq<int> effect4;
            CSeq<int> effect5;
            CSeq<int> effect6;
            CSeq<int> effect7;
            CSeq<int> effect8;
            CSeq<int> effect9;
            CSeq<int> effect10;


            public int SuitID { get { return suitid; } }

            public string SuitName { get { return suitname; } }

            public int Level { get { return level; } }

            public int ProfID { get { return profid; } }

            public int SuitQuality { get { return suitquality; } }

            public bool IsCreate { get { return iscreate; } }

            public int[] Equipid {
                get {
                    if (euipid.Length == 16) {
                        List<int> list = new List<int>();
                        for (int i = euipid.Length - 1; i >= 0; i--)
                        {
                            if (euipid[i] != -1) list.Add(euipid[i]);
                        }
                        euipid = list.ToArray();
                    }
                    return euipid;
                }
            }

            public string Effect1 { get { return effect1; } }
            public CSeq<int> Effect2 { get { return effect2; } }
            public CSeq<int> Effect3 { get { return effect3; } }
            public CSeq<int> Effect4 { get { return effect4; } }
            public CSeq<int> Effect5 { get { return effect5; } }
            public CSeq<int> Effect6 { get { return effect6; } }
            public CSeq<int> Effect7 { get { return effect7; } }
            public CSeq<int> Effect8 { get { return effect8; } }
            public CSeq<int> Effect9 { get { return effect9; } }
            public CSeq<int> Effect10 { get { return effect10; } }
        }

        static RowData m_data = new RowData();

        [DllImport("XTable")]
        public static extern void iGetEquipSuitRow(int val, ref RowData row);

        [DllImport("XTable")]
        static extern int iGetEquipSuitLength();

        public static int length { get { return iGetEquipSuitLength(); } }

        public static RowData GetRow(int val)
        {
            iGetEquipSuitRow(val, ref m_data);
            return m_data;
        }
    }

}