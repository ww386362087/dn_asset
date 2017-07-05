using UnityEngine;
using System.Collections;
using XTable;
using System.IO;

public class Main : MonoBehaviour
{

    void Start()
    {
        XEquipUtil.Test();
        GameObject go = GameObject.Find("Player(Clone)");
        go.AddComponent<XRotation>();
    }


    void OnGUI()
    {
        if (GUI.Button(new Rect(20, 20, 100, 40), "ReadByte"))
        {
            ReadBytes();
        }
    }


    void ReadBytes()
    {
        People p = new People(true);
        for (int i = 0, max = p.Table.Length; i < max; i++)
        {
            Debug.Log("id: " + p.Table[i].id + " name: " + p.Table[i].name + " com:" + p.Table[i].com);
        }

        FashionList f = new FashionList(true);
        for (int i = 0; i < 10; i++)
        {
            Debug.Log("id:" + f.Table[i].ItemID + " name:" + f.Table[i].ItemName + " comment:" + f.Table[i].EquipPos);
        }
    }

}
