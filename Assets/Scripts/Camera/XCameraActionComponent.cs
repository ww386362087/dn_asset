using UnityEngine;


class XCameraActionComponent : XComponent
{
    private XCamera _camera_host = null;

    private Vector3 _last_pos = Vector3.zero;
    private bool _began = false;

    private float _auto_x = 0;
    private float _auto_y = 0;

    private float _manual_x = 0;
    private float _manual_y = 0;

    private float _tx = 0;
    private float _ty = 0;

    private bool _auto = true;
    private const float speed = 0.08f;


    protected override UpdateState state
    {
        get { return UpdateState.FRAME; }
    }

    public override void OnInitial(XObject _obj)
    {
        base.OnInitial(_obj);
        _camera_host = _obj as XCamera;
    }


    public override void OnUninit()
    {
        base.OnUninit();
        _camera_host = null;
    }


    public override void OnUpdate(float delta)
    {
        base.OnUpdate(delta);
        if (XGesture.singleton.Working)
        {
            if (_began)
            {
                _tx = 0; _auto_x = 0;
                _ty = 0; _auto_y = 0;
            }
            else
            {
                _tx = (XGesture.singleton.GesturePosition.x - _last_pos.x) * speed;
                _ty = (XGesture.singleton.GesturePosition.y - _last_pos.y) * speed;
            }
            _last_pos = XGesture.singleton.GesturePosition;
            _began = false;
            _auto = true;
        }
        else
        {
            _began = true;
            if (_auto)
            {
                _tx = 0;
                _ty = 0;
            }
        }

        if (_auto)
        {
            _auto_x += _tx - _auto_x;
            _auto_y += _ty - _auto_y;
          //  if (_auto_y != 0) _camera_host.XRotate(-_auto_y);
            if (_auto_x != 0) _camera_host.YRotate(_auto_x);
        }
        else
        {
            _manual_x += _tx - _manual_x;
            _manual_y += _ty - _manual_y;
            _camera_host.XRotateEx(_manual_x);
          //  _camera_host.YRotateEx(_manual_y);
        }
    }

}
