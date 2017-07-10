using UnityEngine;
using System.Collections;
using XTable;
using System.Collections.Generic;

public class Test : XSingleton<Test>
{
    private CombineConfig combineConfig = null;
    private FashionSuit fashionSuit = new FashionSuit();
    private EquipSuit equipSuit = new EquipSuit();

    private List<EquipPart> m_FashionList = null;
    private List<EquipPart> m_EquipList = null;
    private Vector2 fashionScrollPos = Vector2.zero;
    private Vector2 equipScrollPos = Vector2.zero;

    XRole role;


    public void Initial()
    {
        Application.targetFrameRate = 60;
        role = XEntityMgr.singleton.CreateTestRole();
        role.EntityObject.AddComponent<XRotation>();

        //时装
        TempEquipSuit fashions = new TempEquipSuit();
        m_FashionList = new List<EquipPart>();
        for (int i = 0; i < fashionSuit.Table.Length; ++i)
        {
            FashionSuit.RowData row = fashionSuit.Table[i];
            if (row.FashionID != null)
            {
                XEquipUtil.MakeEquip(row.SuitName, row.FashionID, m_FashionList, fashions, (int)row.SuitID);
            }
        }

        //装备
        m_EquipList = new List<EquipPart>();
        for (int i = 0; i < equipSuit.Table.Length; ++i)
        {
            EquipSuit.RowData row = equipSuit.Table[i];
            if (row.EquipID != null)
                XEquipUtil.MakeEquip(row.SuitName, row.EquipID, m_EquipList, fashions, -1);
        }

        //动作
        uint archerid = 2;
        XEntityPresentation p = new XEntityPresentation();
        XEntityPresentation.RowData xrow = p.GetItemID(archerid);
        role.GetComponent<XAnimComponent>().OverrideAnims(xrow);

        role.GetComponent<XEquipComponent>().EquipTest(m_FashionList[0]);
    }

    int space = 30;
    string[] anims = { "ToSkill", "EndSkill", "ToMove" };
    public void GUI()
    {
        GUILayout.BeginHorizontal();

        GUILayout.Space(space);
        GUILayout.BeginVertical();
        GUILayout.Label("时装");
        fashionScrollPos = GUILayout.BeginScrollView(fashionScrollPos, false, false);

        for (int i = 0; i < m_FashionList.Count; ++i)
        {
            EquipPart part = m_FashionList[i];
            for (int j = 0; j < part.suitName.Count; ++j)
            {
                GUILayout.BeginHorizontal();
                GUILayout.Label(part.suitName[j], GUILayout.MaxWidth(150));
                if (j == 0)
                {
                    if (GUILayout.Button("Preview", GUILayout.MaxWidth(100))) Preview(part);
                }
                GUILayout.EndHorizontal();
            }
            GUILayout.Space(5);
        }
        GUILayout.EndScrollView();
        GUILayout.EndVertical();

        GUILayout.Space(space);
        GUILayout.BeginVertical();
        GUILayout.Label("装备");
        equipScrollPos = GUILayout.BeginScrollView(equipScrollPos, false, false);
        List<EquipPart> currentEquipPrefession = m_EquipList;
        for (int i = 0; i < currentEquipPrefession.Count; ++i)
        {
            EquipPart part = currentEquipPrefession[i];
            for (int j = 0; j < part.suitName.Count; ++j)
            {
                GUILayout.BeginHorizontal();
                GUILayout.Label(part.suitName[j], GUILayout.MaxWidth(200));
                if (j == 0)
                {
                    if (GUILayout.Button("Preview", GUILayout.MaxWidth(100))) Preview(part);
                }
                GUILayout.EndHorizontal();
            }
            GUILayout.Space(5);
        }
        GUILayout.EndScrollView();
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


    private void Preview(EquipPart part)
    {
        role.GetComponent<XEquipComponent>().EquipTest(part);
    }

}
