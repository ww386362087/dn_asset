using UnityEngine;

public class XSkillCamera
{
    public XSkillCamera(GameObject hoster)
    {
        _hoster = hoster;
    }

    private GameObject _hoster = null;
    private enum XCameraExStatus
    {
        Idle,
        Dash,
        Effect,
        UltraShow,
        UltraEnd
    }

    private bool _status_changed = false;

    private XCameraExStatus _status = XCameraExStatus.Idle;

    private GameObject _cameraObject = null;

    private GameObject _dummyObject = null;
    private Transform _dummyCamera = null;
    private Transform _cameraTransform = null;

    private Animator _ator = null;
    public AnimatorOverrideController _overrideController = new AnimatorOverrideController();

    private string _trigger = null;

    private Camera _camera = null;

    private float _elapsed = 0;

    private bool _damp = false;
    private float _damp_delta = 0;
    private Vector3 _damp_dir = Vector3.zero;

    private bool _follow_position = true;
    private bool _relative_to_idle = false;
    private bool _look_at = false;
    private bool _sync_begin = false;
    private CameraMotionSpace _effect_axis = CameraMotionSpace.World;

    private bool _root_pos_inited = false;
    private bool _idle_root_pos_inited = false;

    private Vector3 _root_pos = Vector3.zero;
    private Vector3 _idle_root_pos = Vector3.zero;

    private Vector3 _v_self_p = Vector3.zero;
    private Quaternion _q_self_r = Quaternion.identity;

    private Quaternion _idle_root_rotation = Quaternion.identity;
    private float _idle_root_rotation_y = 0;

    private Vector3 _last_dummyCamera_pos = Vector3.zero;
    private Vector3 _dummyCamera_pos = Vector3.zero;

    private readonly float _damp_factor = 1.0f;

    private XCameraMotionData _motion = new XCameraMotionData();

    //kill all timer when leave scene.
    private uint _token = 0;

    public Camera UnityCamera
    {
        get { return _camera; }
    }

    public Transform CameraTrans
    {
        get { return _cameraTransform; }
    }

    public Animator CameraAnimator
    {
        get { return _ator; }
    }

    public Vector3 Position
    {
        get { return _cameraTransform.position; }
    }

    public Quaternion Rotaton
    {
        get { return _cameraTransform.rotation; }
    }

    public void Initialize()
    {
        _cameraObject = GameObject.Find(@"Main Camera");

        if (_cameraObject != null)
        {
           
            _camera = _cameraObject.GetComponent<Camera>();
            _cameraTransform = _cameraObject.transform;

            XResourceMgr.SafeDestroy(ref _dummyObject);

            _dummyObject = XResourceMgr.Load<GameObject>("Prefabs/DummyCamera", AssetType.Prefab);
            _dummyObject.name = "Dummy Camera";

            _dummyCamera = _dummyObject.transform.GetChild(0);
            _ator = _dummyObject.GetComponent<Animator>();
            _overrideController.runtimeAnimatorController = _ator.runtimeAnimatorController;
            _ator.runtimeAnimatorController = _overrideController;

            _root_pos_inited = false;
            _idle_root_pos_inited = false;
            _status = XCameraExStatus.Idle;
            _status_changed = false;
            _idle_root_rotation_y = 0;
        }
    }

    public void Damp()
    {
        _damp = true;
        _elapsed = 0;
    }

    public void YRotate(float addation)
    {
        if (addation != 0)
        {
            _idle_root_rotation_y += addation;
            _idle_root_rotation = Quaternion.Euler(0, _idle_root_rotation_y, 0);
            _root_pos = _idle_root_rotation * _dummyCamera.position;
        }
    }

    public void OverrideAnimClip(string motion, string clipname)
    {
        //get override clip
        AnimationClip animClip = XResourceMgr.Load<AnimationClip>(clipname, AssetType.Anim);
        OverrideAnimClip(motion, animClip);
    }

    public void OverrideAnimClip(string motion, AnimationClip clip)
    {
        //override
        if (clip != null && _overrideController[motion] != clip) _overrideController[motion] = clip;
    }

    public void Effect(XCameraMotionData motion)
    {
        Effect(motion, "ToEffect");
    }

 
    public void PostUpdate(float fDeltaT)
    {
        if (!_root_pos_inited)
        {
            _idle_root_rotation = Quaternion.Euler(0, _idle_root_rotation_y, 0);
            _root_pos = _idle_root_rotation * _dummyCamera.position;
            _root_pos_inited = true;
            if (!_idle_root_pos_inited)
            {
                _idle_root_pos = _idle_root_rotation * _dummyCamera.position;
                _idle_root_pos_inited = true;
            }
        }

        InnerUpdateEx();
        TriggerEffect();
    }

    private void AutoSync()
    {
        _q_self_r = _hoster.transform.rotation;
        _v_self_p = _hoster.transform.position;
    }

