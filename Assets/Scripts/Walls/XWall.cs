using UnityEngine;

public abstract class XWall : MonoBehaviour
{
    protected bool _forward_collision;
    protected IXPlayerAction _interface;

    private BoxCollider _box = null;

    private Vector3 _left;
    private Vector3 _right;

    // Use this for initialization
    void Awake()
    {
        _box = GetComponent<BoxCollider>();
        _box.enabled = false;

        Vector3 half = Vector3.Cross(Vector3.up, transform.forward) * _box.size.x * _box.transform.localScale.x * 0.5f;

        _left = _box.center + _box.transform.position - half;
        _right = _box.center + _box.transform.position + half;
    }

     void Update()
    {
        XPlayer player = XEntityMgr.singleton.player;
        if (player != null)
        {
            Vector3 pos = player.EntityObject.transform.position;
            Vector3 last_pos = player.lastpos;

            if ((last_pos - pos).sqrMagnitude > 0)
            {
                CollisionDetected(pos, last_pos);
            }
        }
    }

    private void CollisionDetected(Vector3 pos, Vector3 last)
    {
        if (XCommon.singleton.IsLineSegmentCross(last, pos, _left, _right))
        {
            Vector3 dir = pos - last;
            _forward_collision = Vector3.Dot(dir, transform.forward) > 0;
            OnTriggered();
        }
    }

    protected abstract void OnTriggered();
}
