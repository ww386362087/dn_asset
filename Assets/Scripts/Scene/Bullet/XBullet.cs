using System.Collections.Generic;
using UnityEngine;


internal class XBullet
{
    private struct XBulletTarget
    {
        public uint TimerToken;
        public bool Hurtable;
        public int HurtCount;
    }

    private bool _active = true;
    private bool _pingponged = false;
    private uint _tail_results_token = 0;
    private int _tail_results = 0;
    private float _elapsed = 0;

    private GameObject _bullet = null;
    private XBulletData _data = null;
    private RaycastHit _hitInfo;
    private Vector3 _origin = Vector3.zero;
    

    private Dictionary<XHitHoster, XBulletTarget> _hurt_target = new Dictionary<XHitHoster, XBulletTarget>();

    public XBullet(XBulletData data)
    {
        _data = data;
        _elapsed = 0.0f;

        _bullet = XResources.Load<GameObject>(data.Prefab, AssetType.Prefab);
        _bullet.transform.position = _data.BulletRay.origin;
        _bullet.transform.rotation = _data.Velocity > 0 ? Quaternion.LookRotation(_data.BulletRay.direction) : Quaternion.LookRotation(_data.Firer.Transform.forward);
        _data.Firer.ShownTransform = _bullet.transform;
    }

    public bool IsExpired()
    {
        if (_tail_results != 0)
        {
            return _tail_results >= _data.Skill.Result[_data.Sequnce].LongAttackData.TriggerAtEnd_Count;
        }
        if (_data.Skill.Result[_data.Sequnce].LongAttackData.IsPingPong && !_pingponged)
        {
            if (_elapsed > _data.Life) _pingponged = true;
        }
        bool expired = (!_active || (!_pingponged && _elapsed > _data.Life));
        if (_data.Skill.Result[_data.Sequnce].LongAttackData.TriggerAtEnd_Count > 0 && expired)
        {
            _active = false;
            OnTailResult(null);
        }
        return expired;
    }

    public bool IsHurtEntity(XHitHoster id)
    {
        XBulletTarget target;
        if (id != null && _hurt_target.TryGetValue(id, out target))
            return !target.Hurtable;
        return false;
    }

    private void OnTailResult(object o)
    {
        if (o == null)
        {
            _tail_results = 0;
            FakeDestroyBulletObject();
        }

        if (_tail_results < _data.Skill.Result[_data.Sequnce].LongAttackData.TriggerAtEnd_Count)
        {
            _tail_results++;
            TailResult(_tail_results == 1);
            XTimerMgr.singleton.RemoveTimer(_tail_results_token);
            _tail_results_token = XTimerMgr.singleton.SetTimer(_data.Skill.Result[_data.Sequnce].LongAttackData.TriggerAtEnd_Cycle, OnTailResult, this);
        }
    }

    private void TailResult(bool present)
    {
        if (_data.Skill.Result[_data.Sequnce].LongAttackData.TriggerAtEnd)
        {
            if (_data.Warning) _bullet.transform.position = _data.WarningPos;
            Result(null);
        }
        if (!present) return;
        if (!string.IsNullOrEmpty(_data.Skill.Result[_data.Sequnce].LongAttackData.End_Fx))
        {
            GameObject fx = XResources.Load<GameObject>(_data.Skill.Result[_data.Sequnce].LongAttackData.End_Fx, AssetType.Prefab);
            Vector3 pos = _bullet.transform.position;
            if (_data.Skill.Result[_data.Sequnce].LongAttackData.EndFx_Ground) pos.y = 0;
            fx.transform.position = pos;
            fx.transform.rotation = _bullet.transform.rotation;
            XResources.Destroy(fx, _data.Skill.Result[_data.Sequnce].LongAttackData.EndFx_LifeTime);
        }
        if (_data.Firer.ShownTransform == _bullet.transform)
            _data.Firer.ShownTransform = _data.Firer.Transform;
    }

    private void FakeDestroyBulletObject()
    {
        if (_bullet != null)
        {
            Vector3 pos = _bullet.transform.position;
            Quaternion quat = _bullet.transform.rotation;
            GameObject.Destroy(_bullet);
            _bullet = new GameObject("fakeBullet");
            _bullet.transform.position = pos;
            _bullet.transform.rotation = quat;
            _bullet.SetActive(true);
        }
    }

    public void Destroy()
    {
        XTimerMgr.singleton.RemoveTimer(_tail_results_token);
        if (_data.Skill.Result[_data.Sequnce].LongAttackData.TriggerAtEnd_Count == 0) TailResult(true);

        if (_bullet != null)
        {
            GameObject.Destroy(_bullet);
        }
        foreach (XBulletTarget bt in _hurt_target.Values)
        {
            XTimerMgr.singleton.RemoveTimer(bt.TimerToken);
        }
        _bullet = null;
        _data = null;
    }


