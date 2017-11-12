#if TEST

using UnityEngine;
using XTable;
using System.Collections.Generic;

public class TestFashion : ITest
{
    private List<EquipPart> m_FashionList = null;
    private List<EquipPart> m_EquipList = null;
    private Vector2 fashionScrollPos = Vector2.zero;
    private Vector2 equipScrollPos = Vector2.zero;

    XRole role;


    public void Start()
    {
        TestAvatar();
        TestUIAB();
    }

    private void TestAvatar()
    {
        role = XEntityMgr.singleton.CreateTestRole();
        role.EntityObject.AddComponent<XRotation>();

        //时装
        TempEquipSuit fashions = new TempEquipSuit();
        m_FashionList = new List<EquipPart>();
        var fashionsuit = XTableMgr.GetTable<FashionSuit>();
        for (int i = 0, max = fashionsuit.length; i < max; ++i)
        {
            FashionSuit.RowData row = fashionsuit[i];
            if (row.FashionID != null)
            {
                XEquipUtil.MakeEquip(row.SuitName, row.FashionID, m_FashionList, fashions, (int)row.SuitID);
            }
        }

        //装备
        m_EquipList = new List<EquipPart>();
        var equipsuit = XTableMgr.GetTable<EquipSuit>();
        for (int i = 0, max = equipsuit.length; i < max; ++i)
        {
            EquipSuit.RowData row = equipsuit[i];
            if (row.EquipID != null)
                XEquipUtil.MakeEquip(row.SuitName, row.EquipID, m_EquipList, fashions, -1);
        }
    }

    private void TestUIAB()
    {
        CanvasDlg.singleton.SetVisible(true);
        UIManager.singleton.UiCamera.depth = -2;
    }

    int space = 30;
    string[] anims = {
        "ToArtSkill",
        "EndSkill",
        XSkillData.Combined_Command[0],
        XSkillData.Combined_Command[1],
        XSkillData.Combined_Command[2],
        "ToMove",
        "ToStand"
    };

    string[] weapons = {
        "Player_archer_weapon_archer",
        "ar_costume_baseball_a_bigbow_weapon",
        "ar_blackdragon_bigbow_weapon",
        "ar_light_bigbow_weapon",
        "ar_duya_d02_bigbow_weapon",
        "ar_tamasama_d_bigbow_weapon",
        "ar_ziyo_d03_bigbow_weapon",
        "ar_pajamas_a03_bigbow_weapon"
    };

    Color[] colors = new Color[]
    {
        Color.white,
        Color.green,
        Color.grey,
        Color.yellow,
        Color.blue,
        Color.gray,
        Color.red
    };

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
            }
        }
        GUILayout.Space(10);
        GUILayout.Label("武器");
        for (int i = 0, max = weapons.Length; i < max; i++)
        {
            if (GUILayout.Button(weapons[i])) role.GetComponent<XEquipComponent>().AttachWeapon(weapons[i]);
        }
        GUILayout.Space(10);
        GUILayout.Label("发型");
        for (int i=0,max=colors.Length;i<max;i++)
        {
            if (GUILayout.Button(colors[i].ToString())) role.GetComponent<XEquipComponent>().ChangeHairColor(colors[i]);
        }

        GUILayout.EndVertical();

        GUILayout.EndHorizontal();
    }


    private void Preview(EquipPart part)
    {
        role.GetComponent<XEquipComponent>().EquipPart(part);
    }


    private void TestMesh(Transform t)
    {

        Mesh mesh = t.GetComponent<MeshFilter>().mesh;
        XDebug.Log("mesh v: " + mesh.vertices.Length, " tri: ", mesh.triangles.Length, " uv: " + mesh.uv.Length);
        for (int i = 0; i < mesh.vertices.Length; i++) XDebug.Log("v", i, ":", mesh.vertices[i]);
        for (int i = 0; i < mesh.uv.Length; i++) XDebug.Log("uv", i, ":", mesh.uv[i]);
        for (int i = 0; i < mesh.triangles.Length; i++) XDebug.Log("tri", i, ":", mesh.triangles[i]);
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

    public void LateUpdate()
    {
    }

}


#endif