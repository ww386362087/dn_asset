using XTable;
using UnityEngine;
using System.Collections.Generic;

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

        TempEquipSuit fashions = new TempEquipSuit();
        FashionSuit fashionSuit = new FashionSuit();
        List<EquipPart> m_FashionList = new List<EquipPart>();
        for (int i = 0; i < fashionSuit.Table.Length; ++i)
        {
            FashionSuit.RowData row2 = fashionSuit.Table[i];
            if (row2.FashionID != null)
            {
                XEquipUtil.MakeEquip(row2.SuitName, row2.FashionID, m_FashionList, fashions, (int)row2.SuitID);
            }
        }
        player.GetComponent<XEquipComponent>().EquipTest(m_FashionList[0]);

        uint archerid = 2;
        XEntityPresentation p = new XEntityPresentation();
        XEntityPresentation.RowData xrow = p.GetItemID(archerid);
        player.GetComponent<XAnimComponent>().OverrideAnims(xrow);
    }

    
}
