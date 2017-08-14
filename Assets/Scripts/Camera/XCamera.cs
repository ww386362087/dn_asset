using UnityEngine;

public class XCamera : XObject
{

    public enum XStatus
    {
        None,
        Idle,
        Solo,
        Effect
    }


    private XEntity _active_target = null;
    private float _field_of_view = 45;
    private GameObject _dummyObject = null;
    private Transform _dummyCamera = null;
    private Transform _cameraTransform = null;
    private Camera _camera;
    private GameObject _cameraObject = null;
   
    private Animator _ator = null;
    private AnimatorOverrideController _overrideController;

    public Transform CameraTrans { get { return _cameraTransform; } }

    public Vector3 Position { get { return _cameraTransform.position; } }

    public Quaternion Rotaton { get { return _cameraTransform.rotation; } }

    public float FieldOfView { get { return _field_of_view; } }

    public Camera UnityCamera { get { return _camera; } }

    public Animator Ator { get { return _ator; } }


    public XEntity Target
    {
        get { return (_active_target == null || _active_target.Deprecated) ? null : _active_target; }
        set { _active_target = value; }
    }


    public void Initial(GameObject camera)
    {
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


            XPlayer player = XEntityMgr.singleton.player;
            if (player != null)
            {
                _cameraObject.transform.rotation = player.Rotation;
                _cameraObject.transform.position = player.Position + new Vector3(-1, 1, 1);
            }
        }
    }

    public void Uninitial()
    {
        _camera = null;

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




}
