#if TEST

using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.Profiling;
using XTable;


public class TestCPP : ITest
{
    [DllImport("DnTable")]
    private static extern int add(int x, int y);

    int i = add(5, 7);


    void TStart()
    {
        Profiler.BeginSample("ForList");
        var t1 = XTableMgr.GetTable<XEntityPresentation>();
        var t2 = XTableMgr.GetTable<FashionSuit>();
        var t3 = XTableMgr.GetTable<DefaultEquip>();
        XDebug.Log("length: " + t1.Table.Length, " " + t2.Table.Length, " ", t3.Table.Length);
        Profiler.EndSample();
    }


    public void Start()
    {
        //   XDebug.Log("bb"+Vector3.Cross(Vector3.up, Vector3.forward));
        List<int> list = new List<int>();
        list.Add(3);
        list.Add(5);
        list.Add(4);
        list.Sort((x, y) => x - y);
        for(int i=0;i<3;i++)
        {
            XDebug.Log(list[i]);
        }
    }


    public void OnGUI()
    {
        if (GUI.Button(new Rect(1, 1, 200, 100), "this dll i = 5+7, i is" + i))
        {
            TStart();
        }
    }

    public void Update()
    {

    }

    public void LateUpdate()
    {
    }
}


#endif