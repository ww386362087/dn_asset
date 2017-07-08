using UnityEngine;


public class Main : MonoBehaviour
{
   
    void Start()
    {
        Test.singleton.Init();
    }

    void Update()
    {
        XResourceMgr.Update();
    }

  
    void OnGUI()
    {
        Test.singleton.GUI();
    }
}
