using UnityEngine;


public class Main : MonoBehaviour
{
    


    void Start()
    {
        ABManager.singleton.Init(this);
        
        Test.singleton.Initial();
 
    }

    void Update()
    {
        float delta = Time.deltaTime;
        XResourceMgr.Update();
        XEntityMgr.singleton.Update(delta);
    }

  
    void OnGUI()
    {
        Test.singleton.GUI();
    }

    
}
