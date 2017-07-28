using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class XEntityMgr : XSingleton<XEntityMgr>
{

    private Dictionary<uint, XEntity> _dic_entities = new Dictionary<uint, XEntity>();
    private HashSet<XEntity> _hash_entitys = new HashSet<XEntity>();

    public XPlayer player;
    
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
        UnityEngine.Object obj = XResourceMgr.Load<GameObject>("Prefabs/" + attr.Prefab,AssetType.Prefab);
        GameObject o = UnityEngine.Object.Instantiate(obj) as GameObject;
        o.name = attr.Name;
        o.transform.position = attr.AppearPostion;
        o.transform.rotation = attr.AppearQuaternion;
        x.Initilize(o, attr);
        if (!_dic_entities.ContainsKey(attr.id)) _dic_entities.Add(attr.id, x);
        if (_hash_entitys.Add(x)) Debug.Log("has exist entity: " + attr.id);
        return x;
    }


    private void UnloadAll()
    {
        var e = _hash_entitys.GetEnumerator();
        while(e.MoveNext())
        {
            e.Current.UnloadEntity();
        }
        _hash_entitys.Clear();
        _dic_entities.Clear();
    }


    public void UnloadEntity<T>(uint id)
    {
        if (_dic_entities.ContainsKey(id))
        {
            _dic_entities[id].UnloadEntity();
        }
    }

    public void Update(float delta)
    {
        var e = _hash_entitys.GetEnumerator();
        while(e.MoveNext())
        {
            e.Current.Update(delta);
        }
    }
    

    public XRole CreateRole(XAttributes attr, bool autoAdd)
    {
        return PrepareEntity<XRole>(attr, autoAdd);
    }


    public XRole CreateTestRole()
    {
        XAttributes attr = new XAttributes();
        attr.id = 10001;
        attr.Prefab = "Player";
        attr.Name = "Archer";
        attr.Type = EnitityType.Entity_Role;
        attr.AppearPostion = Vector3.zero;
        attr.AppearQuaternion = Quaternion.identity;
        return CreateRole(attr, false);
    }

    public void CreatePlayer()
    {
        XAttributes attr = new XAttributes();
        attr.id = 10001;
        attr.Prefab = "Player";
        attr.Name = "Archer";
        attr.Type = EnitityType.Entity_Role;
        attr.AppearPostion = Vector3.zero;
        attr.AppearQuaternion = Quaternion.identity;
        player = PrepareEntity<XPlayer>(attr, false);
    }


}
