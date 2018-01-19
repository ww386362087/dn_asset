using XTable;
using UnityEngine;
using System.Collections.Generic;

public class NativeEntity
{
    protected Dictionary<uint, NativeComponent> components;
    protected XEntityPresentation.RowData _present;
    private uint _uid = 0;
    private string _path;
    protected SkinnedMeshRenderer _skin = null;
    protected NativeAnimComponent anim;
    GameObject _go;

    public XEntityPresentation.RowData present
    {
        get { return _present; }
    }

    public uint UID
    {
        get { return _uid; }
    }

    public uint PresentID
    {
        get { return _present != null ? _present.UID : 0; }
    }

    public Transform transfrom
    {
        get { return _go != null ? _go.transform : null; }
    }

    public GameObject EntityObject
    {
        get { return _go; }
    }

    public SkinnedMeshRenderer skin
    {
        get { return _skin; }
        set { _skin = value; }
    }

    protected virtual void OnInitial() { }

    protected virtual void OnUnintial() { }

    protected virtual void InitAnim() { }

    public void Load(uint uid, uint presentid)
    {
        _uid = uid;
        _present = XTableMgr.GetTable<XEntityPresentation>().GetItemID(presentid);
        _path = "Prefabs/" + present.Prefab;
        _go = XResources.LoadInPool(_path);
        _go.name = present.Name;
        components = new Dictionary<uint, NativeComponent>();
        anim = AttachComponent<NativeAnimComponent>();
        OnInitial();
        InitAnim();
    }
    
    public void Unload()
    {
        OnInitial();
        XResources.RecyleInPool(_go, _path);
        _present = null;
        components = null;
    }

    public void Update(float delta)
    {
        var e = components.GetEnumerator();
        while(e.MoveNext())
        {
            e.Current.Value.Update(delta);
        }
    }
    
    public T AttachComponent<T>() where T : NativeComponent, new()
    {
        uint uid = XCommon.singleton.XHash(typeof(T).Name);
        if (components.ContainsKey(uid))
        {
            return components[uid] as T;
        }
        else
        {
            T com = new T();
            com.OnInitial(this);
            components.Add(uid, com);
            return com;
        }
    }

    public bool DetachComponent<T>() where T : NativeComponent, new()
    {
        return DetachComponent(typeof(T).Name);
    }


    public T GetComponent<T>() where T : NativeComponent
    {
        uint uid = XCommon.singleton.XHash(typeof(T).Name);
        if (components != null && components.ContainsKey(uid)) return components[uid] as T;
        return null;
    }

    //for native or lua interface
    public NativeComponent GetComponent(string name)
    {
        uint uid = XCommon.singleton.XHash(name);
        if (components != null && components.ContainsKey(uid)) return components[uid];
        return null;
    }

    //for native or lua interface
    public bool DetachComponent(string name)
    {
        uint uid = XCommon.singleton.XHash(name);
        if (components != null && components.ContainsKey(uid))
        {
            components[uid].OnUninit();
            components[uid] = null;
            components.Remove(uid);
            return true;
        }
        return false;
    }
    
    public void DetachAllComponents()
    {
        if (components != null)
        {
            var e = components.GetEnumerator();
            while (e.MoveNext())
            {
                e.Current.Value.OnUninit();
            }
        }
        components.Clear();
    }

    protected void UpdateComponents(float delta)
    {
        if (components != null)
        {
            var e = components.GetEnumerator();
            while (e.MoveNext())
            {
                e.Current.Value.Update(delta);
            }
        }
    }

    protected void OverrideAnim(string key, string clip)
    {
        if (anim != null)
        {
            string path = present.AnimLocation + clip;
            anim.OverrideAnim(key, path);
        }
    }


}

