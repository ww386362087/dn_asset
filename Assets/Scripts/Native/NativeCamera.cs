using UnityEngine;

public class NativeCamera: XSingleton<NativeCamera>
{
    private Camera _camera;
    private NativePlayer _player;
    private Vector3 _offset;

    public Camera camera { get { return _camera; } }

    public void Initial()
    {
        if (_camera == null)
        {
            GameObject g = GameObject.FindGameObjectWithTag("MainCamera");
            if (g == null) return;
            _camera = g.GetComponent<Camera>();
            _camera.transform.rotation = Quaternion.Euler(new Vector3(13, 0, 0));
        }
        if (_player == null)
        {
            _player = NativeEntityMgr.singleton.Player;
        }
        _offset = new Vector3(0.1f, 3f, -6f);
    }

    public void Update(float delta)
    {
        if (_player == null)
        {
            _player = NativeEntityMgr.singleton.Player;
        }
        if (_camera != null && _player != null)
        {
            _camera.transform.position = _player.transfrom.position + _offset;
        }
    }

}