    private void OnRefined(object o)
    {
        XBulletTarget bt;
        XHitHoster id = (XHitHoster)o;

        if (_hurt_target.TryGetValue(id, out bt))
        {
            if (bt.HurtCount < _data.Skill.Result[_data.Sequnce].LongAttackData.Refine_Count)
            {
                bt.Hurtable = true;
                _hurt_target[id] = bt;
            }
        }
    }

    public void Result(XHitHoster hit)
    {
        if (IsHurtEntity(hit)) return;

        //trigger skill result
        _data.Firer.skillResult.InnerResult(_data.Sequnce, _bullet.transform.forward, _bullet.transform.position, _data.Skill, hit);
        if (hit != null)
        {
            XBulletTarget bt;
            if (!_hurt_target.TryGetValue(hit, out bt))
            {
                bt = new XBulletTarget();
                _hurt_target.Add(hit, bt);
            }
            XTimerMgr.singleton.RemoveTimer(bt.TimerToken);
            bt.Hurtable = false;
            bt.HurtCount++;
            bt.TimerToken = _data.Skill.Result[_data.Sequnce].LongAttackData.Refine_Cycle > 0 ?
                XTimerMgr.singleton.SetTimer(_data.Skill.Result[_data.Sequnce].LongAttackData.Refine_Cycle, OnRefined, hit) : 0;
            _hurt_target[hit] = bt;
        }
        if (_data.Skill.Result[_data.Sequnce].LongAttackData.TriggerOnce)
        {
            _pingponged = _data.Skill.Result[_data.Sequnce].LongAttackData.IsPingPong;
        }
    }

    public void Update(float fDeltaT)
    {
        if (!_active) return;
        _elapsed += fDeltaT;
        float dis = 0; Vector3 dir = Vector3.forward;
        switch (_data.Skill.Result[_data.Sequnce].LongAttackData.Type)
        {
            case XResultBulletType.Ring:
                _bullet.transform.position = _data.Firer.Transform.position;
                break;
            case XResultBulletType.Sphere:
            case XResultBulletType.Plane:
                dis = (_elapsed > _data.Runningtime && _elapsed < _data.Life) ? 0 : _data.Velocity * fDeltaT;
                dir = _bullet.transform.forward;
                break;
            case XResultBulletType.Satellite:
                if (_elapsed - fDeltaT == 0)
                {
                    _bullet.transform.position = _data.Firer.Transform.position + _data.BulletRay.direction * _data.Skill.Result[_data.Sequnce].LongAttackData.RingRadius;
                    dis = 0;
                    dir = XCommon.singleton.HorizontalRotateVetor3(_data.Firer.Transform.forward, _data.Skill.Result[_data.Sequnce].LongAttackData.Palstance < 0 ? -90 : 90);
                }
                else
                {
                    Vector3 curr = XCommon.singleton.HorizontalRotateVetor3(_data.BulletRay.direction, _data.Skill.Result[_data.Sequnce].LongAttackData.Palstance * (_elapsed - fDeltaT)) * _data.Skill.Result[_data.Sequnce].LongAttackData.RingRadius;
                    Vector3 next = XCommon.singleton.HorizontalRotateVetor3(_data.BulletRay.direction, _data.Skill.Result[_data.Sequnce].LongAttackData.Palstance * _elapsed) * _data.Skill.Result[_data.Sequnce].LongAttackData.RingRadius;

                    _bullet.transform.rotation = XCommon.singleton.VectorToQuaternion(XCommon.singleton.Horizontal(next - curr));
                    next += _data.Firer.Transform.position;
                    Vector3 d = next - _bullet.transform.position; d.y = 0;
                    dis = d.magnitude;
                    dir = d.normalized;
                }
                break;
        }

        if (_data.Skill.Result[_data.Sequnce].LongAttackData.IsPingPong && _pingponged)
        {
            Vector3 v = _data.Firer.Transform.position - _bullet.transform.position;
            v.y = 0;
            if (dis >= Vector3.Magnitude(v))
            {
                dis = Vector3.Magnitude(v);
                _active = false;
            }
            dir = XCommon.singleton.Horizontal(v);
        }
        else
        {
            if (_data.Target != null && _data.Skill.Result[_data.Sequnce].LongAttackData.Follow)
            {
                dir = XCommon.singleton.Horizontal(_data.Target.transform.position - _bullet.transform.position);
            }
        }

        if (_data.Skill.Result[_data.Sequnce].LongAttackData.Type != XResultBulletType.Satellite) _bullet.transform.rotation = XCommon.singleton.VectorToQuaternion(dir);
        Vector3 move = dir * dis;
        _origin.Set(_bullet.transform.position.x, _bullet.transform.position.y, _bullet.transform.position.z);

        if (_active)
        {
            _bullet.transform.position += move;
            if (_data.Skill.Result[_data.Sequnce].LongAttackData.Manipulation)
            {
                XHitHoster[] hits = GameObject.FindObjectsOfType<XHitHoster>();
                Vector3 center = _bullet.transform.position;
                foreach (XHitHoster hit in hits)
                {
                    Vector3 gap = center - hit.transform.position;
                    gap.y = 0;
                    if (gap.magnitude < _data.Skill.Result[_data.Sequnce].LongAttackData.ManipulationRadius)
                    {
                        float len = _data.Skill.Result[_data.Sequnce].LongAttackData.ManipulationForce * fDeltaT;
                        hit.transform.Translate(gap.normalized * Mathf.Min(dis, len), Space.World);
                    }
                }
            }
        }

        if (_data.Skill.Result[_data.Sequnce].LongAttackData.WithCollision)
        {
            switch (_data.Skill.Result[_data.Sequnce].LongAttackData.Type)
            {
                case XResultBulletType.Ring:
                    float t = _elapsed > _data.Life ? 0 : (_data.Skill.Result[_data.Sequnce].LongAttackData.RingFull ? (_elapsed > _data.Life * 0.5f ? (_data.Life - _elapsed) : _elapsed) : _elapsed);
                    float ir = t * _data.Skill.Result[_data.Sequnce].LongAttackData.RingVelocity;
                    float or = ir + _data.Skill.Result[_data.Sequnce].LongAttackData.RingRadius;
                    RingCollideUnit(ir, or, _data.Firer.Transform.position, this);
                    break;
                case XResultBulletType.Sphere:
                case XResultBulletType.Satellite:
                    Vector3 project = new Vector3(move.x, 0, move.z);
                    float hlen = project.magnitude * 0.5f;
                    dir.y = 0;
                    float rotation = (dir.sqrMagnitude == 0) ? 0 : Vector3.Angle(Vector3.right, dir);
                    if (rotation > 0 && XCommon.singleton.Clockwise(Vector3.right, dir)) rotation = -rotation;
                    BulletCollideUnit(
                        new Vector3(_origin.x + dir.x * hlen, 0, _origin.z + dir.z * hlen),
                        hlen,
                        rotation,
                        _data.Radius,
                        this);
                    break;
                case XResultBulletType.Plane:
                    PlaneBulletCollideUnit(_origin, move, _data.Radius, this);
                    break;
            }
        }
    }

