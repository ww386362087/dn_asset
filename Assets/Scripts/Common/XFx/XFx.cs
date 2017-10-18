using System.Collections.Generic;
using UnityEngine;

public class XFx
{
    enum ECallbackCmd
    {
        ESyncActive = 1,
        ESyncPlay = 1 << 1,
        ESyncLayer = 1 << 2,
        ESyncRenderQueue = 1 << 3
    }

    private static int globalFxID = 0;
    private int _instanceID = -1;
    private short m_LoadStatus = 0;
    private GameObject _gameObject = null;
    private Transform _transformCache = null;
    private Animation _animation = null;
    private AnimationState _animState = null;
    private Animator _animator = null;
    private List<ParticleSystem> _particles = new List<ParticleSystem>();
    private List<Projector> _projectors = new List<Projector>();
    private List<MeshRenderer> _meshs = null;
    private TrailRenderer _trail = null;

    private Transform _parent = null;
    private Vector3 _pos;
    private Quaternion _rot;
    private Vector3 _scale;
    private Vector3 _offset;
    private int _layer = -1;
    private float _speed_ratio = 0.0f;
    private float _startSize = 1;
    private float _startProjectorSize = 1.0f;
    private bool _enable = true;
    private uint _token = 0;
    public float DelayDestroy = 0;
    public int _callback = 0;
    readonly Vector3 Far_Far_Away = new Vector3(0, -1000, 0);

    public bool IsLoaded
    {
        get { return m_LoadStatus == 1; }
    }

    public int instanceID { get { return _instanceID; } }

    public static int GetGlobalFxID()
    {
        globalFxID++;
        if (globalFxID > 1000000)
        {
            globalFxID = 0;
        }
        return globalFxID;
    }


    public void CreateXFx(string location, bool async)
    {
        _instanceID = GetGlobalFxID();
        if (string.IsNullOrEmpty(location) || location.EndsWith("empty"))
        {
            OnLoadFinish(null);
        }
        else
        {
            if (async)
            {
                XResources.LoadAsync<GameObject>(location, AssetType.Prefab, OnLoadFinish);
            }
            else
            {
                GameObject go = XResources.Load<GameObject>(location, AssetType.Prefab);
                OnLoadFinish(go);
            }
        }
    }

    public void DestroyXFx()
    {
        DestroyXFx(true);
    }

    private void DestroyXFx(bool stop)
    {
        if (_instanceID >= 0)
        {
            if (stop) Stop();
            Reset();
        }
        XTimerMgr.singleton.RemoveTimer(_token);
    }

    public void Play(Vector3 position, Quaternion rotation, Vector3 scale, float speed_ratio = 1)
    {
        _parent = null;
        _pos = position;
        _rot = rotation;
        _scale = scale;
        _speed_ratio = speed_ratio;
        ReqPlay();
    }

    public void Play(GameObject parent, Vector3 offset, Vector3 scale, float speed_ratio = 1)
    {
        _parent = parent.transform;
        _pos = Vector3.zero;
        _rot = Quaternion.identity;
        _scale = scale;
        _offset = offset;
        _speed_ratio = speed_ratio;
        ReqPlay();
    }

    private void ReqPlay()
    {
        if (IsLoaded)
        {
            RealPlay();
        }
        else
        {
            _callback |= (int)ECallbackCmd.ESyncPlay;
        }
    }

    private void RealPlay()
    {
        SetTransform();
        if (_enable)
        {
            if (_gameObject != null && !_gameObject.activeSelf)
                _gameObject.SetActive(true);
            if (_animation != null)
            {
                _animation.enabled = true;
                _animation.Play();
                _animState = _animation[_animation.name];
                if (_animState != null)
                {
                    if (_speed_ratio > 0)
                        _animState.speed = 1.0f / _speed_ratio;
                    else
                        _animState.speed = 0;
                }
            }
            if (_animator != null)
            {
                _animator.enabled = true;
                if (_speed_ratio > 0)
                    _animator.speed = 1.0f / _speed_ratio;
                else
                    _animator.speed = 0;
                if (_animator.runtimeAnimatorController != null)
                    _animator.Play(_animator.runtimeAnimatorController.name, 0, 0);
            }
            if (_particles != null)
            {
                _startSize = _scale.x;
                for (int n = 0; n < _particles.Count; n++)
                {
                    ParticleSystem ps = _particles[n];
                    var main = ps.main;
                    if (_speed_ratio > 0)
                        main.simulationSpeed = 1.0f / _speed_ratio;
                    else
                        main.simulationSpeed = 0.0f;
                    if (_startSize > 0)
                    {
                        var startSize = main.startSize;
                        var startSizeZ = main.startSizeZ;
                        startSize.constantMin = _startSize * startSizeZ.constantMin;
                        startSize.constantMax = _startSize * startSizeZ.constantMax;
                    }

                    ps.time = 0;
                    ps.Play(false);
                }
            }
            if (_projectors != null)
            {
                _startProjectorSize = 1.0f;
                float scaleSize = 1.0f;
                if (_scale.z > 0)
                {
                    scaleSize = _scale.x / _scale.z;
                    _startProjectorSize = _scale.z;
                }
                for (int n = 0; n < _projectors.Count; n++)
                {
                    Projector proj = _projectors[n];
                    proj.enabled = true;
                    proj.aspectRatio = scaleSize;
                    proj.orthographicSize *= _startProjectorSize;
                }
            }
            if (_trail != null) _trail.enabled = true;
        }
    }

