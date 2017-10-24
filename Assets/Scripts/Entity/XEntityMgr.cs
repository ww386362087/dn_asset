using UnityEngine;
using System.Collections.Generic;
using System;
using XTable;

public class XEntityMgr : XSingleton<XEntityMgr>
{
    private Dictionary<uint, XEntity> _dic_entities = new Dictionary<uint, XEntity>();
    private HashSet<XEntity> _hash_entitys = new HashSet<XEntity>();
    private Dictionary<EnitityType, List<XEntity>> _map_entities = new Dictionary<EnitityType, List<XEntity>>();
    public XPlayer Player;
    
    private T PrepareEntity<T>(XAttributes attr) where T : XEntity
    {
        T x = Activator.CreateInstance<T>();
        GameObject o = XResources.LoadInPool("Prefabs/" + attr.Prefab);
        o.name = attr.id.ToString();
        o.transform.position = attr.AppearPostion;
        o.transform.rotation = attr.AppearQuaternion;
        x.Initilize(o, attr);
        if (!_dic_entities.ContainsKey(attr.id) && IsPlayer(x)) _dic_entities.Add(attr.id, x);
        if (!_hash_entitys.Add(x) && IsPlayer(Player)) XDebug.Log("has exist entity: ", attr.id);
        return x;
    }


    public void UnloadAll()
    {
        var e = _hash_entitys.GetEnumerator();
        while (e.MoveNext())
        {
            e.Current.UnloadEntity();
        }
        _hash_entitys.Clear();
        _dic_entities.Clear();
        _map_entities.Clear();
    }


    public void UnloadEntity<T>(uint id)
    {
        if (_dic_entities.ContainsKey(id))
        {
            _dic_entities[id].UnloadEntity();
            _dic_entities.Remove(id);
        }
        _hash_entitys.RemoveWhere(x => x.EntityID == id);
        var e = _map_entities.GetEnumerator();
        while (e.MoveNext())
        {
            e.Current.Value.RemoveAll(ent => ent.EntityID == id);
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
        XAttachEventArgs e = new XAttachEventArgs();
        XEventMgr.singleton.FireEvent(e);
    }

    /// <summary>
    /// 退出登录
    /// </summary>
    public void DetachFromHost()
    {
        XDetachEventArgs e = new XDetachEventArgs();
        XEventMgr.singleton.FireEvent(e);
    }

    public XEntity GetEntity(uint id)
    {
        if (_dic_entities.ContainsKey(id))
            return _dic_entities[id];
        return null;
    }


    public XEntity CreateEntity<T>(uint staticid, Vector3 pos, Quaternion rot) where T : XEntity
    {
        XAttributes attr = InitAttrFromClient((int)staticid);
        attr.AppearPostion = pos;
        attr.AppearQuaternion = rot;
        XEntity e = PrepareEntity<T>(attr);
        Add(EnitityType.Entity, e);
        return e;
    }


    public XRole CreateRole(XAttributes attr)
    {
        return PrepareEntity<XRole>(attr);
    }

    public XRole CreateTestRole()
    {
        XAttributes attr = InitAttrFromClient(2);
        attr.AppearPostion = Vector3.zero;
        attr.AppearQuaternion = Quaternion.identity;
        return CreateRole(attr);
    }

    public XPlayer CreatePlayer()
    {
        SceneList.RowData row = XScene.singleton.SceneRow;
        XAttributes attr = InitAttrFromClient(2);
        string s = row.StartPos;
        string[] ss = s.Split('=');
        float[] fp = new float[3];
        float.TryParse(ss[0], out fp[0]);
        float.TryParse(ss[1], out fp[1]);
        float.TryParse(ss[2], out fp[2]);
        attr.AppearPostion = new Vector3(fp[0], fp[1], fp[2]);
        attr.AppearQuaternion = Quaternion.Euler(row.StartRot[0], row.StartRot[1], row.StartRot[2]);
        return Player = PrepareEntity<XPlayer>(attr);
    }
    
    public XNPC CreateNPC(XNpcList.RowData row)
    {
        var pos = new Vector3(row.NPCPosition[0], row.NPCPosition[1], row.NPCPosition[2]);
        var rot = Quaternion.Euler(row.NPCRotation[0], row.NPCRotation[1], row.NPCRotation[2]);
        return CreateNPC(row, pos, rot);
    }

    public XNPC CreateNPC(XNpcList.RowData row,Vector3 pos, Quaternion rot)
    {
        XAttributes attr = InitAttrByPresent(row.PresentID);
        attr.AppearPostion = pos;
        attr.AppearQuaternion = rot;
        var e = PrepareEntity<XNPC>(attr);
        Add(EnitityType.Npc, e);
        return e;
    }
    
    public List<XEntity> GetAllNPC()
    {
        return _map_entities[EnitityType.Npc];
    }
    
    public List<XEntity> GetAllAlly()
    {
        return _map_entities[EnitityType.Ally];
    }

    public List<XEntity> GetAllEnemy()
    {
        return _map_entities[EnitityType.Enemy];
    }

    public void SetRelation(uint entityid, EnitityType type)
    {
        if (type < EnitityType.Ship_Start)
        {
            XDebug.LogError("Set Relation err");
            return;
        }
        XEntity e = null;
        if (_dic_entities.TryGetValue(entityid, out e))
        {
            e.SetRelation(type);
            Add(type, e);
        }
    }

    private bool IsPlayer(XEntity e)
    {
        if (Player != null)
            return Player.EntityID == e.EntityID;
        return false;
    }

    private XAttributes InitAttrFromClient(int staticid)
    {
        var entity = XTableMgr.GetTable<XEntityStatistics>().GetByID(staticid);
        if (entity == null) throw new Exception("entity is nil with id: " + staticid);
        XAttributes attr = new XAttributes();
        var prow = XTableMgr.GetTable<XEntityPresentation>().GetItemID(entity.PresentID);
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
        var prow = XTableMgr.GetTable<XEntityPresentation>().GetItemID(presentID);
        if (prow == null) throw new Exception("present is nil with id: " + presentID);
        attr.Prefab = prow.Prefab;
        attr.id = (uint)XCommon.singleton.New_id;
        attr.PresentID = presentID;
        attr.Name = prow.Name;
        return attr;
    }

    private bool Add(EnitityType type, XEntity e)
    {
        if (_map_entities.ContainsKey(type))
        {
            List<XEntity> l = _map_entities[type];
            if (l.Contains(e)) return false;
            else l.Add(e);
        }
        else
        {
            List<XEntity> l = new List<XEntity>();
            l.Add(e);
            _map_entities.Add(type, l);
        }
        return true;
    }

}
