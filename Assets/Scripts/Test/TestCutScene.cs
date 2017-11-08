#if TEST

using UnityEngine;
using System;
using System.IO;
using System.Xml.Serialization;

public class TestCutScene : ITest
{
    private XCutSceneData _run_data = null;
    const int sceneid = 408;

    public void Start()
    {
        XScene.singleton.Enter(sceneid);
    }

    public void OnGUI()
    {
        if(GUI.Button(new Rect(20,20,180,60),"Play"))
        {
            Load();
            XScene.singleton.AttachCutScene(_run_data);
        }
        if(GUI.Button(new Rect(20,120,180,60),"Back"))
        {
            XScene.singleton.DetachCutScene();
        }
    }
    
    public void Update() {}

    public void LateUpdate(){}
    
    void Load()
    {
        string path = @"Assets/Resources/Table/CutScene/4_4_start.txt";
       _run_data = DeserializeData<XCutSceneData>(path);
    }

    public T DeserializeData<T>(string pathwithname)
    {
        try
        {
            using (FileStream reader = new FileStream(pathwithname, FileMode.Open))
            {
                XmlSerializer formatter = new XmlSerializer(typeof(T));
                return (T)formatter.Deserialize(reader);
            }
        }
        catch (Exception)
        {
            return default(T);
        }
    }
}


#endif