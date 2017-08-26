#if TEST

public class TestScene : ITest
{
   
    const int sceneid = 401;

    public void Start()
    {
        XScene.singleton.Enter(sceneid);
    }

    public void OnGUI() { }


    public void Update() { }

    
}



#endif