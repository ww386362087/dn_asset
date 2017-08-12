using XTable;
using UnityEngine;

public class TestScene : ITest
{

    XPlayer player;
    Camera came;
    const int sceneid = 401;

    public void Start()
    {
        XScene.singleton.EnterScene(sceneid);
        //create player and monster here
        CreatePlayer();

        XScene.singleton.EnterSceneFinally();
    }

    public void OnGUI() { }


    public void Update() { }



    void CreatePlayer()
    {
        SceneList sc = new SceneList();
        SceneList.RowData row = sc.GetItemID(sceneid);
        XEntityMgr.singleton.CreatePlayer(row);
        player = XEntityMgr.singleton.player;
        player.EnableCC(true);
        Debug.Log("player name: " + player.EntityObject.name);
    }
    
    
}
