#if TEST

using System;
using System.Runtime.InteropServices;
using System.Text;
using UnityEngine;
using XTable;

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
    public static extern void iInitial(string path);

    [DllImport("XTable")]
    public static extern void iReadQteStatusList(string path);

    [DllImport("XTable")]
    public static extern void iGetQteStatusListRow(int val,ref QteStatusListMarshalRowData row);
    
    [DllImport("XTable")]
    public static extern void iInitCallbackCommand(CppDelegate cb);

    public delegate void CppDelegate(IntPtr str);

    QteStatusList m_table;
    GUILayoutOption[] ui_opt = new GUILayoutOption[] { GUILayout.Width(100), GUILayout.Height(40) };

    QteStatusListMarshalRowData m_cacheMarshalData = new QteStatusListMarshalRowData();

    public void Start()
    {
        iInitCallbackCommand(new CppDelegate(OnCallback));
        iInitial(Application.streamingAssetsPath + "/");
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
            iReadQteStatusList(path);
            XDebug.Log("read table finish");
        }
        if (GUILayout.Button("Table-C#", ui_opt))
        {
            m_table = XTableMgr.GetTable<QteStatusList>();
            TestBytes();
        }
        if (GUILayout.Button("Row-CPP", ui_opt))
        {
            iGetQteStatusListRow(24, ref m_cacheMarshalData);
            XDebug.Log(m_cacheMarshalData.Value+" name: "+m_cacheMarshalData.Name+" comment: "+m_cacheMarshalData.Comment);
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