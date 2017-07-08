using UnityEngine;
using System.Collections;
using XTable;

public class Test : XSingleton<Test>
{
    //private CombineConfig combineConfig = null;
    //private DefaultEquip defaultEquip = new DefaultEquip();
    //private FashionList fashionList = new FashionList();
    //private FashionSuit fashionSuit = new FashionSuit();
    //private EquipSuit equipSuit = new EquipSuit();
    XRole role;


    public void Initial()
    {
        Application.targetFrameRate = 60;
        role = XEntityMgr.singleton.CreateTestRole();
        role.EntityObject.AddComponent<XRotation>();

        //时装


        //装备

        //动作
        uint archerid = 2;
        XEntityPresentation p = new XEntityPresentation();
        XEntityPresentation.RowData row = p.GetItemID(archerid);
        role.GetComponent<XAnimComponent>().OverrideAnims(row);
    }

    int space = 30;
    string[] anims = { "ToSkill", "EndSkill", "ToMove" };
    private Vector2 fashionScrollPos = Vector2.zero;
    //private Vector2 equipScrollPos = Vector2.zero;
    public void GUI()
    {
        GUILayout.BeginHorizontal();

        GUILayout.Space(space);
        GUILayout.BeginVertical();
        GUILayout.Label("时装");
        fashionScrollPos = GUILayout.BeginScrollView(fashionScrollPos, false, false);

        //for (int i = 0; i < m_FashionList.Count; ++i)
        //{
        //    EquipPart part = m_FashionList[i];
        //    for (int j = 0; j < part.suitName.Count; ++j)
        //    {
        //        GUILayout.BeginHorizontal();
        //        GUILayout.Label(part.suitName[j], GUILayout.MaxWidth(150));
        //        if (j == 0)
        //        {
        //            if (GUILayout.Button("Preview", GUILayout.MaxWidth(100))) Preview(part);
        //        }
        //        GUILayout.EndHorizontal();
        //    }
        //    GUILayout.Space(5);
        //}
        GUILayout.EndScrollView();
        GUILayout.EndVertical();

        GUILayout.Space(space);
        GUILayout.BeginVertical();
        GUILayout.Label("装备");
        GUILayout.EndVertical();

        GUILayout.Space(space);
        GUILayout.BeginVertical();
        GUILayout.Label("动作");
        for (int i = 0, max = anims.Length; i < max; i++)
        {
            if (GUILayout.Button(anims[i])) role.GetComponent<XAnimComponent>().SetTrigger(anims[i]);
        }
        GUILayout.EndVertical();

        GUILayout.EndHorizontal();
    }


}
