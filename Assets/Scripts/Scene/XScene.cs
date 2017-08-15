using UnityEngine;
using XTable;

internal class XScene : XSingleton<XScene>
{
    private XCamera _camera;
    private uint _sceneid;

    private Terrain _terrain;

    public uint SceneID { get { return _sceneid; } }

    public XCamera GameCamera
    {
        get
        {
            if (_camera == null) _camera = new XCamera();
            return _camera;
        }
    }


    public void Update(float deltaTime)
    {
    }

    public void LateUpdate()
    {
        if (_camera != null)
            _camera.LateUpdate();
    }


    public void Enter(uint sceneid)
    {
        _sceneid = sceneid;
        OnEnterScene(sceneid);
        //to-do 
        CreatePlayer();
        CreateMonsters();

        OnEnterSceneFinally();
    }

    private void OnEnterScene(uint sceneid)
    {
        _sceneid = sceneid;
        GameObject camera = GameObject.FindGameObjectWithTag("MainCamera");
        GameCamera.PreInitial(camera);
        Documents.singleton.OnEnterScene();
    }

    private void OnEnterSceneFinally()
    {
        GameCamera.Initial();
        Documents.singleton.OnEnterSceneFinally();
        _terrain = Terrain.activeTerrain;
    }


    public void LeaveScene()
    {
        _camera.Uninitial();
        _camera = null;
        Documents.singleton.OnLeaveScene();
    }


    public float TerrainY(Vector3 pos)
    {
        if(_terrain!=null)
        {
            return _terrain.SampleHeight(pos);
        }
        return 0;
    }

    private void CreatePlayer()
    {
        SceneList sc = new SceneList();
        SceneList.RowData row = sc.GetItemID(_sceneid);
        XEntityMgr.singleton.CreatePlayer(row);
        XPlayer player = XEntityMgr.singleton.player;
        player.EnableCC(true);
        Debug.Log("player name: " + player.EntityObject.name);
    }


    private void CreateMonsters()
    {
        //to-do create monster
    }



}