    private static void RingCollideUnit(float ir, float or, Vector3 center, XBullet bullet)
    {
        XHitHoster[] ents = GameObject.FindObjectsOfType<XHitHoster>();
        for (int i = 0; i < ents.Length; i++)
        {
            bool collided = false;
            Vector3 v = ents[i].transform.position - center; v.y = 0;
            float dis = v.sqrMagnitude;
            collided = dis > (ir * ir) && dis < (or * or);
            if (collided) bullet.Result(ents[i]);
            if (bullet.IsExpired()) break;
        }
    }

    private static void BulletCollideUnit(Vector3 rectcenter, float hlen, float rotation, float r, XBullet bullet)
    {
        XHitHoster[] ents = GameObject.FindObjectsOfType<XHitHoster>();
        for (int i = 0; i < ents.Length; i++)
        {
            bool collided = false;
            Vector3 cycle = ents[i].RadiusCenter;
            cycle -= rectcenter;
            cycle.y = 0;
            cycle = XCommon.singleton.HorizontalRotateVetor3(cycle, rotation, false);
            collided = XCommon.singleton.IsRectCycleCross(hlen, r, cycle, ents[i].Radius) || Vector3.SqrMagnitude(cycle) < r * r;
            if (collided) bullet.Result(ents[i]);
            if (bullet.IsExpired()) break;
        }
    }

    private static void PlaneBulletCollideUnit(Vector3 origin, Vector3 move, float r, XBullet bullet)
    {
        Vector3 side = XCommon.singleton.HorizontalRotateVetor3(move, 90);
        Vector3 left = origin + side * r;
        Vector3 right = origin - side * r;

        XHitHoster[] ents = GameObject.FindObjectsOfType<XHitHoster>();

        for (int i = 0; i < ents.Length; i++)
        {
            bool collided = false;
            Vector3 pos = ents[i].RadiusCenter;
            collided = XCommon.singleton.IsLineSegmentCross(pos, pos - move, left, right);
            if (collided) bullet.Result(ents[i]);
            if (bullet.IsExpired()) break;
        }
    }

}

