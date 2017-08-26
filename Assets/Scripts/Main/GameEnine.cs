using System.Collections;

public class GameEnine : XObject
{

    private static GameEntrance _entrance;
    private static bool _init_finish = false;

    public static GameEntrance entrance { get { return _entrance; } }

    public static void Init(GameEntrance en)
    {
        _entrance = en;

        TimerManager.singleton.Init();
        ABManager.singleton.Initial();
        Documents.singleton.Initial();
        UIManager.singleton.Initial();

        _init_finish = true;
    }




    public static void Update(float delta)
    {

        if (!_init_finish) return;

        //xtouch must be update first
        XTouch.singleton.Update(delta);

        TimerManager.singleton.Update(delta);
        XResourceMgr.Update();
        XEntityMgr.singleton.Update(delta);
        XScene.singleton.Update(delta);
    }


    public static void LateUpdate()
    {
        if (!_init_finish) return;

        XEntityMgr.singleton.LateUpdate();
        XScene.singleton.LateUpdate();
    }

    public static void OnUnintial()
    {

    }




}
