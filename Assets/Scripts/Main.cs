using UnityEngine;
using System.Collections;
using XTable;
using System.IO;


public class Main : MonoBehaviour
{
    XRole role;
    void Start()
    {
        //test
        Application.targetFrameRate = 60;
        role = XEntityMgr.singleton.CreateTestRole();
        Test();
    }

    void Update()
    {
        XResourceMgr.Update();
    }

    void Test()
    {
        role.EntityObject.AddComponent<XRotation>();
        uint archerid = 2;
        XEntityPresentation p = new XEntityPresentation(true);
        XEntityPresentation.RowData row = p.GetItemID(archerid);
        role.GetComponent<XAnimComponent>().OverrideAnims(row);
    }

    string[] anims = { "ToStand", "ToSkill", "EndSkill", "ToMove", "ToArtSkill" };
    void OnGUI()
    {
        GUILayout.Label("动作");
        for (int i = 0, max = anims.Length; i < max; i++)
        {
            if (GUILayout.Button(anims[i])) role.GetComponent<XAnimComponent>().SetTrigger(anims[i]);
        }
    }
}
