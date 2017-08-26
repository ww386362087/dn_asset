using UnityEngine;
using System.Collections.Generic;
using System;
using XTable;

public class XEntityMgr : XSingleton<XEntityMgr>
{

    private Dictionary<uint, XEntity> _dic_entities = new Dictionary<uint, XEntity>();
    private HashSet<XEntity> _hash_entitys = new HashSet<XEntity>();
    
    public XPlayer Player;

    private XEntity CreateEntity(XAttributes attr, bool autoAdd)
    {
        XEntity e = null;
        switch (attr.Type)
        {
            case EnitityType.Entity_Boss: e = PrepareEntity<XBoss>(attr, autoAdd); break;
            case EnitityType.Entity_Role: e = PrepareEntity<XRole>(attr, autoAdd); break;
            case EnitityType.Entity_Player: e = PrepareEntity<XPlayer>(attr, autoAdd); break;
            case EnitityType.Entity_Npc: e = PrepareEntity<XNPC>(attr, autoAdd); break;
            case EnitityType.Entity_Enemy: e = PrepareEntity<XEnemy>(attr, autoAdd); break;
        }
        return e;
    }


    private T PrepareEntity<T>(XAttributes attr, bool autoAdd) where T : XEntity
    {
        T x = Activator.CreateInstance<T>();
        GameObject o = XResourceMgr.Load<GameObject>("Prefabs/" + attr.Prefab, AssetType.Prefab);
        if (!Application.isMobilePlatform)
            o.name = attr.Name;
        o.transform.position = attr.AppearPostion;
        o.transform.rotation = attr.AppearQuaternion;
        x.Initilize(o, attr);
        if (!_dic_entities.ContainsKey(attr.id)) _dic_entities.Add(attr.id, x);
        if (!_hash_entitys.Add(x)) Debug.Log("has exist entity: " + attr.id);
        return x;
    }


    private void UnloadAll()
    {
        var e = _hash_entitys.GetEnumerator();
        while (e.MoveNext())
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
        while (e.MoveNext())
        {
            e.Current.OnUpdate(delta);
        }
    }


    public void LateUpdate()
    {
        var e = _hash_entitys.GetEnumerator();
        while (e.MoveNext())
        {
            e.Current.OnLateUpdate();
        }
    }



    /// <summary>
    /// 登录进入
    /// </summary>
    public void AttachToHost()
    {
        var e = _hash_entitys.GetEnumerator();
        while (e.MoveNext())
        {
            e.Current.OnAttachToHost();
        }
    }

    /// <summary>
    /// 退出登录
    /// </summary>
    public void DetachFromHost()
    {
        var e = _hash_entitys.GetEnumerator();
        while (e.MoveNext())
        {
            e.Current.OnDeatchToHost();
        }
    }

    public XEntity GetEntity(uint id)
    {
        if (_dic_entities.ContainsKey(id))
            return _dic_entities[id];
        return null;
    }


    public XEntity CreateEntity(uint id,Vector3 pos,Quaternion rot,bool autoAdd)
    {
        XAttributes attr = InitAttrFromClient((int)id);
        attr.AppearPostion = pos;
        attr.AppearQuaternion = rot;
        XEntity e = PrepareEntity<XEntity>(attr, autoAdd);
        return e;
    }


    public XRole CreateRole(XAttributes attr, bool autoAdd)
    {
        return PrepareEntity<XRole>(attr, autoAdd);
    }


    public XRole CreateTestRole()
    {
        XAttributes attr = InitAttrFromClient(2);
        attr.Type = EnitityType.Entity_Role;
        attr.AppearPostion = Vector3.zero;
        attr.AppearQuaternion = Quaternion.identity;
        return CreateRole(attr, false);
    }

    public XPlayer CreatePlayer()
    {
        SceneList.RowData row = XScene.singleton.SceneRow;
        XAttributes attr = InitAttrFromClient(2);
        attr.Type = EnitityType.Entity_Player;
        string s = row.StartPos;
        string[] ss = s.Split('=');
        float[] fp = new float[3];
        float.TryParse(ss[0], out fp[0]);
        float.TryParse(ss[1], out fp[1]);
        float.TryParse(ss[2], out fp[2]);
        attr.AppearPostion = new Vector3(fp[0], fp[1], fp[2]);
        attr.AppearQuaternion = Quaternion.Euler(row.StartRot[0], row.StartRot[1], row.StartRot[2]);
        return Player = PrepareEntity<XPlayer>(attr, false);
    }


    public XNPC CreateNPC(XNpcList.RowData row)
    {
        XAttributes attr = InitAttrByPresent(row.PresentID);
        attr.Type = EnitityType.Entity_Npc;
        attr.AppearPostion = Player.Position + new Vector3(0, 0, -2);// new Vector3(row.NPCPosition[0], row.NPCPosition[1], row.NPCPosition[2]);
        attr.AppearQuaternion = Quaternion.Euler(row.NPCRotation[0], row.NPCRotation[1], row.NPCRotation[2]);
        return PrepareEntity<XNPC>(attr, false);
    }


    private XAttributes InitAttrFromClient(int entityID)
    {
        var entity = XEntityStatistics.sington.GetByID(entityID);
        if (entity == null) throw new Exception("entity is nil with id: " + entityID);
        XAttributes attr = new XAttributes();
        var prow = XEntityPresentation.sington.GetItemID(entity.PresentID);
        if (prow == null) throw new Exception("present is nil with id: " + entity.PresentID);
        attr.Prefab = prow.Prefab;
        attr.id = (uint)XCommon.singleton.New_id;
        attr.PresentID = entity.PresentID;
        attr.Name = prow.Name;
        return attr;
    }

    private XAttributes InitAttrByPresent(uint presentID)
    {
        XAttributes attr = new XAttributes();
        var prow = XEntityPresentation.sington.GetItemID(presentID);
        if (prow == null) throw new Exception("present is nil with id: " + presentID);
        attr.Prefab = prow.Prefab;
        attr.id = (uint)XCommon.singleton.New_id;
        attr.PresentID = presentID;
        attr.Name = prow.Name;
        return attr;
    }

}
