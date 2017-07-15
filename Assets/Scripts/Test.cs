using UnityEngine;
using XTable;
using System.Collections.Generic;

public class Test : XSingleton<Test>
{
    private FashionSuit fashionSuit = new FashionSuit();
    private EquipSuit equipSuit = new EquipSuit();

    private List<EquipPart> m_FashionList = null;
    private List<EquipPart> m_EquipList = null;
    private Vector2 fashionScrollPos = Vector2.zero;
    private Vector2 equipScrollPos = Vector2.zero;

    XRole role;


    public void Initial()
    {
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


    public void TestAB()
    {
        Object oo = ABManager.singleton.LoadImm("Animation/Player_archer/Player_archer_attack_pinpointshot", AssetType.Anim);
        AnimationClip clip = oo as AnimationClip;
        Debug.Log("clip: " + clip.length);

        //Object o = ABManager.singleton.LoadImm("Equipments/ar_costume_marine01_glove", AssetType.Prefab);
        //Debug.Log("o: " + (o == null) + " " + o.name);
        //XMeshTexData md= (o as GameObject).GetComponent<XMeshTexData>();
        //Debug.Log("md: " + md.offset);

        ABManager.singleton.LoadImm("Animation/Player_archer/Player_archer_2_4_cutscene_end", AssetType.Anim, (o) =>
        {
            AnimationClip cli = o as AnimationClip;
            Debug.Log("clip length: " + cli.length);
        });
    }

    int space = 30;
    string[] anims = { "ToSkill", "EndSkill", "ToMove" };
    string[] weapons = {
        "Player_archer_weapon_archer",
        "ar_costume_baseball_a_bigbow_weapon",
        "ar_blackdragon_bigbow_weapon",
        "ar_light_bigbow_weapon",
        "ar_duya_d02_bigbow_weapon",
        "ar_tamasama_d_bigbow_weapon",
        "ar_ziyo_d03_bigbow_weapon",
        "ar_pajamas_a03_bigbow_weapon"};
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
        for (int i = 0; i < m_EquipList.Count; ++i)
        {
            EquipPart part = m_EquipList[i];
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
            if (GUILayout.Button(anims[i]))
            {
                role.GetComponent<XAnimComponent>().SetTrigger(anims[i]);
                if(i==0)
                {
                    XIdleEventArgs arg = new XIdleEventArgs();
                    arg.Firer = role.GetComponent<XAIComponent>();
                    XEventMgr.singleton.FireEvent(arg);
                }
                else if(i==1)
                {
                    XMoveEventArgs arg = new XMoveEventArgs();
                    arg.Speed = 12;
                    arg.Firer = role.GetComponent<XAIComponent>();
                    XEventMgr.singleton.FireEvent(arg);
                }
            }
        }
        GUILayout.Space(10);
        GUILayout.Label("武器");
        for (int i = 0, max = weapons.Length; i < max; i++)
        {
            if (GUILayout.Button(weapons[i])) role.GetComponent<XEquipComponent>().AttachWeapon(weapons[i]);
        }
        GUILayout.Space(10);

        GUILayout.EndVertical();

        GUILayout.EndHorizontal();
    }


    private void Preview(EquipPart part)
    {
        role.GetComponent<XEquipComponent>().EquipTest(part);
    }

}
