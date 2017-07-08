using UnityEngine;


public class Main : MonoBehaviour
{
   
    void Start()
    {
        Test.singleton.Initial();
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
