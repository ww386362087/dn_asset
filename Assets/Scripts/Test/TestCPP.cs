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
    GUIStyle ui_sty = new GUIStyle();
    GUIStyle ui_st2 = new GUIStyle();
    string ui_in = "23";
    string ui_rst = string.Empty;
    QteStatusListMarshalRowData m_cacheMarshalData = new QteStatusListMarshalRowData();

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
        XDebug.Log("[cpp]:"+command);
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
        if (GUILayout.Button("Read-QTE", ui_opt))
        {
            iReadQteStatusList();
            ui_rst = "read table finish";
            XDebug.LogGreen(ui_rst);
        }
        ui_in = GUILayout.TextField(ui_in, ui_sty, ui_opt);

        if (GUILayout.Button("Get-Row", ui_opt))
        {
            int val = 23;
            int.TryParse(ui_in, out val);
            iGetQteStatusListRow(val, ref m_cacheMarshalData);
            ui_rst = "value:\t"+m_cacheMarshalData.Value + "\nname:\t" + m_cacheMarshalData.Name + "\ncomment:\t" + m_cacheMarshalData.Comment;
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