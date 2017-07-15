using UnityEngine;


public class Main : MonoBehaviour
{
    
    void Start()
    {
        ABManager.singleton.Init(this);
        
        Test.singleton.Initial();
        Test.singleton.TestAB();
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
