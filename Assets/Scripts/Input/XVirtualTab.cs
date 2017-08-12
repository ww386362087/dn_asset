using UnityEngine;

/// <summary>
/// 虚拟摇杆
/// </summary>
internal class XVirtualTab : XSingleton<XVirtualTab>
{
    private int _finger_id = -1;
    private bool _bTouch = false;
    private bool _bFeeding = false;
    private bool _bFreeze = false;
    private Vector3 _direction = Vector3.zero;

    private Vector2 _center = Vector2.zero;
    private Vector2 _tab_dir = Vector2.up;
    private readonly float _dead_zone = 15;
    private float _max_distance = 75;

    public bool Feeding { get { return _bFeeding && !_bFreeze; } }

    public int FingerId { get { return _finger_id; } }

    public bool Freezed
    {
        get { return _bFreeze; }
        set
        {
            _bFreeze = value;
            Cancel();
        }
    }


    public Vector3 Direction { get { return _direction; } }

    public float MaxDistance { get { return _max_distance; } }

    public void Feed(XTouchItem touch)
    {
        if (!_bFreeze && ((_finger_id == -1 && touch.FingerId != XGesture.singleton.FingerId) || _finger_id == touch.FingerId))
        {
            if (_bFeeding)
            {
                if (XTouch.IsActiveTouch(touch))
                    CalcMove(touch, false);
                else
                    Cancel();
            }
            else
            {
                if (_bTouch)
                {
                    if (XTouch.IsActiveTouch(touch))
                    {
                        Vector2 delta = touch.Position - _center;

                        if (delta.sqrMagnitude > _dead_zone * _dead_zone)
                        {
                            _bFeeding = true;
                            CalcMove(touch, true);
                        }
                    }
                    else
                        Cancel();
                }
                else
                {
                    if (touch.Phase == TouchPhase.Began && touch.Position.x < Screen.width * 0.5f)
                    {
                        _bTouch = true;
                        _center = touch.Position;
                        _finger_id = touch.FingerId;
                    }
                }
            }
        }
    }


    public void Cancel()
    {
        if (_bTouch)
        {
            _bTouch = false;
            _bFeeding = false;
            _center = Vector2.zero;
            _finger_id = -1;

            XJoyStickStopEvent e = new XJoyStickStopEvent();
            XEventMgr.singleton.FireEvent(e);

            JoyStickDlg.singleton.Hide();
        }
    }


    private void CalcMove(XTouchItem touch, bool newly)
    {
        TabCulling();

        Vector2 dir = touch.Position - _center;

        if (!newly && Application.isEditor && touch.FingerId == 1)
        {
            float tab_round_angle = 480 * Time.deltaTime;

            float angles = Vector2.Angle(_tab_dir, dir);

            if (angles > tab_round_angle)
            {
                bool hands_before = XCommon.singleton.Clockwise(_tab_dir, dir);
                _tab_dir = XCommon.singleton.HorizontalRotateVetor2(_tab_dir, hands_before ? tab_round_angle : -tab_round_angle, false);
                bool hands_end = XCommon.singleton.Clockwise(_tab_dir, dir);

                if (hands_before != hands_end) _tab_dir = dir;
            }
            else
                _tab_dir = dir;
        }
        else
            _tab_dir = dir;

        float radius = _tab_dir.magnitude;

        if (radius > _max_distance)
        {
            radius = _max_distance;
            TabCulling();
        }

        float angle = Vector2.Angle(Vector2.up, _tab_dir);
        bool bClockwise = XCommon.singleton.Clockwise(Vector2.up, _tab_dir);

        if (XScene.singleton.GameCamera == null || XScene.singleton.GameCamera.CameraTrans == null) return;
        Vector3 forward = XScene.singleton.GameCamera.CameraTrans.forward;
        forward.y = 0;
        forward.Normalize();
        _direction = XCommon.singleton.HorizontalRotateVetor3(forward, bClockwise ? angle : -angle);
       
        JoyStickDlg.singleton.Show(true, _center);
        JoyStickDlg.singleton.SetOffsetPos(radius, (bClockwise ? angle : 360.0f - angle) - 90);
        
    }

    private void TabCulling()
    {
        if (_center.x - _max_distance < 0) _center.x = _max_distance;
        if (_center.y - _max_distance < 0) _center.y = _max_distance;
        if (_center.x + _max_distance > Screen.width) _center.x = Screen.width - _max_distance;
        if (_center.y + _max_distance > Screen.height) _center.y = Screen.height - _max_distance;
    }
}
