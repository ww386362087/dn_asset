#if TEST

using AOT;
using System;
using System.Runtime.InteropServices;
using UnityEngine;
using XTable;

public class TestCPP : ITest
{
#if UNITY_IPHONE || UNITY_XBOX360
	[DllImport("__Internal")]
#else
    [DllImport("XTable")]
#endif
    public static extern void iInitCallbackCommand(CppDelegate cb);

#if UNITY_IPHONE || UNITY_XBOX360
	[DllImport("__Internal")]
#else
    [DllImport("XTable")]
#endif
    public static extern void iInitial(string stream,string persist);

#if UNITY_IPHONE || UNITY_XBOX360
	[DllImport("__Internal")]
#else
    [DllImport("XTable")]
#endif
    public static extern int iAdd(int x, int y);

#if UNITY_IPHONE || UNITY_XBOX360
	[DllImport("__Internal")]
#else
    [DllImport("XTable")]
#endif
    public static extern int iSub(IntPtr x, IntPtr y);
    
    public delegate void CppDelegate(IntPtr str);

    GUILayoutOption[] ui_opt = new GUILayoutOption[] { GUILayout.Width(160), GUILayout.Height(80) };
    GUILayoutOption[] ui_op2 = new GUILayoutOption[] { GUILayout.Width(480), GUILayout.Height(240) };
    GUIStyle ui_sty = new GUIStyle();
    string ui_rst = string.Empty;
   
    public void Start()
    {
        ui_sty.normal.textColor = Color.red;
        ui_sty.fontSize = 20;
        iInitCallbackCommand(new CppDelegate(OnCallback));
        iInitial(Application.streamingAssetsPath + "/", Application.persistentDataPath + "/");
    }


    [MonoPInvokeCallback(typeof(CppDelegate))]
    static void OnCallback(IntPtr ptr)
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
        }
        if (GUILayout.Button("Get-Qte-Row", ui_opt))
        {
            int len = CQteStatusList.length;
            ui_rst = "\nqte status list table line cnt: " + len + "\n";
            for (int i = 0; i < 22; i++)
            {
                var rst = CQteStatusList.GetRow(i);
                ui_rst += "\nvalue:" + rst.Value + "name:\t" + rst.Name + "comment:\t" + rst.Comment;
            }
        }
        if (GUILayout.Button("Get-Suit-Row", ui_opt))
        {
            int len = CEquipSuit.length;
            ui_rst = "\nequi suit table line cnt: " + len + "\n";
            for (int i = 0; i < 22; i++)
            {
                var rst = CEquipSuit.GetRow(i);
                ui_rst += "\nsuitid:" + rst.SuitID + " name:" + rst.SuitName + "level:" + rst.Level + " profid:" + rst.ProfID + " isCreate:" + rst.IsCreateShow;
            }
        }
        GUILayout.EndVertical();
        GUILayout.Space(50);
        GUILayout.TextArea(ui_rst,ui_sty, ui_op2);
        GUILayout.EndHorizontal();
    }

    public void Update() { }

    public void LateUpdate() { }

    public void OnQuit() { }

}

#endif