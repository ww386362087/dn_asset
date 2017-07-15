using UnityEngine;


public class Main : MonoBehaviour
{
    
    void Start()
    {
        ABManager.singleton.Init(this);

        //Object oo = ABManager.singleton.LoadImm("Animation/Player_archer/Player_archer_attack_pinpointshot", AssetType.Anim);
        //AnimationClip clip = oo as AnimationClip;
        //Debug.Log("clip: " + clip.length);

        Object o = ABManager.singleton.LoadImm("Equipments/ar_costume_marine01_glove", AssetType.Prefab);
        Debug.Log("o: " + (o == null) + " " + o.name);
        XMeshTexData md= (o as GameObject).GetComponent<XMeshTexData>();
        Debug.Log("md: " + md.offset);

        //GameObject go = Instantiate(o) as GameObject;
        //XMeshTexData data = go.transform.GetComponent<XMeshTexData>();
        //Debug.Log("data: "+data.offset);

        //Texture2D txture = data.tex;
        //Debug.Log("width:" + txture.width);


        //Object obj = ABManager.singleton.LoadImm("Equipments/ar_blackdragon_body", AssetType.Prefab);
        //Debug.Log("obj: " + (obj == null) + " " + obj.name);


        Test.singleton.Initial();
    }

    void Update()
    {
        XResourceMgr.Update();
    }

  
    void OnGUI()
    {
        Test.singleton.GUI();
    }

    
}
