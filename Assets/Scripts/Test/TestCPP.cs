using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.Profiling;
using XTable;


public class TestCPP : MonoBehaviour
{
    [DllImport("DnTable")]
    private static extern int add(int x, int y);

  int i = add(5, 7);


    void TStart()
    {
       // Profiler.BeginSample("ForList");
        XEntityPresentation tab = new XEntityPresentation();
        FashionSuit tab2 = new FashionSuit();
        DefaultEquip tab3 = new DefaultEquip();
     //   Profiler.EndSample();
    }

    void OnGUI()
    {
        if (GUI.Button(new Rect(1, 1, 200, 100), "this dll i = 5+7, i is" + i))
        {
            TStart();
        }
       
    }
}
