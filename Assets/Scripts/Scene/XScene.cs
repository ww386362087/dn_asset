using System.Collections;
using System.Collections.Generic;
using UnityEngine;

internal class XScene : XSingleton<XScene>
{
    private XCamera _camera = new XCamera();
    private uint _sceneid;


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
        Documents.singleton.OnEnterScene();
    }

    public void EnterSceneFinally()
    {
        Documents.singleton.OnEnterSceneFinally();
    }


    public void LeaveScene()
    {
        _camera.Uninitial();
    }


}
