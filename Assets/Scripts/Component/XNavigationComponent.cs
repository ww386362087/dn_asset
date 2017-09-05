using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class XNavigationComponent : XComponent
{
    private NavMeshAgent _nav = null;
    private NavMeshPath _path = new NavMeshPath();
    private Vector3 _destination = Vector3.zero;
    private Vector3 _forward = Vector3.forward;
    private IEnumerator<Vector3> _nodes = null;
    private bool _bFoundNext = false;
    private bool _bNav = false;

    XEntity _entity;

    public bool IsOnNav { get { return _bNav; } }

    public override void OnInitial(XObject o)
    {
        base.OnInitial(o);
        _entity = o as XEntity;
        _bNav = false;
    }


    public override void OnUninit()
    {
        Interrupt();
        if (_nav != null) GameObject.Destroy(_nav);
        base.OnUninit();
    }


    public void Interrupt()
    {
        _path.ClearCorners();
        _bNav = false;
    }

    private void Navigate(Vector3 targetPos)
    {
        if (_nav == null) return;
        _path.ClearCorners();
        _nav.enabled = true;
        _nav.CalculatePath(targetPos, _path);
        _nav.enabled = false;
#if UNITY_EDITOR
        /*if (_path.corners.Length > 0)
        {
            Debug.DrawLine(_entity.MoveObj.Position, _path.corners[0], Color.red, 1);

            for (int i = 1; i < _path.corners.Length; i++)
            {
                Debug.DrawLine(_path.corners[i], _path.corners[i - 1], Color.red, 1);
            }
        }*/
#endif
        _nodes = TooShort() ? null : GetEnumerator();
        _destination = _entity.Position;
        _bFoundNext = (_nodes != null && _nodes.MoveNext());
        _bNav = true;
        MoveNext();
    }

    private bool TooShort()
    {
        return _path.corners.Length == 2 && (_path.corners[0] - _path.corners[1]).magnitude < 2;
    }

    private void MoveNext()
    {
        if (_bFoundNext)
        {
            Vector3 last = _destination;
            _destination = _nodes.Current;
            _bFoundNext = _nodes.MoveNext();
            if (_entity.IsPlayer)
            {
                if (!_bFoundNext)
                {
                    Vector3 vec = _entity.Position - _destination;
                    Vector3 forward = XCommon.singleton.Horizontal(vec);
                    if (vec.magnitude < 1)
                        _destination = last;
                    else
                        _destination += forward;
                }
            }
            _forward = XCommon.singleton.Horizontal(_destination - _entity.Position);
            if (_forward.sqrMagnitude == 0) _forward = _entity.Forward;
        }
        else
        {
            _bNav = false;
        }
    }

    public IEnumerator<Vector3> GetEnumerator()
    {
        for (int i = 1; i < _path.corners.Length; i++)
        {
            yield return _path.corners[i];
        }
    }

}
