

public class GameEnine : XObject
{

    private static GameEntrance _entrance;

    public static GameEntrance entrance { get { return _entrance; } }

    public static void Init(GameEntrance en)
    {
        _entrance = en;

        ABManager.singleton.Initial();
        Documents.singleton.Initial();
        UIManager.singleton.Initial();
    }




    public static void Update(float delta)
    {
        //xtouch must be update first
        XTouch.singleton.Update(delta);

        XResourceMgr.Update();
        XEntityMgr.singleton.Update(delta);
        XScene.singleton.Update(delta);
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
