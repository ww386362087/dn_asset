using UnityEngine;

/// <summary>
/// Only used by cutscene editor tool
/// </summary>
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
            _dummyObject = XResources.Load<GameObject>("Prefabs/DummyCamera", AssetType.Prefab);
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
        }
        InnerUpdateEx();
        TriggerEffect();
    }
    
    private void InnerUpdateEx()
    {
        _dummyCamera_pos = _idle_root_rotation * (_dummyCamera.position - _dummyObject.transform.position) + _dummyObject.transform.position;
        Vector3 forward = Vector3.Cross(_dummyCamera.forward, _dummyCamera.up);
        Quaternion q = Quaternion.LookRotation(forward, _dummyCamera.up);
        _cameraTransform.rotation = _idle_root_rotation * q;

        Vector3 delta = _dummyCamera_pos - _root_pos;
        Vector3 target_pos = _root_pos;
        target_pos += delta;
        _cameraTransform.position = target_pos;
    }

    public void Effect(string motion,string trigger)
    {
        AnimationClip clip = XResources.Load<AnimationClip>(motion, AssetType.Anim);
        if (clip != null)
        {
            _trigger = trigger;
            if (_overrideController["CameraEffect"] != clip)
                _overrideController["CameraEffect"] = clip;
        }
    }

    private void TriggerEffect()
    {
        if (_trigger != null && !_ator.IsInTransition(0))
        {
            _ator.SetTrigger(_trigger);
            _trigger = null;
        }
    }

}
