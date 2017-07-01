using UnityEngine;
using System.Collections;
using XTable;
using System.IO;

public class Main : MonoBehaviour
{

    GameObject go;
    
    float speed = 1;

    void Start()
    {
        XEquipUtil.Test();
        go = GameObject.Find("Player(Clone)");
    }

    private string CombinePath(string path)
    {
        return Application.dataPath + "/Resources/" + path + ".bytes";
    }

    void OnGUI()
    {
        if (GUI.Button(new Rect(20, 20, 100, 40), "ReadByte"))
        {
            ReadBytes();
        }
    }


    void Update()
    {
        if (go != null)
        {
            go.transform.localRotation = Quaternion.Euler(new Vector3(0, 12 + speed, 0));
            speed++;
        }
        if (Input.GetKeyUp(KeyCode.F2))
        {
            ReadBytes();
        }
    }

    void ReadBytes()
    {
        People p = new People();
        FileStream fs = new FileStream(CombinePath(p.bytePath), FileMode.Open);
        p.ReadFile(fs);
        fs.Close();
        for (int i = 0, max = p.Table.Length; i < max; i++)
        {
            Debug.Log("id: " + p.Table[i].id + " name: " + p.Table[i].name + " com:" + p.Table[i].com);
        }

        XTable.FashionList f = new XTable.FashionList();
        fs = new FileStream(CombinePath(f.bytePath), FileMode.Open);
        f.ReadFile(fs);
        fs.Close();
        for (int i = 0; i < 10; i++)
        {
            Debug.Log("id:" + f.Table[i].ItemID + " name:" + f.Table[i].ItemName + " comment:" + f.Table[i].EquipPos);
        }
    }

}
