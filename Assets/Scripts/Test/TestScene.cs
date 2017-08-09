using XTable;
using UnityEngine;
using System;

public class TestScene : ITest
{

    XPlayer player;
    Camera came;
    const int sceneid = 401;

    public void Start()
    {
        CreatePlayer();
    }

    public void OnGUI() { }


    public void Update() { }



    void CreatePlayer()
    {
        SceneList sc = new SceneList();
        SceneList.RowData row = sc.GetItemID(sceneid);
        XEntityMgr.singleton.CreatePlayer(row);
        player = XEntityMgr.singleton.player;
    }

    
}
