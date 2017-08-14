

public class GameEnine : XObject
{

    private static GameEntrance _entrance;

	public static void Init(GameEntrance en)
    {
        _entrance = en;

        ABManager.singleton.Init(en);
        Documents.singleton.Initial();
        UIManager.singleton.Initial();
    }



    public static void Update(float delta)
    {
        XResourceMgr.Update();
        XEntityMgr.singleton.Update(delta);
        XTouch.singleton.Update(delta);
    }


    public static void LateUpdate()
    {
        XEntityMgr.singleton.LateUpdate();
    }



}
