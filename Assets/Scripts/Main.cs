using UnityEngine;


public class Main : MonoBehaviour
{
    
    void Start()
    {
        ABManager.singleton.Init(this);
        Documents.singleton.Initial();
        UIManager.singleton.Initial();

#if TEST
        TestManager.Get().Start();
#endif
    }

    void Update()
    {
        float delta = Time.deltaTime;
        XResourceMgr.Update();
        XEntityMgr.singleton.Update(delta);
        XTouch.singleton.Update(delta);

#if TEST
        TestManager.Get().Update();
#endif
    }


    void LateUpdate()
    {
        XEntityMgr.singleton.LateUpdate();
    }

    void OnGUI()
    {
#if TEST
        TestManager.Get().OnGUI();
#endif
    }


}
