#if TEST

using System.Runtime.InteropServices;
using UnityEngine;
using System.IO;
using System.Text;

public class TestCPP : ITest
{
    [DllImport("XTable")]
    private static extern int iAdd(int x, int y);

    [DllImport("XTable")]
    private static extern int iInitial(string path);

    [DllImport("XTable")]
    private static extern void iReadTable(string path);

    [DllImport("XTable")]
    public static extern void iInitCallbackCommand(callbackDelegate cb);

    public delegate void callbackDelegate(string str);

    public void Start()
    {
        iInitCallbackCommand(new callbackDelegate(OnCallback));
    }

    void OnCallback(string commad)
    {
        XDebug.Log(commad);
    }
    
    public void OnGUI()
    {
        if (GUI.Button(new Rect(20, 10, 200, 100), "init"))
        {
            int rst = iInitial(Application.streamingAssetsPath+"/");
            XDebug.Log("to initial finish "+(rst==1));
        }
        if (GUI.Button(new Rect(20, 120, 200, 100), "test"))
        {
            int i = iAdd(8, 7);
            XDebug.Log("add rest: " + i);
        }
        if (GUI.Button(new Rect(20, 230, 200, 100), "read"))
        {
            string path = Application.streamingAssetsPath + "/Table/QteStatusList.bytes";
            iReadTable(path);
            XDebug.Log("read table finish");
        }
    }

    public void Update() { }

    public void LateUpdate() { }

    public void OnQuit()
    {
        
    }

}


#endif