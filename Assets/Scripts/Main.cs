using UnityEngine;


public class Main : MonoBehaviour
{
    
    void Start()
    {
        ABManager.singleton.Init(this);
        Documents.singleton.Initial();
        UIManager.singleton.Initial();
        TestManager.Get().Start();
    }

    void Update()
    {
        float delta = Time.deltaTime;
        XResourceMgr.Update();
        XEntityMgr.singleton.Update(delta);
        XTouch.singleton.Update(delta);
        TestManager.Get().Update();
    }


    void LateUpdate()
    {
        XEntityMgr.singleton.PostUpdate();
    }

    void OnGUI()
    {
        TestManager.Get().OnGUI();
    }


}
