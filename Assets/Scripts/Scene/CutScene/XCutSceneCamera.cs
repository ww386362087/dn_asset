using UnityEngine;

public class XCutSceneCamera
{
    private GameObject _cameraObject = null;
    private GameObject _dummyObject = null;
    private Transform _dummyCamera = null;
    private Transform _cameraTransform = null;
    private Animator _ator = null;

    public AnimatorOverrideController _overrideController = new AnimatorOverrideController();

    private Camera _camera = null;
    private bool _root_pos_inited = false;
    private Vector3 _root_pos = Vector3.zero;
    private Quaternion _idle_root_rotation = Quaternion.identity;
    private Vector3 _dummyCamera_pos = Vector3.zero;
    private XCameraMotionData _motion = new XCameraMotionData();

    public XActor Target = null;

    private string _trigger = null;

    public Camera UnityCamera
    {
        get { return _camera; }
    }

    public bool Initialize()
    {
        _cameraObject = GameObject.Find(@"Main Camera");
        if (_cameraObject != null)
        {
            _camera = _cameraObject.GetComponent<Camera>();
            _cameraTransform = _cameraObject.transform;
            GameObject.DestroyImmediate(_dummyObject);
            _dummyObject = XResourceMgr.Load<GameObject>("Prefabs/DummyCamera", AssetType.Prefab);
            _dummyObject.name = "Dummy Camera";
            _dummyCamera = _dummyObject.transform.GetChild(0);
            _ator = _dummyObject.GetComponent<Animator>();
            _overrideController.runtimeAnimatorController = _ator.runtimeAnimatorController;
            _ator.runtimeAnimatorController = _overrideController;
            _root_pos_inited = false;
        }
        return true;
    }

    public void PostUpdate(float fDeltaT)
    {
        if (!_root_pos_inited)
        {
            _root_pos = _dummyCamera.position;
            _root_pos_inited = true;
            _idle_root_rotation = _motion.Follow_Position ? Quaternion.Euler(0, 180, 0) : Quaternion.identity;
        }
        InnerUpdateEx();
        TriggerEffect();
    }


    private void InnerUpdateEx()
    {
        _dummyCamera_pos = _idle_root_rotation * (_dummyCamera.position - _dummyObject.transform.position) + _dummyObject.transform.position;
        Vector3 v_self_p = Target == null ? Vector3.zero : Target.Actor.transform.position;
        Vector3 forward = Vector3.Cross(_dummyCamera.forward, _dummyCamera.up);
        Quaternion q = Quaternion.LookRotation(forward, _dummyCamera.up);
        Vector3 delta = _dummyCamera_pos - _root_pos;
        Vector3 target_pos = _root_pos + (_motion.Follow_Position ? v_self_p : Vector3.zero);
        _cameraTransform.rotation = _idle_root_rotation * q;
        target_pos += delta;
        _cameraTransform.position = target_pos;
    }

    public void Effect(XCameraMotionData motion)
    {
        AnimationClip clip = XResourceMgr.Load<AnimationClip>(motion.Motion, AssetType.Anim);
        if (clip != null)
        {
            _trigger = CameraTrigger.ToEffect.ToString();
            if (_overrideController["CameraEffect"] != clip)
                _overrideController["CameraEffect"] = clip;

            _motion.Follow_Position = motion.Follow_Position;
            _motion.AutoSync_At_Begin = motion.AutoSync_At_Begin;
            _motion.LookAt_Target = motion.LookAt_Target;
            _motion.Motion = motion.Motion;
        }
    }

    private void TriggerEffect()
    {
        if (_trigger != null && !_ator.IsInTransition(0))
        {
            _ator.SetTrigger(_trigger);
            _root_pos_inited = false;
            _trigger = null;
        }
    }


}
