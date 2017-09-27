using UnityEngine;
using XTable;

public class XActor
{
    private GameObject _go = null;
    private Transform _shadow = null;
    private Animator _ator = null;

    public GameObject Actor { get { return _go; } }

    public XActor(float x, float y, float z, string clip)
    {
        string path = @"Prefabs/Player";
        _go = XResourceMgr.Load<GameObject>(path, AssetType.Prefab);
        _go.transform.position = new Vector3(x, y, z);
        _ator = _go.GetComponent<Animator>();
        if (_ator != null) MakeActor(clip);
    }

    public XActor(string prefab, float x, float y, float z, string clip)
    {
        _go = XResourceMgr.Load<GameObject>(prefab, AssetType.Prefab);
        _go.transform.position = new Vector3(x, y, z);
        _go.transform.rotation = Quaternion.identity;
        _ator = _go.GetComponent<Animator>();
        if (_ator != null) MakeActor(clip);
    }

    public XActor(uint id, float x, float y, float z, string clip)
    {
        _go = Object.Instantiate(GetDummy(id), new Vector3(x, y, z), Quaternion.identity) as GameObject;
        _ator = _go.GetComponent<Animator>();
        if (_ator != null) MakeActor(clip);
    }

    public GameObject GetDummy(uint statictid)
    {
        XEntityStatistics.RowData row = XTableMgr.GetTable<XEntityStatistics>().GetByID((int)statictid);
        if (row != null)
        {
            XEntityPresentation.RowData raw_data = XTableMgr.GetTable<XEntityPresentation>().GetItemID(row.PresentID);
            if (raw_data == null) return null;
            string prefab = raw_data.Prefab;
            return XResourceMgr.Load<GameObject>("Prefabs/" + prefab, AssetType.Prefab);
        }
        return null;
    }
    
    private void MakeActor(string clip)
    {
        DisablePhysic();
        AnimatorOverrideController overrideController = new AnimatorOverrideController();
        overrideController.runtimeAnimatorController = _ator.runtimeAnimatorController;
        _ator.runtimeAnimatorController = overrideController;
        overrideController["Idle"] = XResourceMgr.Load<AnimationClip>(clip, AssetType.Anim);
        _shadow = _go.transform.Find("Shadow");
        if (_shadow != null) _shadow.GetComponent<Renderer>().enabled = true;
        _ator.cullingMode = AnimatorCullingMode.AlwaysAnimate;

    }

    private void DisablePhysic()
    {
        if (_go != null)
        {
            CharacterController cc = _go.GetComponent<CharacterController>();
            if (cc != null) cc.enabled = false;
        }
    }
    
}
