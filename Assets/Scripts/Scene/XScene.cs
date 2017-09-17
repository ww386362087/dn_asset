using UnityEngine;
using XTable;

public class XScene : XSingleton<XScene>
{
    private XCamera _camera;
    private uint _sceneid;
    private SceneList.RowData _scene_row;

    private XCutSceneData _cutscene_data = null;
    private XCutSceneRunner _cutscene_runer = null;
    private Terrain _terrain;

    public SceneType SceneType { get; set; }

    public uint SceneID { get { return _sceneid; } }

    /// <summary>
    /// 0 单机，客户端刷怪
    /// 1 联机，服务器刷怪和同步
    /// 2 单机，服务器把刷怪数据给客户端，客户端刷怪
    /// </summary>
    private int SyncModeValue = 0;
    
    public bool SyncMode { get { return SyncModeValue == 1; } }

    public SceneList.RowData SceneRow { get { return _scene_row; } }

    public bool IsPlayCutScene { get { return _cutscene_data != null; } }

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
        if(_sceneid>0)
        {
            if (_camera != null)
                _camera.Update(deltaTime);
        }
    }

    public void LateUpdate()
    {
        if(_sceneid>0)
        {
            if (_camera != null)
                _camera.LateUpdate();
        }
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

    public string GetSceneDynamicPrefix()
    {
        if (_scene_row != null && _scene_row.DynamicScene != null)
        {
            return "DynamicScene/" + _scene_row.DynamicScene + "/";
        }
        return "";
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
        if (_terrain != null)
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
    }

    public void AttachCutScene(XCutSceneData csd)
    {
        if (!IsPlayCutScene)
        {
            _cutscene_data = csd;
            GameCamera.Target = null;
            _cutscene_runer = GameCamera.CameraObject.AddComponent<XCutSceneRunner>();
            _cutscene_runer.cut_scene_data = _cutscene_data;
        }
        else
        {
            Debug.LogError("Is Playing Cutscene");
        }
    }


    public void DetachCutScene()
    {
        GameCamera.Target = XEntityMgr.singleton.Player;
        GameCamera.ResetIdle();
        if (_cutscene_runer != null)
        {
            _cutscene_runer.UnLoad();
            GameObject.Destroy(_cutscene_runer);
        }
        _cutscene_data = null;
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
