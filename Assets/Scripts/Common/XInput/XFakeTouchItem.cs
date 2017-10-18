using UnityEngine;

internal struct XFakeTouch
{

    public Vector2 deltaPosition { get; set; }

    public float deltaTime { get; set; }

    public int fingerId { get; set; }

    public TouchPhase phase { get; set; }

    public Vector2 position { get; set; }

    public Vector2 rawPosition { get; set; }

    public int tapCount { get; set; }


}