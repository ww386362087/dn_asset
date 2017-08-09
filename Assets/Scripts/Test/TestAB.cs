using UnityEngine;
using XTable;
using System.Collections.Generic;
using System;

public class TestAB : ITest
{
    private FashionSuit fashionSuit = new FashionSuit();
    private EquipSuit equipSuit = new EquipSuit();

    private List<EquipPart> m_FashionList = null;
    private List<EquipPart> m_EquipList = null;
    private Vector2 fashionScrollPos = Vector2.zero;
    private Vector2 equipScrollPos = Vector2.zero;

    XRole role;


    public void Start()
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

        TestUIAB();
    }


    public void TestUIAB()
    {
        GameObject o = XResourceMgr.Load<GameObject>("UI/Canvas2", AssetType.Prefab);
        Debug.Log("name: " + o.name);
        GameObject go = MonoBehaviour.Instantiate(o) as GameObject;
        go.transform.localPosition = Vector3.zero;
        go.transform.localScale = Vector3.one;
        go.transform.localRotation = Quaternion.identity;
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
    public void OnGUI()
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
                if (i == 0)
                {
                    XIdleEventArgs arg = new XIdleEventArgs();
                    XEventMgr.singleton.FireEvent(arg);
                }
                else if (i == 1)
                {
                    XMoveEventArgs arg = new XMoveEventArgs();
                    arg.Speed = 12;
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


    private void TestMesh(Transform t)
    {

        Mesh mesh = t.GetComponent<MeshFilter>().mesh;
        Debug.Log("mesh v: " + mesh.vertices.Length + " tri: " + mesh.triangles.Length + " uv: " + mesh.uv.Length);
        for (int i = 0; i < mesh.vertices.Length; i++) Debug.Log("v" + i + ":" + mesh.vertices[i]);
        for (int i = 0; i < mesh.uv.Length; i++) Debug.Log("uv" + i + ":" + mesh.uv[i]);
        for (int i = 0; i < mesh.triangles.Length; i++) Debug.Log("tri" + i + ":" + mesh.triangles[i]);
        mesh = new Mesh();
        mesh.Clear();

        Vector3[] vertices = { new Vector3(-0.5f, -0.5f, 0), new Vector3(-0.5f, 0.5f, 0), new Vector3(0.5f, 0.5f, 0), new Vector3(0.5f, -0.5f, 0) };
        Vector2[] uv = { new Vector2(0, 0), new Vector2(0, 1f), new Vector2(1, 1f), new Vector2(1, 0) };
        int[] triangles = { 0, 2, 1, 0, 3, 2 };

        mesh.vertices = vertices;
        mesh.uv = uv;
        mesh.triangles = triangles;

        t.GetComponent<MeshFilter>().mesh = mesh;

    }

  

    public void Update() { }
}
