#if TEST

using System;
using System.Runtime.InteropServices;
using UnityEngine;
using XTable;

public class TestCPP : ITest
{
    
    [DllImport("XTable")]
    public static extern void iInitCallbackCommand(CppDelegate cb);

    [DllImport("XTable")]
    public static extern void iInitial(string stream,string persist);

    [DllImport("XTable")]
    public static extern int iAdd(int x, int y);

    [DllImport("XTable")]
    public static extern int iSub(IntPtr x, IntPtr y);
    
    public delegate void CppDelegate(IntPtr str);

    GUILayoutOption[] ui_opt = new GUILayoutOption[] { GUILayout.Width(160), GUILayout.Height(80) };
    GUILayoutOption[] ui_op2 = new GUILayoutOption[] { GUILayout.Width(480), GUILayout.Height(240) };
    GUIStyle ui_sty = new GUIStyle();
    GUIStyle ui_st2 = new GUIStyle();
    string ui_qte = "2";
    string ui_suit="5";
    string ui_rst = string.Empty;
   
    public void Start()
    {
        ui_sty.fontSize = 21;
        ui_sty.fontStyle = FontStyle.Bold;
        ui_sty.alignment = TextAnchor.MiddleCenter;
        ui_st2.normal.textColor = Color.red;
        ui_st2.fontSize = 24;
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
            ui_rst = "8+7=" + i;
            XDebug.LogGreen(ui_rst);
        }
        if (GUILayout.Button("Cal-Sub", ui_opt))
        {
            int a = 8, b = 2;
            IntPtr p1 = Marshal.AllocCoTaskMem(Marshal.SizeOf(a));
            Marshal.StructureToPtr(a, p1, false);
            IntPtr p2 = Marshal.AllocCoTaskMem(Marshal.SizeOf(b));
            Marshal.StructureToPtr(b, p2, false);
            int rst = iSub(p1, p2);
            ui_rst = a + "-" + b + "=" + rst;
            XDebug.Log(ui_rst);
        }
        ui_qte = GUILayout.TextField(ui_qte, ui_sty, ui_opt);
        if (GUILayout.Button("Get-Qte-Row", ui_opt))
        {
            int val = 2;
            int.TryParse(ui_qte, out val);
            int len = CQteStatusList.length;
            var rst = CQteStatusList.GetRow(val);
            ui_rst = "qte length: " + len + "\n";
            ui_rst += "value:\t"+rst.Value + "\nname:\t" + rst.Name + "\ncomment:\t" + rst.Comment;
            XDebug.Log(ui_rst);
        }
        ui_suit = GUILayout.TextField(ui_suit, ui_sty, ui_opt);
        if (GUILayout.Button("Get-Suit-Row", ui_opt))
        {
            int val = 5;
            int.TryParse(ui_suit, out val);
            int len = CEquipSuit.length;
            var rst = CEquipSuit.GetRow(val);
            ui_rst = "suit length: " + len + "\n";
            ui_rst += "suitid:\t" + rst.SuitID + "\nname:\t" + rst.SuitName + "\nlevel:\t" + rst.Level;
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