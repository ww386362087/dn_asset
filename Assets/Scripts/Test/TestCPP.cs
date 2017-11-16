#if TEST

using System.Runtime.InteropServices;
using UnityEngine;


public class TestCPP : ITest
{
    [DllImport("XTable")]
    private static extern int iAdd(int x, int y);

    [DllImport("XTable")]
    private static extern int iInitial();

    [DllImport("XTable")]
    private static extern void iReadTable();


    public void Start()
    {
    }


    public void OnGUI()
    {
        if(GUI.Button(new Rect(20,10,200,100),"init"))
        {
           int rst = iInitial();
            XDebug.Log("to initial finish "+rst);
        }
        if (GUI.Button(new Rect(20, 120, 200, 100), "test"))
        {
            int i = iAdd(8, 7);
            XDebug.Log("add rest: " + i);
        }
        if (GUI.Button(new Rect(20, 230, 200, 100), "read"))
        {
            iReadTable();
            XDebug.Log("read table finish");
        }
    }

    public void Update() { }

    public void LateUpdate() { }

}


#endif