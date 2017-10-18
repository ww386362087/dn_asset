using UnityEngine;

internal class XTouch : XSingleton<XTouch>
{
    public const int mouseFingerID = 0;
    public const int keyboardFinderID = 1;

    private XTouchItem _touch = new XTouchItem();
    public const int MaxTouchCount = 2;

    public static bool PointOnUI(Vector3 point)
    {
        RaycastHit hitinfo;
        Ray uiRay = UIManager.singleton.UiCamera.ScreenPointToRay(point);
        if (Physics.Raycast(uiRay, out hitinfo, Mathf.Infinity, 1 << 5))
        {
            return true;// !hitinfo.collider.CompareTag("ChatUI");
        }
        else
        {
            return false;
        }
    }


    public static bool IsActiveTouch(XTouchItem touch)
    {
        return touch.Phase != TouchPhase.Ended && 
            touch.Phase != TouchPhase.Canceled;
    }


    public void Update(float deltaTime)
    {
        UpdateTouch();
    }



    private void UpdateTouch()
    {
        int max = Mathf.Min(Input.touchCount, MaxTouchCount);
        //real touch always updated first
        for (int i = 0; i < max; i++)
        {
            _touch.Fake = false;
            _touch.touch = Input.GetTouch(i);
            HandleTouch(_touch);
        }


        if (XKeyboard.singleton.Enabled)
        {
            XKeyboard.singleton.Update();
            max = XKeyboard.singleton.touchCount;
            for (int i = 0; i < max; i++)
            {
                HandleTouch(XKeyboard.singleton.GetTouch(i));
            }
        }
    }


    private void HandleTouch(XTouchItem touch)
    {
        bool isOnUI = PointOnUI(touch.Position);
        if(isOnUI)
        {
            switch(touch.Phase)
            {
                case TouchPhase.Began:
                    if(touch.Fake)
                    {
                        touch.faketouch.phase = TouchPhase.Canceled;
                    }
                    else
                    {
                        touch.Convert2FakeTouch(TouchPhase.Canceled);
                    }
                    break;
                case TouchPhase.Moved:
                case TouchPhase.Stationary:
                case TouchPhase.Canceled:
                case TouchPhase.Ended:
                    break;
            }
        }
        
        XVirtualTab.singleton.Feed(touch);
        XGesture.singleton.Feed(touch);
    }


}


