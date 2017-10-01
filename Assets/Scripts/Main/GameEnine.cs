using UnityEngine;

public sealed class GameEnine : XObject
{

    private static MonoBehaviour _entrance;

    public static MonoBehaviour entrance { get { return _entrance; } }

    public static void Init(MonoBehaviour en)
    {
        _entrance = en;

        XTimerMgr.singleton.Init();
        XConfig.Initial(LogLevel.Log, LogLevel.Error);
        XGlobalConfig.Initial();
        XTableMgr.Initial();

        XResources.Init();
        UIManager.singleton.Initial();
        Documents.singleton.Initial();
    }



    public static void Update(float delta)
    {
        //xtouch must be update first
        XTouch.singleton.Update(delta);

        XTimerMgr.singleton.Update(delta);
        XResources.Update();
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
    
    public static void OnApplicationQuit()
    {
        XDebug.Log("game quit!");
    }

    public static void SetMonoForTest(MonoBehaviour mono)
    {
        _entrance = mono;
    }

}

