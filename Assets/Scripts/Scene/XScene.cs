using UnityEngine;

internal class XScene : XSingleton<XScene>
{
    private XCamera _camera = new XCamera();
    private uint _sceneid;

    private Terrain _terrain;

    public uint SceneID { get { return _sceneid; } }

    public XCamera GameCamera
    {
        get { return _camera; }
    }


    public void Update(float deltaTime)
    {
    }

    public void PostUpdate()
    {
    }


    public void EnterScene(uint sceneid)
    {
        _sceneid = sceneid;
        Documents.singleton.OnEnterScene();
    }

    public void EnterSceneFinally()
    {
        GameObject camera = GameObject.FindGameObjectWithTag("MainCamera");
        GameCamera.Initial(camera);
        Documents.singleton.OnEnterSceneFinally();
        _terrain = Terrain.activeTerrain;
    }


    public void LeaveScene()
    {
        _camera.Uninitial();
    }


    public float TerrainY(Vector3 pos)
    {
        if(_terrain!=null)
        {
            return _terrain.SampleHeight(pos);
        }
        return 0;
    }

}
