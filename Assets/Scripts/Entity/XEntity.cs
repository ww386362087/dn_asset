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
    protected Vector3 _movement = Vector3.zero;
    public float speed = 0.02f;

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

   

    public void Initilize(GameObject o, XAttributes attr)
    {
        base.Initilize();
        _object = o;
        _attr = attr;
        OnInitial();
    }

    public void UnloadEntity()
    {
        _attr = null;
        GameObject.Destroy(_object);
        _object = null;
        OnUnintial();
        base.Unload();
    }

    public virtual void OnInitial() { }

    protected virtual void OnUnintial() { }

    public virtual void Update(float delta)
    {
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


    public void ApplyMove()
    {
       
    }

    public void ApplyMove(float x,float y,float z)
    {
        _movement.x = x;
        _movement.y = y;
        _movement.z = z;
    }
    

}
