using System.Collections;
using System.Collections.Generic;
using UnityEngine;

internal class XKeyboard : XSingleton<XKeyboard>
{

    private XTouchItem[] _touches = new XTouchItem[XTouch.MaxTouchCount];

    private int _touch_count = 0;
    private bool _bAxis = false;
    private float _x = 0;
    private float _y = 0;
    private Vector3 _lastMousePos = Vector3.zero;
    


    public bool Enabled { get { return !Application.isMobilePlatform; } }


    public int touchCount { get { return _touch_count; } }


    public XKeyboard()
    {
        for (int i = 0; i < XTouch.MaxTouchCount; i++)
            _touches[i] = new XTouchItem();
    }

    public XTouchItem GetTouch(int idx)
    {
        return _touches[idx];
    }

    public void Update()
    {
        _touch_count = 0;

        _bAxis = false;
        //mouse
        if (Input.GetMouseButton(0))
        {
            _touches[_touch_count].faketouch.fingerId = XTouch.mouseFingerID;
            _touches[_touch_count].faketouch.position = Input.mousePosition;
            _touches[_touch_count].faketouch.deltaTime = Time.smoothDeltaTime;
            _touches[_touch_count].faketouch.deltaPosition = Input.mousePosition - _lastMousePos;
            _touches[_touch_count].faketouch.phase = (Input.GetMouseButtonDown(0) ? TouchPhase.Began :
                                (_touches[_touch_count].faketouch.deltaPosition.sqrMagnitude > 1.0f ? TouchPhase.Moved : TouchPhase.Stationary));
            _touches[_touch_count].faketouch.tapCount = 1;
            _touches[_touch_count].Fake = true;
            _lastMousePos = Input.mousePosition;
            _touch_count++;
        }
        else if(Input.GetMouseButtonUp(0))
        {
            _touches[_touch_count].faketouch.fingerId = XTouch.mouseFingerID;
            _touches[_touch_count].faketouch.position = Input.mousePosition;
            _touches[_touch_count].faketouch.deltaTime = Time.smoothDeltaTime;
            _touches[_touch_count].faketouch.phase = TouchPhase.Ended;
            _touches[_touch_count].faketouch.tapCount = 1;
            _touches[_touch_count].Fake = true;
            _touch_count++;
        }


        //keyboard
        _bAxis = true;
        int x = 0;
        int y = 0;
        if (Input.GetKey(KeyCode.A)) x--;
        if (Input.GetKey(KeyCode.D)) x++;
        if (Input.GetKey(KeyCode.S)) y--;
        if (Input.GetKey(KeyCode.W)) y++;
        if (x != 0 || y != 0)
        {
            _touches[_touch_count].faketouch.fingerId = XTouch.keyboardFinderID;
            _touches[_touch_count].faketouch.deltaTime = Time.smoothDeltaTime;
            _touches[_touch_count].faketouch.phase = (_x == 0 && _y == 0) ? TouchPhase.Began : TouchPhase.Moved;
            _touches[_touch_count].faketouch.position = _touches[_touch_count].faketouch.phase == TouchPhase.Began ?
                new Vector2(XVirtualTab.singleton.MaxDistance, XVirtualTab.singleton.MaxDistance) :
                ((x != 0 && y != 0) ?
                new Vector2(XVirtualTab.singleton.MaxDistance + x * XVirtualTab.singleton.MaxDistance * 0.707f, XVirtualTab.singleton.MaxDistance + y * XVirtualTab.singleton.MaxDistance * 0.707f) :
                new Vector2(XVirtualTab.singleton.MaxDistance + x * XVirtualTab.singleton.MaxDistance, XVirtualTab.singleton.MaxDistance + y * XVirtualTab.singleton.MaxDistance));
            _touches[_touch_count].faketouch.tapCount = 1;
            _touches[_touch_count].Fake = true;
            _touch_count++;
        }
        else if (_x != 0 || _y != 0)
        {
            _touches[_touch_count].faketouch.fingerId = XTouch.keyboardFinderID;
            _touches[_touch_count].faketouch.position = new Vector2(XVirtualTab.singleton.MaxDistance, XVirtualTab.singleton.MaxDistance);
            _touches[_touch_count].faketouch.deltaTime = Time.smoothDeltaTime;
            _touches[_touch_count].faketouch.phase = TouchPhase.Ended;
            _touches[_touch_count].faketouch.tapCount = 1;
            _touches[_touch_count].Fake = true;
            _touch_count++;
        }
        _x = x; _y = y;
        if (!_bAxis)
        {
            _x = 0; _y = 0;
        }
    }
   

}

