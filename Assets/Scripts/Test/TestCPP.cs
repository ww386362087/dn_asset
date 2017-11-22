#if TEST

using System;
using System.Runtime.InteropServices;
using UnityEngine;

public class TestCPP : ITest
{
    
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
    public struct QteStatusListMarshalRowData
    {
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 64)]
        public string Comment;

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 64)]
        public string Name;
        
        public int Value;
    }

    [DllImport("XTable")]
    public static extern int iAdd(int x, int y);

    [DllImport("XTable")]
    public static extern int iSub(IntPtr x, IntPtr y);

    [DllImport("XTable")]
    public static extern void iInitial(string stream,string persist);

    [DllImport("XTable")]
    public static extern void iReadQteStatusList();

    [DllImport("XTable")]
    public static extern void iGetQteStatusListRow(int val,ref QteStatusListMarshalRowData row);
    
    [DllImport("XTable")]
    public static extern void iInitCallbackCommand(CppDelegate cb);

    public delegate void CppDelegate(IntPtr str);

    GUILayoutOption[] ui_opt = new GUILayoutOption[] { GUILayout.Width(100), GUILayout.Height(40) };
    GUILayoutOption[] ui_op2 = new GUILayoutOption[] { GUILayout.Width(480), GUILayout.Height(240) };
    string ui_in = "23";
    string ui_rst = string.Empty;
    QteStatusListMarshalRowData m_cacheMarshalData = new QteStatusListMarshalRowData();

    public void Start()
    {
        iInitCallbackCommand(new CppDelegate(OnCallback));
        iInitial(Application.streamingAssetsPath + "/", Application.persistentDataPath + "/");
    }
    

    void OnCallback(IntPtr ptr)
    {
        string command = Marshal.PtrToStringAnsi(ptr);
        XDebug.Log(command);
    }

    void OnCallback(string command)
    {
        if (string.IsNullOrEmpty(command)) XDebug.Log("rst is null ");
        else XDebug.Log(command);
    }
    
    public void OnGUI()
    {
        GUILayout.BeginHorizontal();
        GUILayout.Space(10);
        GUILayout.BeginVertical();
        GUILayout.Space(10);
        if (GUILayout.Button("Add", ui_opt))
        {
            int i = iAdd(8, 7);
            ui_rst = "add rest: " + i;
            XDebug.LogGreen(ui_rst);
        }
        if (GUILayout.Button("Sub", ui_opt))
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
        if (GUILayout.Button("Table-QTE", ui_opt))
        {
            iReadQteStatusList();
            ui_rst = "read table finish";
            XDebug.LogGreen(ui_rst);
        }
        ui_in = GUILayout.TextField(ui_in,ui_opt);

        if (GUILayout.Button("Row-CPP", ui_opt))
        {
            int val = 23;
            int.TryParse(ui_in, out val);
            iGetQteStatusListRow(val, ref m_cacheMarshalData);
            ui_rst = m_cacheMarshalData.Value + " name: " + m_cacheMarshalData.Name + " comment: " + m_cacheMarshalData.Comment;
            XDebug.Log(ui_rst);
        }
        GUILayout.EndVertical();
        GUILayout.Space(20);
        GUILayout.TextArea(ui_rst, ui_op2);
        GUILayout.EndHorizontal();
    }

    public void Update() { }

    public void LateUpdate() { }

    public void OnQuit() { }

}

#endif