using UnityEngine;
using System.Collections.Generic;
using System;
using XTable;

public class XEntityMgr : XSingleton<XEntityMgr>
{
    private List<XEntity> _empty = new List<XEntity>();
    private Dictionary<uint, XEntity> _dic_entities = new Dictionary<uint, XEntity>();
    private HashSet<XEntity> _hash_entitys = new HashSet<XEntity>();
    private Dictionary<EntityType, List<XEntity>> _map_entities = new Dictionary<EntityType, List<XEntity>>();
    public XPlayer Player;

    private T PrepareEntity<T>(XAttributes attr) where T : XEntity
    {
        T x = Activator.CreateInstance<T>();
        GameObject o = XResources.LoadInPool("Prefabs/" + attr.Prefab);
        o.name = attr.id.ToString();
        o.transform.position = attr.AppearPostion;
        o.transform.rotation = attr.AppearQuaternion;
        x.Initilize(o, attr);
        if (!_dic_entities.ContainsKey(attr.id) && !IsPlayer(x)) _dic_entities.Add(attr.id, x);
        if (!_hash_entitys.Add(x) && !IsPlayer(Player)) XDebug.Log("has exist entity: ", attr.id);
        SetRelation(attr.id, attr.FightGroup);
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
        Add(EntityType.Entity, e);
        return e;
    }

    public XRole CreateTestRole()
    {
        XAttributes attr = InitAttrFromClient(2);
        attr.AppearPostion = Vector3.zero;
        attr.AppearQuaternion = Quaternion.identity;
        return PrepareEntity<XRole>(attr);
    }

    public XPlayer CreatePlayer()
    {
        SceneList.RowData row = XScene.singleton.SceneRow;
        int statisticid = 2;
        XAttributes attr = InitAttrFromClient(statisticid);
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

    public XNPC CreateNPC(XNpcList.RowData row, Vector3 pos, Quaternion rot)
    {
        XAttributes attr = InitAttrByPresent(row.PresentID);
        attr.AppearPostion = pos;
        attr.AppearQuaternion = rot;
        var e = PrepareEntity<XNPC>(attr);
        Add(EntityType.Npc, e);
        return e;
    }

    public List<XEntity> GetAllNPC()
    {
        return _map_entities[EntityType.Npc];
    }

    public List<XEntity> GetAllAlly()
    {
        return _map_entities[EntityType.Ally];
    }

    public List<XEntity> GetAllAlly(XEntity e)
    {
        EntityType type = (EntityType)e.Attributes.FightGroup;
        if (type == EntityType.Ally) return _map_entities[EntityType.Ally];
        else if (type == EntityType.Enemy) return _map_entities[EntityType.Enemy];
        return _empty;
    }

    public List<XEntity> GetAllEnemy()
    {
        return _map_entities[EntityType.Enemy];
    }

    public List<XEntity> GetAllEnemy(XEntity e)
    {
        EntityType type = (EntityType)e.Attributes.FightGroup;
        if (type == EntityType.Ally) return _map_entities[EntityType.Enemy];
        else if (type == EntityType.Enemy) return _map_entities[EntityType.Ally];
        return _empty;
    }

    public void SetRelation(uint entityid, int group)
    {
        int max = EntityType.Ship_End - EntityType.Ship_Start;
        if (group < 0 || group > max)
        {
            XDebug.LogError("Set Relation err ", entityid);
            return;
        }
        EntityType type = group + EntityType.Ship_Start;
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

    private XAttributes InitAttrFromClient(int statisticid)
    {
        var srow = XTableMgr.GetTable<XEntityStatistics>().GetByID(statisticid);
        if (srow == null) throw new Exception("entity is nil with id: " + statisticid);
        XAttributes attr = new XAttributes();
        var prow = XTableMgr.GetTable<XEntityPresentation>().GetItemID(srow.PresentID);
        if (prow == null) throw new Exception("present is nil with id: " + srow.PresentID);
        attr.Prefab = prow.Prefab;
        attr.id = (uint)XCommon.singleton.New_id;
        attr.PresentID = srow.PresentID;
        attr.Name = prow.Name;
        attr.InitAttribute(srow);
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

    private bool Add(EntityType type, XEntity e)
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
