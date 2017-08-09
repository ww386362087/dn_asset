using UnityEngine;
using System.Collections.Generic;


public enum EnitityType
{
    Entity_None = 1 << 0,
    Entity_Role = 1 << 1,
    Entity_Player = 1 << 2,
    Entity_Enemy = 1 << 3,
    Entity_Opposer = 1 << 4,
    Entity_Boss = 1 << 5,
    Entity_Puppet = 1 << 6,
    Entity_Elite = 1 << 7,
    Entity_Npc = 1 << 8,
}


public abstract class XEntity : XObject
{

    protected abstract EnitityType _eEntity_Type { get; }
    protected XAttributes _attr = null;
    protected GameObject _object = null;
    protected int _layer = 0;
    public uint EntityID
    {
        get { return _attr != null ? _attr.id : 0; }
    }

    public bool IsPlayer
    {
        get { return (_eEntity_Type & EnitityType.Entity_Player) != 0; }
    }

    public bool IsRole
    {
        get { return (_eEntity_Type & EnitityType.Entity_Role) != 0; }
    }

    public bool IsOpposer
    {
        get { return (_eEntity_Type & EnitityType.Entity_Opposer) != 0; }
    }

    public bool IsEnemy
    {
        get { return (_eEntity_Type & EnitityType.Entity_Enemy) != 0; }
    }

    public bool IsPuppet
    {
        get { return (_eEntity_Type & EnitityType.Entity_Puppet) != 0; }
    }

    public bool IsBoss
    {
        get { return (_eEntity_Type & EnitityType.Entity_Boss) != 0; }
    }

    public bool IsElite
    {
        get { return (_eEntity_Type & EnitityType.Entity_Elite) != 0; }
    }

    public bool IsNpc
    {
        get { return (_eEntity_Type & EnitityType.Entity_Npc) != 0; }
    }

    public GameObject EntityObject
    {
        get { return _object; }
    }

    public XAttributes EntityAttribute
    {
        get { return _attr; }
    }

    public int DefaultLayer
    {
        get { return _layer; }
    }

    public virtual bool HasAI
    {
        get { return GetComponent<XAIComponent>() == null; }
    }

    protected Dictionary<uint, XComponent> components;

    public void Initilize(GameObject o, XAttributes attr)
    {
        _object = o;
        _attr = attr;
        components = new Dictionary<uint, XComponent>();
        OnInitial();
        base.Initilize();
    }

    public void UnloadEntity()
    {
        DetachAllComponents();
        _attr = null;
        GameObject.Destroy(_object);
        _object = null;
        base.Unload();
        OnUnintial();
    }

    public virtual void OnInitial() { }

    protected virtual void OnUnintial() { }

    public virtual void Update(float delta) { }

    private void CheckCondtion()
    {
        if (components == null)
            throw new XComponentException("entity components is nil");
    }

    public T AttachComponent<T>() where T : XComponent, new()
    {
        CheckCondtion();
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

    public bool DetachComponent<T>() where T : XComponent, new()
    {
       return DetachComponent(typeof(T).Name);
    }


    public T GetComponent<T>() where T : XComponent
    {
        uint uid = XCommon.singleton.XHash(typeof(T).Name);
        if (components != null && components.ContainsKey(uid)) return components[uid] as T;
        return null;
    }

    //lua interface
    public object GetComponent(string name)
    {
        uint uid = XCommon.singleton.XHash(name);
        if (components != null && components.ContainsKey(uid)) return components[uid];
        return null;
    }

    //lua interface
    public bool DetachComponent(string name)
    {
        uint uid = XCommon.singleton.XHash(name);
        if (components != null && components.ContainsKey(uid))
        {
            components[uid].OnUninit();
            components.Remove(uid);
            return true;
        }
        return false;
    }


    private void DetachAllComponents()
    {
        if (components != null)
        {
            var e = components.GetEnumerator();
            while(e.MoveNext())
            {
                e.Current.Value.OnUninit();
            }
        }
        components.Clear();
    }


    public void AttachHost()
    {
        OnAttachToHost();
    }

    public void DeatchHost()
    {
        OnDeatchToHost();
    }


    protected virtual void OnAttachToHost()
    {
    }


    protected virtual void OnDeatchToHost()
    {
    }
}
