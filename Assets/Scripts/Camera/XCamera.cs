using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class XCamera
{

    public enum XStatus
    {
        None,
        Idle,
        Solo,
        Effect
    }


    private Transform _cameraTransform = null;
    private XEntity _active_target = null;
    private float _field_of_view = 45;
    private Camera _camera;
    private GameObject _cameraObject = null;

    public Transform CameraTrans { get { return _cameraTransform; } }

    public Vector3 Position { get { return _cameraTransform.position; } }

    public Quaternion Rotaton { get { return _cameraTransform.rotation; } }


    public XEntity Target
    {
        get { return (_active_target == null || _active_target.Deprecated) ? null : _active_target; }
        set { _active_target = value; }
    }


    public void Initial(GameObject camera)
    {
        _cameraObject = camera;
        _cameraTransform = camera.transform;

        if (null != _cameraObject)
        {
            _camera = _cameraObject.GetComponent<Camera>();
            _camera.enabled = true;
            _field_of_view = _camera.fieldOfView;


            XPlayer player = XEntityMgr.singleton.player;

            if (player != null && player.EntityObject != null)
            {
                _cameraObject.transform.rotation = player.EntityObject.transform.rotation;
                _cameraObject.transform.position = player.EntityObject.transform.position + new Vector3(-1, 1, 1);
            }
        }
    }

    public void Uninitial()
    {
        _camera = null;
        
    }


}
