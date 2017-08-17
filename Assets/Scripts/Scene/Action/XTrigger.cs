using UnityEngine;

public abstract class XTrigger : MonoBehaviour
{
    protected IXPlayerAction _interface;
    private CapsuleCollider _cap = null;

    // Use this for initialization
    void Awake()
    {
        _cap = GetComponent<CapsuleCollider>();
        _cap.enabled = false;
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
                CollisionDetected(pos);
            }
        }
    }

    private void CollisionDetected(Vector3 pos)
    {
        Vector3 delta = (pos - (_cap.transform.position + _cap.center)); delta.y = 0;
        if (delta.sqrMagnitude < _cap.radius * _cap.radius)
        {
            OnTriggered();
        }
    }

    protected abstract void OnTriggered();
}
