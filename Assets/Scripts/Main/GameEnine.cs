using UnityEngine;

public class GameEnine : XObject
{

    private static MonoBehaviour _entrance;

    public static MonoBehaviour entrance { get { return _entrance; } }

    public static void Init(MonoBehaviour en)
    {
        _entrance = en;

        XTimerMgr.singleton.Init();
        XGlobalConfig.Initial();
        XTableMgr.Initial();
        ABManager.singleton.Initial();
        Documents.singleton.Initial();
        UIManager.singleton.Initial();
        
    }

    

    public static void Update(float delta)
    {
        //xtouch must be update first
        XTouch.singleton.Update(delta);

        XTimerMgr.singleton.Update(delta);
        XResourceMgr.Update();
        XEntityMgr.singleton.Update(delta);
        XScene.singleton.Update(delta);
        XAutoFade.Update();
    }


    public static void LateUpdate()
    {
        XEntityMgr.singleton.LateUpdate();
        XScene.singleton.LateUpdate();
    }

    public static void OnUnintial()
    {

    }




}
