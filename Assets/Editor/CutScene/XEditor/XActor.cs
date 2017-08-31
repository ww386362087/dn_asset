using UnityEditor;
using UnityEngine;

public class XActor
{
    private GameObject _actor = null;
    private Transform _shadow = null;
    private Animator _ator = null;

    public GameObject Actor { get { return _actor; } }
    
    public XActor(float x, float y, float z, string clip)
    {
        _actor = AssetDatabase.LoadAssetAtPath("Assets/Editor/EditorResources/Prefabs/ZJ_zhanshi.prefab", typeof(GameObject)) as GameObject;
        _actor = Object.Instantiate(_actor) as GameObject;
        DisablePhysic();
        _actor.transform.position = new Vector3(x, y, z);
        _ator = _actor.GetComponent<Animator>();
        AnimatorOverrideController overrideController = new AnimatorOverrideController();
        overrideController.runtimeAnimatorController = _ator.runtimeAnimatorController;
        _ator.runtimeAnimatorController = overrideController;
        overrideController["Idle"] = XResourceMgr.Load<AnimationClip>(clip, AssetType.Anim);
        _shadow = _actor.transform.Find("Shadow");
        if (_shadow != null) _shadow.GetComponent<Renderer>().enabled = true;
        _ator.cullingMode = AnimatorCullingMode.AlwaysAnimate;
    }

    public XActor(string prefab, float x, float y, float z, string clip)
    {
        _actor = XResourceMgr.Load<GameObject>(prefab, AssetType.Prefab);
        _actor.transform.position = new Vector3(x, y, z);
        _actor.transform.rotation = Quaternion.identity;
        _ator = _actor.GetComponent<Animator>();
        DisablePhysic();
        if (_ator != null)
        {
            AnimatorOverrideController overrideController = new AnimatorOverrideController();
            overrideController.runtimeAnimatorController = _ator.runtimeAnimatorController;
            _ator.runtimeAnimatorController = overrideController;
            overrideController["Idle"] = XResourceMgr.Load<AnimationClip>(clip, AssetType.Anim);
            _shadow = _actor.transform.Find("Shadow");
            if (_shadow != null) _shadow.GetComponent<Renderer>().enabled = true;
            _ator.cullingMode = AnimatorCullingMode.AlwaysAnimate;
        }
    }

    public XActor(uint id, float x, float y, float z, string clip)
    {
        _actor = Object.Instantiate(XEditorLibrary.GetDummy(id), new Vector3(x, y, z), Quaternion.identity) as GameObject;
        _ator = _actor.GetComponent<Animator>();
        DisablePhysic();
        AnimatorOverrideController overrideController = new AnimatorOverrideController();
        overrideController.runtimeAnimatorController = _ator.runtimeAnimatorController;
        _ator.runtimeAnimatorController = overrideController;
        overrideController["Idle"] = XResourceMgr.Load<AnimationClip>(clip, AssetType.Anim);
        _shadow = _actor.transform.Find("Shadow");
        if (_shadow != null) _shadow.GetComponent<Renderer>().enabled = true;
        _ator.cullingMode = AnimatorCullingMode.AlwaysAnimate;
    }

    private void DisablePhysic()
    {
        if (_actor != null)
        {
            CharacterController cc = _actor.GetComponent<CharacterController>();
            if (cc != null) cc.enabled = false;
        }
    }
    
}
