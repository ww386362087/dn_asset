using UnityEngine;

/// <summary>
/// screen 右侧手势->camera旋转
/// </summary>

class XCameraActionComponent : XComponent
{
    private XCamera _camera = null;

    private Vector3 _last_pos = Vector3.zero;
    private bool _began = false;

    private float _auto_x = 0;
    private float _auto_y = 0;

    private float _manual = 0;

    private float _tx = 0;
    private float _ty = 0;

    private bool _auto = true;
    private const float speed = 0.06f;

    private float _flowSpeed = 0.8f;//the value must be less 1f

    protected override UpdateState state
    {
        get { return UpdateState.FRAME; }
    }

    public override void OnInitial(XObject _obj)
    {
        base.OnInitial(_obj);
        _camera = _obj as XCamera;
    }


    public override void OnUninit()
    {
        base.OnUninit();
        _camera = null;
    }

    protected override void EventSubscribe()
    {
        base.EventSubscribe();
        RegisterEvent(XEventDefine.XEvent_Gesture_Cancel, OnGestureCancel);
    }

    public override void OnUpdate(float delta)
    {
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
            _auto_x = (_tx - _auto_x) * _flowSpeed;
            _auto_y += (_ty - _auto_y) * _flowSpeed;
            if (_auto_y != 0) _camera.XRotate(-_auto_y);
            if (_auto_x != 0) _camera.YRotate(_auto_x);
        }
        else
        {
            _manual = _manual * _flowSpeed;
            if (_manual > 0.002f)
            {
                _camera.YRotate(_tx);
            }
        }
    }


    private void OnGestureCancel(XEventArgs e)
    {
        _manual = _auto_x;
        _auto = false;
    }

}
