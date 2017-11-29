#if TEST

public class TestAI : ITest
{
    const int sceneid = 401;


    public void Start()
    {
        XScene.singleton.Enter(sceneid);

        var equip = XEntityMgr.singleton.Player.GetComponent<XEquipComponent>();
        equip.AttachWeapon("ar_costume_baseball_a_bigbow_weapon");
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

    public void OnQuit()
    {

    }

}
#endif