#if TEST

public class TestAI : ITest
{
    const int sceneid = 401;
    public void Start()
    {
        XScene.singleton.Enter(sceneid);
    }

    public void Update()
    {
    }

    public void LateUpdate()
    {
    }

    public void OnGUI()
    {
    }

}
#endif