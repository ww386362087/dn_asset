using UnityEngine;
using XTable;

internal class XScene : XSingleton<XScene>
{
    private XCamera _camera;
    private uint _sceneid;
    private SceneList.RowData _scene_row;
    private Terrain _terrain;

    public SceneList.RowData SceneRow { get { return _scene_row; } }


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
        if (_camera != null)
            _camera.Update(deltaTime);
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
        CreateNPCs();
        CreateMonsters();

        OnEnterSceneFinally();
    }

    private void OnEnterScene(uint sceneid)
    {
        _sceneid = sceneid;
        _scene_row = SceneList.sington.GetItemID(_sceneid);
        GameObject camera = GameObject.FindGameObjectWithTag("MainCamera");
        GameCamera.Initial(camera);
        Documents.singleton.OnEnterScene();
    }

    private void OnEnterSceneFinally()
    {
        GameCamera.OnEnterSceneFinally();
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
        XEntityMgr.singleton.CreatePlayer();
        XPlayer player = XEntityMgr.singleton.Player;
        player.EnableCC(true);
        Debug.Log("player name: " + player.EntityObject.name);
    }


    private void CreateMonsters()
    {
        //to-do create monster
    }


    private void CreateNPCs()
    {
        var row = XNpcList.sington.GetItemID(24);
        XEntityMgr.singleton.CreateNPC(row);
    }
}
