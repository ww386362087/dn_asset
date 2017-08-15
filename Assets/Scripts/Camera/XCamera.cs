using UnityEngine;

public class XCamera : XObject
{

    private XEntity _active_target = null;
    private float _field_of_view = 45;
    private GameObject _dummyObject = null;
    private Transform _dummyCamera = null;
    private Transform _cameraTransform = null;
    private Camera _camera;
    private GameObject _cameraObject = null;
    private Animator _ator = null;
    private AnimatorOverrideController _overrideController;

    private float _tdis = 4.2f;
    private float _basic_dis = 4.2f;
    //basic root
    private bool _root_pos_inited = false;
    private Vector3 _root_pos = Vector3.zero;
    private Quaternion _idle_root_rotation = Quaternion.identity;
    private bool _init_idle_root_basic_x = false;

    private float _idle_root_rotation_x = 0;
    private float _idle_root_rotation_y = 0;

    //position & rotation
    private Vector3 _dummyCamera_pos = Vector3.zero;
    private Quaternion _dummyCamera_quat = Quaternion.identity;


    public Transform CameraTrans { get { return _cameraTransform; } }

    public Vector3 Position { get { return _cameraTransform.position; } }

    public Quaternion Rotaton { get { return _cameraTransform.rotation; } }

    public float FieldOfView { get { return _field_of_view; } }

    public float TargetOffset { get { return _tdis; } set { _tdis = value; } }

    public Camera UnityCamera { get { return _camera; } }

    public Animator Ator { get { return _ator; } }

    public XEntity Target
    {
        get { return (_active_target == null || _active_target.Deprecated) ? null : _active_target; }
        set { _active_target = value; }
    }

    public void PreInitial(GameObject camera)
    {
        base.Initilize();
        _cameraObject = camera;
        _cameraTransform = camera.transform;

        if (_cameraObject != null)
        {
            _camera = _cameraObject.GetComponent<Camera>();
            _camera.enabled = true;
            _field_of_view = _camera.fieldOfView;

            if (_dummyObject == null)
            {
                _dummyObject = XResourceMgr.Load<GameObject>("Prefabs/DummyCamera", AssetType.Prefab);
                _dummyObject.name = "Dummy Camera";
            }
            _dummyCamera = _dummyObject.transform.GetChild(0);
            _ator = _dummyObject.GetComponent<Animator>();
            if (_overrideController == null)
            {
                _overrideController = new AnimatorOverrideController();
            }
            _overrideController.runtimeAnimatorController = _ator.runtimeAnimatorController;
            _ator.runtimeAnimatorController = _overrideController;
        }
    }


    public void Initial()
    {
        XPlayer player = XEntityMgr.singleton.player;
        _active_target = player;
    }

    public void Uninitial()
    {
        _camera = null;
        GameObject.Destroy(_dummyObject);
        base.Unload();
    }


    public void LateUpdate()
    {
        if (!_root_pos_inited)
        {
            Vector3 forward = Vector3.Cross(_dummyCamera.forward, _dummyCamera.up);
            _dummyCamera_quat = Quaternion.LookRotation(forward, _dummyCamera.up);

            if (!_init_idle_root_basic_x) _basic_dis = (_dummyCamera.position - _dummyObject.transform.position).magnitude;
            _idle_root_rotation = Quaternion.Euler(_idle_root_rotation_x, _idle_root_rotation_y, 0);
            _root_pos = _idle_root_rotation * _dummyCamera.position;
            _root_pos_inited = true;
            _init_idle_root_basic_x = true;
        }
        if (_active_target != null) InnerUpdateEx();
    }

    private void InnerUpdateEx()
    {
        InnerPosition();

        Quaternion rotation = Quaternion.identity;
        if (_active_target != null) rotation = _active_target.Rotation;
        Vector3 v_self_p = Target == null ? Vector3.zero : Target.Position;
        Vector3 forward = Vector3.Cross(_dummyCamera.forward, _dummyCamera.up);
        _dummyCamera_quat = Quaternion.LookRotation(forward, _dummyCamera.up);

        Vector3 delta = _dummyCamera_pos - _root_pos;
        Vector3 target_pos = Quaternion.identity * _root_pos;
        delta = Quaternion.identity * delta;

        _cameraTransform.rotation = Quaternion.identity * _idle_root_rotation * _dummyCamera_quat;
        target_pos += v_self_p;
        target_pos += delta;
        _cameraTransform.position = target_pos;
        LookAtTarget();
    }


    private void InnerPosition()
    {
        Vector3 dummyCamera = _dummyCamera.position;
        if (Target.IsPlayer)
        {
            Vector3 offset_dir = (_dummyCamera.position - _dummyObject.transform.position);
            float offset_dis = offset_dir.magnitude; offset_dir.Normalize();
            if (offset_dir.z > 0)
            {
                offset_dis = -offset_dis;
                offset_dir = -offset_dir;
            }
            float effect = (offset_dis - _basic_dis);
            float dis = TargetOffset + effect;
            if (dis <= 0) dis = 0.1f;
            dummyCamera = _dummyObject.transform.position + dis * offset_dir;
        }
        _dummyCamera_pos = _idle_root_rotation * (dummyCamera - _dummyObject.transform.position) + (_dummyObject.transform.position);
    }

    public void LookAtTarget()
    {
        if (Target != null)
        {
            Vector3 pos = Target.Position + (_dummyCamera == null ? Vector3.zero : _dummyCamera.position);
            _cameraTransform.LookAt(pos);
        }
    }

    public bool IsVisibleFromCamera(XEntity entity, bool fully)
    {
        Plane[] planes = GeometryUtility.CalculateFrustumPlanes(_camera);
        return entity.TestVisible(planes, fully);
    }


    public void OverrideAnimClip(string motion, AnimationClip clip)
    {
        if (clip != null)
        {
            if (_overrideController[motion] != clip)
                _overrideController[motion] = clip;
        }
        else
        {
            _overrideController[motion] = null;
        }
    }

    //SetCameraLayer(XPlayer.PlayerLayer, true);
    public void SetCameraLayer(int layer, bool add)
    {
        if (add)
        {
            _camera.cullingMask |= 1 << layer;
        }
        else
        {
            _camera.cullingMask &= ~(1 << layer);
        }
    }

    public void SetCameraLayer(int layermask)
    {
        _camera.cullingMask = layermask;
    }

    public int GetCameraLayer()
    {
        return _camera.cullingMask;
    }

    public void SetSolidBlack(bool enabled)
    {
        if (enabled)
        {
            _camera.clearFlags = CameraClearFlags.SolidColor;
            _camera.backgroundColor = Color.black;
        }
        else
        {
            _camera.clearFlags = CameraClearFlags.Skybox;
        }
    }


}
