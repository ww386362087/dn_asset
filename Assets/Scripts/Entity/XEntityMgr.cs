using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class XEntityMgr : XSingleton<XEntityMgr>
{

    private Dictionary<uint, XEntity> _entities = new Dictionary<uint, XEntity>();
    
    private XEntity CreateEntity(XAttributes attr, bool autoAdd)
    {
        XEntity e = null;
        switch (attr.Type)
        {
            case EnitityType.Entity_Boss: e = PrepareEntity<XBoss>(attr, autoAdd); break;
            case EnitityType.Entity_Role: e = PrepareEntity<XRole>(attr,  autoAdd); break;
            case EnitityType.Entity_Player: e = PrepareEntity<XPlayer>(attr, autoAdd); break;
            case EnitityType.Entity_Npc: e = PrepareEntity<XNPC>(attr,  autoAdd); break;
            case EnitityType.Entity_Enemy: e = PrepareEntity<XEnemy>(attr,  autoAdd); break;
        }
        return e;
    }


    private T PrepareEntity<T>(XAttributes attr, bool autoAdd) where T : XEntity
    {
        T x = Activator.CreateInstance<T>();
        UnityEngine.Object obj = XResourceMgr.Load<GameObject>("Prefabs/" + attr.Prefab);
        GameObject o = UnityEngine.Object.Instantiate(obj) as GameObject;
        o.name = attr.Name;
        o.transform.position = attr.AppearPostion;
        o.transform.rotation = attr.AppearQuaternion;
        x.Initilize(o, attr);
        if (!_entities.ContainsKey(attr.ID)) _entities.Add(attr.ID, x);
        return x;
    }


    public void UnloadEntity<T>(uint id)
    {
        if (_entities.ContainsKey(id))
        {
            _entities[id].Unload();
        }
    }

 

    public XRole CreateRole(XAttributes attr, bool autoAdd)
    {
        XRole e = PrepareEntity<XRole>(attr, autoAdd);
        return e;
    }


    public XRole CreateTestRole()
    {
        XAttributes attr = new XAttributes();
        attr.Prefab = "Player";
        attr.Name = "Archer";
        attr.Type = EnitityType.Entity_Role;
        attr.AppearPostion = Vector3.zero;
        attr.AppearQuaternion = Quaternion.identity;
        return CreateRole(attr, false);
    }

}