    private void InnerPosition()
    {
        _dummyCamera_pos = _idle_root_rotation * _dummyCamera.position;

        if (_damp)
        {
            _damp_dir = (_dummyCamera_pos - _last_dummyCamera_pos);
            if (_elapsed == 0) _damp_delta = _damp_dir.magnitude;
            _damp_dir.Normalize();

            if (_elapsed > _damp_factor)
            {
                _elapsed = _damp_factor;
                _damp = false;
            }
            _dummyCamera_pos = _dummyCamera_pos - _damp_dir * (_damp_delta * ((_damp_factor - _elapsed) / _damp_factor));
        }
        _last_dummyCamera_pos = _dummyCamera_pos;
    }

    private void InnerUpdateEx()
    {
        InnerPosition();

        Quaternion q_self_r = _hoster.transform.rotation;
        Vector3 v_self_p = _hoster.transform.position;
        Vector3 r = _dummyCamera.rotation.eulerAngles; r.y -= 90;
        float f = r.x; r.x = r.z; r.z = f;

        if (_status_changed || _status == XCameraExStatus.Idle) _status_changed = false;

        Vector3 delta = (_dummyCamera_pos - _root_pos);
        Vector3 target_pos = (_sync_begin ? _q_self_r : Quaternion.identity) * (_relative_to_idle ? _idle_root_pos : _root_pos);
        delta = (_sync_begin ? _q_self_r : Quaternion.identity) * delta;
        if (!_look_at) _cameraTransform.rotation = _idle_root_rotation * (_sync_begin ? _q_self_r : Quaternion.identity) * Quaternion.Euler(r);
        target_pos += (_follow_position ? v_self_p : (_sync_begin ? _v_self_p : Vector3.zero));

        switch (_effect_axis)
        {
            case CameraMotionSpace.World:
                    target_pos += delta;
                break;
            case CameraMotionSpace.Self:
                    target_pos += (_follow_position ? Quaternion.identity : q_self_r) * delta;
                break;
        }
        _cameraTransform.position = target_pos;
        if (_look_at) _cameraTransform.LookAt(_hoster.transform.position + _dummyObject.transform.position);
    }

    public void EndEffect(object o)
    {
        if (_status == XCameraExStatus.Idle) return;

        _trigger = "ToIdle";
        _motion.Follow_Position = true;
        _motion.Coordinate = CameraMotionSpace.World;
        _motion.AutoSync_At_Begin = false;
        _motion.LookAt_Target = true;
        _motion.Motion = null;
    }

    public void Effect(XCameraMotionData motion, bool overrideclip)
    {
        //must be called from UPDATE pass
        AnimationClip clip = XResourceMgr.Load<AnimationClip>(motion.Motion3D, AssetType.Anim);

        if (clip != null)
        {
            _trigger = "ToEffect";
            if (overrideclip && _overrideController["CameraEffect"] != clip) _overrideController["CameraEffect"] = clip;

            _motion.LookAt_Target = motion.LookAt_Target;
            _motion.Follow_Position = true;
            _motion.Coordinate = CameraMotionSpace.World;

            switch (motion.Motion3DType)
            {
                case CameraMotionType.AnchorBased:
                    _motion.AutoSync_At_Begin = true;
                    _motion.LookAt_Target = false;
                    break;
                case CameraMotionType.CameraBased:
                    _motion.AutoSync_At_Begin = false;
                    break;
            }
            _motion.Motion = motion.Motion3D;
        }
    }

    public void Effect(XCameraMotionData motion, string trigger)
    {
        _trigger = trigger;
        _motion.LookAt_Target = motion.LookAt_Target;
        _motion.Follow_Position = true;
        _motion.Coordinate = CameraMotionSpace.World;

        switch (motion.Motion3DType)
        {
            case CameraMotionType.AnchorBased:
                _motion.AutoSync_At_Begin = true;
                _motion.LookAt_Target = false;
                break;
            case CameraMotionType.CameraBased:
                _motion.AutoSync_At_Begin = false;
                break;
        }
        _motion.Motion = motion.Motion3D;
    }

    private void TriggerEffect()
    {
        if (_trigger != null && !_ator.IsInTransition(0))
        {
            switch (_trigger)
            {
                case "ToIdle":
                    _status = XCameraExStatus.Idle;
                    _idle_root_pos_inited = false;
                    break;
                case "ToEffect":
                    _status = XCameraExStatus.Effect;
                    break;
                case "ToDash":
                    _status = XCameraExStatus.Dash;
                    break;
                case "ToUltraShow":
                    _status = XCameraExStatus.UltraShow;
                    break;
                case "ToUltraEnd":
                    _status = XCameraExStatus.UltraEnd;
                    break;
            }

            XTimerMgr.singleton.RemoveTimer(_token);

            _follow_position = _motion.Follow_Position;
            _effect_axis = _motion.Coordinate;
            _sync_begin = _motion.AutoSync_At_Begin;
            _look_at = _motion.LookAt_Target;

            if (_sync_begin) AutoSync();

            _ator.SetTrigger(_trigger);
            _root_pos_inited = false;
            _status_changed = true;
            _trigger = null;
        }
    }
}