    private void SetTransform()
    {
        if (_transformCache != null)
        {
            if (_parent == null)
            {
                _transformCache.position = _pos + _offset;
                _transformCache.rotation = _rot;
                _transformCache.localScale = _scale;
            }
            else
            {
                _transformCache.parent = _parent;
                _transformCache.localPosition = Vector3.zero + _parent.rotation * _offset;
                _transformCache.localRotation = Quaternion.identity;
                _transformCache.localScale = _scale;
            }
        }
    }

    public void Stop()
    {
        if (_gameObject != null && _gameObject.transform != null)
        {
            _gameObject.transform.localScale = Vector3.one;
        }
        if (_animation != null)
        {
            _animation.Stop();
            _animation.enabled = false;
        }
        if (_animator != null)
        {
            _animator.speed = 1;
            _animator.enabled = false;
        }
        if (_particles != null)
        {
            for (int i = 0; i < _particles.Count; i++)
            {
                ParticleSystem ps = _particles[i];
                if (ps != null)
                {
                    ps.Stop(false);
                    ps.Clear(false);
                }
            }
        }
        if (_projectors != null)
        {
            for (int n = 0; n < _projectors.Count; n++)
            {
                Projector proj = _projectors[n];
                if (proj != null)
                {
                    proj.enabled = false;
                    if (_startProjectorSize > 0.0f)
                        proj.orthographicSize /= _startProjectorSize;
                }
            }
        }
        if (_trail != null)
        {
            _trail.enabled = false;
        }
        _startSize = 1.0f;
        _startProjectorSize = 1.0f;
    }

    private void OnLoadFinish(Object obj)
    {
        _gameObject = obj as GameObject;
        m_LoadStatus = 1;
        if (_gameObject != null)
        {
            _transformCache = _gameObject.transform;
            _animation = _transformCache.GetComponentInChildren<Animation>();
            _animator = _transformCache.GetComponentInChildren<Animator>();
            if (_animator != null)
            {
                _animator.enabled = true;
            }
            if (_animation != null)
            {
                _animation.enabled = true;
            }
            _particles.Clear();
            _projectors.Clear();

            Component[] tmp = _transformCache.GetComponentsInChildren<Component>(true);
            for (int i = 0, max = tmp.Length; i < max; i++)
            {
                Component com = tmp[i];
                if (com is ParticleSystem)
                {
                    ParticleSystem ps = com as ParticleSystem;
                    if (PreProcessFx(ps.gameObject, XFxMgr.singleton.CameraLayerMask))
                        _particles.Add(ps);
                }
                else if (com is Projector)
                {
                    Projector proj = com as Projector;
                    if (PreProcessFx(proj.gameObject, XFxMgr.singleton.CameraLayerMask))
                        _projectors.Add(proj);
                }
                else if (com is TrailRenderer)
                {
                    _trail = com as TrailRenderer;
                }
            }

            if ((_callback & (int)ECallbackCmd.ESyncPlay) != 0) RealPlay();
            if ((_callback & (int)ECallbackCmd.ESyncLayer) != 0) SyncLayer();
            if ((_callback & (int)ECallbackCmd.ESyncRenderQueue) != 0) RefreshUIRenderQueue();
        }
    }

    private bool PreProcessFx(GameObject go, int qualityLayer)
    {
        int layerOffset = 1 << go.layer;
        if ((layerOffset & qualityLayer) == 0)
        {
            if (go.activeSelf)
                go.SetActive(false);
            return false;
        }
        return true;
    }


    public void Reset()
    {
        _instanceID = -1;
        _transformCache = null;
        _animation = null;
        _animator = null;
        _particles.Clear();
        _projectors.Clear();
        if (_meshs != null)
        {
            _meshs.Clear();
            _meshs = null;
        }
        _trail = null;
        _parent = null;
        _layer = -1;
        _pos = Far_Far_Away;
        _offset = Vector3.zero;
        _rot = Quaternion.identity;
        _scale = Vector3.one;
        _callback = 0;
        m_LoadStatus = 0;
        XResources.Destroy(_gameObject);
    }
    

    public void RefreshUIRenderQueue()
    {
        if (IsLoaded)
        {
            if (_gameObject != null)
            {
                for (int i = 0; i < _particles.Count; ++i)
                {
                    //RefreshRenderQueue(true);   
                }
            }
        }
        else _callback |= (int)ECallbackCmd.ESyncRenderQueue;
    }

    public void SetRenderLayer(int layer)
    {
        if (_layer != layer)
        {
            _layer = layer;
            if (IsLoaded)
            {
                SyncLayer();
            }
            else
            {
                _callback |= (int)ECallbackCmd.ESyncLayer;
            }
        }
    }

    private void SyncLayer()
    {
        if (_layer >= 0)
        {
            for (int n = 0; n < _particles.Count; n++)
            {
                ParticleSystem ps = _particles[n];
                if (ps != null)
                    ps.gameObject.layer = _layer;
            }
            for (int n = 0; n < _projectors.Count; n++)
            {
                Projector proj = _projectors[n];
                proj.gameObject.layer = _layer;
            }
            if (_meshs != null)
            {
                for (int n = 0; n < _meshs.Count; n++)
                {
                    MeshRenderer mesh = _meshs[n];
                    if (mesh != null)
                        mesh.gameObject.layer = _layer;
                }
            }
        }
    }

}
