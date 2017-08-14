#if TEST

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
        new XEntityPresentation();
        new FashionSuit();
        new DefaultEquip();
        Profiler.EndSample();
    }


    public void Start()
    {
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

}


#endif