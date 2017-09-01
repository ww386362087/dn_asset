using UnityEngine;
using System.Collections.Generic;


public class XFxMgr : XSingleton<XFxMgr>
{
    public int CameraLayerMask = ~0;
    public static int _UILayerOffset = 0;

    private Dictionary<int, XFx> _fxs = new Dictionary<int, XFx>();

    public void PostSetting()
    {
        Camera camera = XScene.singleton.GameCamera.UnityCamera;
        CameraLayerMask = camera.cullingMask | _UILayerOffset;
    }

    public XFx CreateFx(string prefab_location)
    {
        return CreateFx(prefab_location, false);
    }

    public XFx CreateFx(string prefab_location, bool async)
    {
        XFx fx = new XFx();
        fx.CreateXFx(prefab_location, async);
        _fxs.Add(fx.instanceID, fx);
        return fx;
    }

    public XFx CreateAndPlay(string location, GameObject parent, Vector3 offset, Vector3 scale, float speed_ratio = 1, float duration = -1, bool async = true)
    {
        XFx fx = new XFx();
        fx.CreateXFx(location, async);
        fx.Play(parent, offset, scale, speed_ratio);
        fx.DelayDestroy = duration;
        DestroyFx(fx, false);
        return fx;
    }


    public XFx CreateUIFx(string location, Transform parent)
    {
        return CreateUIFx(location, parent, false);
    }

    public XFx CreateUIFx(string location, Transform parent, bool processMesh)
    {
        return CreateUIFx(location, parent, Vector3.one, processMesh);
    }

    public XFx CreateUIFx(string location, Transform parent, Vector3 scale, bool processMesh)
    {
        XFx fx = CreateFx(location);
        int uiLayer = LayerMask.NameToLayer("UI");
        fx.SetRenderLayer(uiLayer);
        fx.Play(parent.gameObject, Vector3.zero, scale, 1.0f);
        fx.RefreshUIRenderQueue();
        return fx;
    }


    private void DestroyFx(XFx fx, bool bImmediately)
    {
        fx.DestroyXFx();
        RemoveFx(fx);
    }

    public void RemoveFx(XFx fx)
    {
        if (_fxs.ContainsKey(fx.instanceID))
            _fxs.Remove(fx.instanceID);
    }

}

