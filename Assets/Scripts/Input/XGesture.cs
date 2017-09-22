using UnityEngine;

internal class XGesture : XSingleton<XGesture>
{

    private readonly float _dead_zone = 10;
    private bool _bTouch = false;
    private int _finger_id = -1;

    private bool _one = false;
    private bool _one_up = false;
    private bool _bswype = false;
    private float _last_swype_at = 0;
    private bool _bFreeze = false;
    
    private float _swype_dis = 0;

    private Vector2 _start = Vector2.zero;
    private Vector2 _swype_start = Vector2.zero;
    private Vector2 _end = Vector2.zero;
    private Vector3 _swypedir = Vector3.zero;
    private Vector3 _gesturepos = Vector3.zero;
    private Vector3 _touchpos = Vector3.zero;

    private float _last_touch_down_at = 0;
    
    public bool Freezed
    {
        get { return _bFreeze; }
        set
        {
            _bFreeze = value;
            Cancel();
        }
    }

    public bool Working
    {
        get { return _finger_id != -1; }
    }

    public float LastSwypeAt
    {
        get { return _last_swype_at; }
    }

    public Vector3 GesturePosition
    {
        get { return _gesturepos; }
    }

    public Vector3 TouchPosition
    {
        get { return _touchpos; }
    }

    public int FingerId { get { return _finger_id; } }

    public void Feed(XTouchItem touch)
    {
        _one |= OneUpdate(touch);
        _one_up |= OneUpUpdate(touch);

        if ((!_bFreeze) && ((_finger_id == -1 && touch.FingerId != XVirtualTab.singleton.FingerId) || _finger_id == touch.FingerId))
        {
            if (touch.Phase == TouchPhase.Began)
            {
                _start = touch.Position;
                _swype_start = _start;
                _bTouch = true;
            }

            if (_bTouch)
            {
                if (XTouch.IsActiveTouch(touch))
                {
                    _gesturepos = touch.Position;
                    _bswype = SwypeUpdate(touch);
                    if (_bswype) _finger_id = touch.FingerId;
                }
                else
                    Cancel();
            }
        }
    }


    public void Cancel()
    {
        XEventMgr.singleton.FireEvent(new XGestureCancelEvent());

        _bTouch = false;
        _one = false;
        _bswype = false;
        _finger_id = -1;
    }

    
    private bool OneUpdate(XTouchItem touch)
    {
        if (touch.Phase == TouchPhase.Began &&
                   (touch.FingerId == XTouch.mouseFingerID ||
                    touch.FingerId == XTouch.keyboardFinderID))
        {
            _touchpos = touch.Position;
            return true;
        }
        return false;
    }


    private bool OneUpUpdate(XTouchItem touch)
    {
        if (touch.FingerId != XTouch.mouseFingerID && touch.FingerId != XTouch.keyboardFinderID) return false;

        if (touch.Phase == TouchPhase.Began)
        {
            if (!Application.isMobilePlatform && touch.FingerId == XTouch.keyboardFinderID) return false;

            //only process mouse point but keyboard for Editor Mode
            _touchpos = touch.Position;
            _last_touch_down_at = Time.time;
        }
        else if (touch.Phase == TouchPhase.Canceled || touch.Phase == TouchPhase.Ended)
        {
            float deltaT = Time.time - _last_touch_down_at;
            if (deltaT < 0.5f / Time.timeScale)
            {
                Vector2 delta;
                delta.x = _touchpos.x - touch.Position.x;
                delta.y = _touchpos.y - touch.Position.y;
                
                if (delta.magnitude < _dead_zone)
                {
                    _touchpos = touch.Position;
                    return true;
                }
            }
        }
        return false;
    }


    private bool SwypeUpdate(XTouchItem touch)
    {
        if (touch.Phase == TouchPhase.Moved)
        {
            _end = touch.Position;
            Vector2 delta = _end - _swype_start;

            float endAt = Time.time;
            _swype_dis = delta.magnitude;

            if (_swype_dis > _dead_zone)
            {
                _swype_start = _end;
                _swypedir.x = delta.x;
                _swypedir.y = 0;
                _swypedir.z = delta.y;
                _swypedir.Normalize();
                _gesturepos = _end;
                _last_swype_at = endAt;
                return true;
            }
        }

        return false;
    }

}

