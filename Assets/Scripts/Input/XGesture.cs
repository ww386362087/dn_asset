using System.Collections;
using System.Collections.Generic;
using UnityEngine;

internal class XGesture:XSingleton<XGesture>
{
    private readonly float _dead_zone = 10;
    private bool _bTouch = false;
    private int _finger_id = -1;


    public int FingerId { get { return _finger_id; } }

    public void Feed(XTouchItem touch)
    {

    }

}


