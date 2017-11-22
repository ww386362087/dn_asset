#if TEST

using System;
using System.Runtime.InteropServices;
using UnityEngine;

public class TestCPP : ITest
{

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
    public class Seq<T>
    {
        public T val0, val1;

        public void Set(T v1, T v2)
        {
            val0 = v1;
            val1 = v2;
        }

        public T this[int i]
        {
            get { return i == 0 ? val0 : val1; }
        }
    }


    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
    public struct QteStatusListMarshalRowData
    {
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 64)]
        public string Comment;

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 64)]
        public string Name;
        
        public int Value;
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
    public struct EquipSuitMarshalRowData
    {
        public int suitid;

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 64)]
        public string suitname;

        public int level;
        public int profid;
        public int suitquality;
        public bool iscreate;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
        public int[] euipid;

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 64)]
        public string effect1;

        Seq<int> effect2;
        Seq<int> effect3;
        Seq<int> effect4;
        Seq<int> effect5;
        Seq<int> effect6;
        Seq<int> effect7;
        Seq<int> effect8;
        Seq<int> effect9;
        Seq<int> effect10;
    }
    
    [DllImport("XTable")]
    public static extern void iInitCallbackCommand(CppDelegate cb);

    [DllImport("XTable")]
    public static extern void iInitial(string stream,string persist);

    [DllImport("XTable")]
    public static extern int iAdd(int x, int y);

    [DllImport("XTable")]
    public static extern int iSub(IntPtr x, IntPtr y);
    
    [DllImport("XTable")]
    public static extern void iGetQteStatusListRow(int val,ref QteStatusListMarshalRowData row);

    [DllImport("XTable")]
    public static extern void iGetEquipSuitRow(int val, ref EquipSuitMarshalRowData row);

    public delegate void CppDelegate(IntPtr str);

    GUILayoutOption[] ui_opt = new GUILayoutOption[] { GUILayout.Width(160), GUILayout.Height(80) };
    GUILayoutOption[] ui_op2 = new GUILayoutOption[] { GUILayout.Width(480), GUILayout.Height(240) };
    GUIStyle ui_sty = new GUIStyle();
    GUIStyle ui_st2 = new GUIStyle();
    string ui_qte = "23";
    string ui_suit = "80124";
    string ui_rst = string.Empty;
    QteStatusListMarshalRowData m_qteMarshalData = new QteStatusListMarshalRowData();
    EquipSuitMarshalRowData m_suitMarshalData = new EquipSuitMarshalRowData();

    public void Start()
    {
        ui_sty.fontSize = 21;
        ui_sty.fontStyle = FontStyle.Bold;
        ui_sty.alignment = TextAnchor.MiddleCenter;
        ui_st2.normal.textColor = Color.red;
        ui_st2.fontSize = 18;
        iInitCallbackCommand(new CppDelegate(OnCallback));
        iInitial(Application.streamingAssetsPath + "/", Application.persistentDataPath + "/");
    }

    void OnCallback(IntPtr ptr)
    {
        string command = Marshal.PtrToStringAnsi(ptr);
        XDebug.LogC(command);
    }
    
    public void OnGUI()
    {
        GUILayout.BeginHorizontal();
        GUILayout.Space(10);
        GUILayout.BeginVertical();
        if (GUILayout.Button("Cal-Add", ui_opt))
        {
            int i = iAdd(8, 7);
            ui_rst = "add rst: " + i;
            XDebug.LogGreen(ui_rst);
        }
        if (GUILayout.Button("Cal-Sub", ui_opt))
        {
            int a = 5, b = 6;
            IntPtr p1 = Marshal.AllocCoTaskMem(Marshal.SizeOf(a));
            Marshal.StructureToPtr(a, p1, false);
            IntPtr p2 = Marshal.AllocCoTaskMem(Marshal.SizeOf(b));
            Marshal.StructureToPtr(b, p2, false);
            int rst = iSub(p1,p2);
            ui_rst = "sub rst: " + rst;
            XDebug.Log(ui_rst);
        }

        ui_qte = GUILayout.TextField(ui_qte, ui_sty, ui_opt);
        if (GUILayout.Button("Get-Qte-Row", ui_opt))
        {
            int val = 23;
            int.TryParse(ui_qte, out val);
            iGetQteStatusListRow(val, ref m_qteMarshalData);
            ui_rst = "value:\t"+m_qteMarshalData.Value + "\nname:\t" + m_qteMarshalData.Name + "\ncomment:\t" + m_qteMarshalData.Comment;
            XDebug.Log(ui_rst);
        }

        ui_suit = GUILayout.TextField(ui_suit, ui_sty, ui_opt);
        if (GUILayout.Button("Get-Suit-Row", ui_opt))
        {
            int val = 80125;
            int.TryParse(ui_suit, out val);
            iGetEquipSuitRow(val, ref m_suitMarshalData);
            ui_rst = "suitid:\t" + m_suitMarshalData.suitid + "\nname:\t" + m_suitMarshalData.suitname + "\nlevel:\t" + m_suitMarshalData.level;
            XDebug.Log(ui_rst);
        }
        GUILayout.EndVertical();
        GUILayout.Space(20);
        GUILayout.TextArea(ui_rst,ui_st2, ui_op2);
        GUILayout.EndHorizontal();
    }

    public void Update() { }

    public void LateUpdate() { }

    public void OnQuit() { }

}

#endif