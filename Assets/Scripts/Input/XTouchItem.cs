using UnityEngine;

internal class XTouchItem
{
    public bool Fake { get; set; }

    public Touch touch;

    public XFakeTouch faketouch;

    public float DeltaTime
    {
        get { return Fake ? faketouch.deltaTime : touch.deltaTime; }
    }

    public int FingerId
    {
        get { return Fake ? faketouch.fingerId : touch.fingerId; }
    }

    public TouchPhase Phase
    {
        get { return Fake ? faketouch.phase : touch.phase; }
    }

    public Vector2 Position
    {
        get { return Fake ? faketouch.position : touch.position; }
    }

    public Vector2 RawPosition
    {
        get { return Fake ? faketouch.rawPosition : touch.rawPosition; }
    }

    public int TapCount
    {
        get { return Fake ? faketouch.tapCount : touch.tapCount; }
    }

    public void Convert2FakeTouch(TouchPhase phase)
    {
        faketouch.fingerId = touch.fingerId;
        faketouch.position = touch.position;
        faketouch.deltaTime = touch.deltaTime;
        faketouch.deltaPosition = touch.deltaPosition;
        faketouch.phase = phase;
        faketouch.tapCount = touch.tapCount;
        Fake = true;
    }

}

