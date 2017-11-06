using UnityEngine;

internal class XBulletData
{
    private XSkillData _data = null;
    private ISkillHoster _hoster = null;
    private Vector3 _warning_pos = Vector3.zero;
    private int _sequnce = 0;
    private float _velocity = 0;
    private bool _warning = false;
    private GameObject _target = null;
    private IHitHoster[] _hits = null;

    public Ray BulletRay;
    public GameObject Target { get { return _target; } }
    public Vector3 WarningPos { get { return _warning_pos; } }
    public bool Warning { get { return _warning; } }
    public XSkillData Skill { get { return _data; } }
    public ISkillHoster Firer { get { return _hoster; } }
    public string Prefab { get { return _data.Result[_sequnce].LongAttackData.Prefab; } }
    public int Sequnce { get { return _sequnce; } }
    public float Velocity { get { return _velocity; } }
    public float Life { get { return _data.Result[_sequnce].LongAttackData.Runningtime + _data.Result[_sequnce].LongAttackData.Stickytime; } }
    public float Runningtime { get { return _data.Result[_sequnce].LongAttackData.Runningtime; } }
    public float Radius { get { return _data.Result[_sequnce].LongAttackData.Radius; } }
    public IHitHoster[] Hits { get { return _hits; } }

    public XBulletData(ISkillHoster firer, XSkillData data, GameObject target, int idx, float diviation, int wid,IHitHoster[] hits)
    {
        _sequnce = idx;
        _data = data;
        _hoster = firer;
        _hits = hits;
        _warning_pos = Vector3.zero;

        if (data.Result[idx].Attack_All)
        {
            _warning_pos = target.transform.position;
        }
        else if (data.Result[idx].Warning)
        {
            _warning_pos = firer.Attribute.skillWarning.warningPosAt[data.Result[idx].Warning_Idx][wid];
        }

        _warning = _warning_pos.sqrMagnitude > 0;
        float height = _hoster.Present_data.BoundHeight;
        Vector3 begin = _hoster.Transform.position; begin.y += height * 0.5f;
        Vector3 dir = _warning ? (_warning_pos - _hoster.Transform.position) : firer.Transform.forward;
        begin += firer.Transform.rotation * new Vector3(
            data.Result[idx].LongAttackData.At_X,
            data.Result[idx].LongAttackData.At_Y,
            data.Result[idx].LongAttackData.At_Z);
        dir.y = 0;
        Vector3 flyTo = XCommon.singleton.HorizontalRotateVetor3(dir.normalized, diviation);
        float h = (_data.Result[_sequnce].LongAttackData.AimTargetCenter && firer.Target != null) ? (begin.y - height * 0.5f) : 0;
        _velocity = Warning ? (WarningPos - begin).magnitude / Runningtime : _data.Result[_sequnce].LongAttackData.Velocity;
        flyTo = (h == 0 || _velocity == 0) ? flyTo : (h * Vector3.down + _velocity * Runningtime * flyTo).normalized;

        BulletRay = new Ray(begin, flyTo);
        _target = _data.Result[_sequnce].LongAttackData.Follow ? firer.Target : null;
    }
    
}
