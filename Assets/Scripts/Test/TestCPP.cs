#if TEST

using System;
using System.Runtime.InteropServices;
using System.Text;
using UnityEngine;
using XTable;

public class TestCPP : ITest
{
    [DllImport("XTable")]
    public static extern int iAdd(int x, int y);

    [DllImport("XTable")]
    public static extern void iInitial(string path);

    [DllImport("XTable")]
    public static extern void iReadTable(string path);

    [DllImport("XTable")]
    public static extern void iGetComment(string ptr, int val);

    [DllImport("XTable")]
    public static extern IntPtr GetStr();

    [DllImport("XTable")]
    public static extern void iInitCallbackCommand(CppDelegate cb);

    public delegate void CppDelegate(IntPtr str);

    QteStatusList m_table;
    GUILayoutOption[] ui_opt = new GUILayoutOption[] { GUILayout.Width(100), GUILayout.Height(40) };

    public void Start()
    {
        iInitCallbackCommand(new CppDelegate(OnCallback));
        iInitial(Application.streamingAssetsPath + "/");
        IntPtr str = GetStr();
        string str1 = Marshal.PtrToStringAnsi(str);
        XDebug.Log(str1);
    }


    private void TestBytes()
    {
        if (m_table != null)
        {
            for (int i = 0, max = m_table.length; i < max; i++)
            {
                var bytes = Encoding.Default.GetBytes(m_table[i].Comment);
                XDebug.LogGreen(m_table[i].Comment);
                PrintBytes(bytes);
            }
        }
    }

    public void PrintBytes(byte[] bytes)
    {
        StringBuilder sb = new StringBuilder();
        for (int i = 0; i < bytes.Length; i++)
        {
            sb.Append("0x");
            sb.AppendFormat("{0:x2}", bytes[i]); //十六进制
            sb.Append(",");
        }
        XDebug.Log(sb.ToString());
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


    [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 64)]
    string str_comment;

    public void OnGUI()
    {
        GUILayout.BeginHorizontal();
        GUILayout.BeginVertical();
        if (GUILayout.Button("Add", ui_opt))
        {
            int i = iAdd(8, 7);
            XDebug.Log("add rest: " + i);
        }
        if (GUILayout.Button("Table-CPP", ui_opt))
        {
            string path = Application.streamingAssetsPath + "/Table/QteStatusList.bytes";
            iReadTable(path);
            XDebug.Log("read table finish");
        }
        if (GUILayout.Button("Table-C#", ui_opt))
        {
            m_table = XTableMgr.GetTable<QteStatusList>();
            TestBytes();
        }
        if (GUILayout.Button("Row-CPP", ui_opt))
        {
            iGetComment(str_comment, 24);
            XDebug.Log(str_comment);
        }
        GUILayout.EndVertical();
        GUILayout.Space(20);
        if (m_table != null)
        {
            GUILayout.BeginVertical();
            for (int i = 0, max = m_table.length; i < max; i++)
            {
                GUILayout.Label(m_table[i].Value + "\t" + m_table[i].Name + "\t\t" + m_table[i].Comment);
            }
            GUILayout.EndVertical();
        }
        GUILayout.EndHorizontal();
    }

    public void Update() { }

    public void LateUpdate() { }

    public void OnQuit() { }

}

#endif