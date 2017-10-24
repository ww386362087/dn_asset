using UnityEngine;
using XTable;

namespace Level
{

    internal class XLevelBaseTask
    {
        public int _id;
        protected XLevelSpawnInfo _spawner;

        public XLevelBaseTask(XLevelSpawnInfo spawn)
        {
            _spawner = spawn;
        }

        public virtual bool Execute(float time)
        {
            return true;
        }
    }


    internal class XLevelSpawnTask : XLevelBaseTask
    {
        public uint UID;
        public int rot;
        public Vector3 pos;
        public LevelSpawnType spawnType;
        public bool isSummonTask;

        public XLevelSpawnTask(XLevelSpawnInfo spawn) : base(spawn)
        {
            isSummonTask = false;
        }

        public XEntity CreateMonster(uint id, float yRotate, Vector3 pos, int _waveid)
        {
            Quaternion rotation = Quaternion.Euler(0, yRotate, 0);
            XEntity entity = XEntityMgr.singleton.CreateEntity<XMonster>(id, pos, rotation);
            if (entity != null)
            {
                entity.Wave = _waveid;
                entity.CreateTime = Time.realtimeSinceStartup;
                entity.SetRelation(EntityType.Enemy);
                return entity;
            }
            return null;
        }


        public XEntity CreateNPC(uint id, float yRotate, Vector3 pos, int _waveid)
        {
            Quaternion rotation = Quaternion.Euler(0, yRotate, 0);
            XNpcList.RowData row = XTableMgr.GetTable<XNpcList>().GetByUID((int)id);
            XEntity entity = XEntityMgr.singleton.CreateNPC(row, pos, rotation);
            if (entity != null)
            {
                entity.Wave = _waveid;
                entity.CreateTime = Time.realtimeSinceStartup;
                entity.SetRelation(EntityType.Neutral);
                return entity;
            }
            return null;
        }

        public override bool Execute(float time)
        {
            base.Execute(time);
            XLevelDynamicInfo dInfo = null;
            if (!isSummonTask)
            {
                dInfo = _spawner.GetWaveDynamicInfo(_id);
                if (dInfo == null) return true;
            }
            XEntity entity = null;
            if (spawnType == LevelSpawnType.Spawn_Monster)
            {
                // 从本地创建
                entity = CreateMonster(UID, rot, pos + new Vector3(0, 0.02f, 0), _id);
                XLevelStatistics.singleton.ls.AddLevelSpawnEntityCount(entity.EntityID);
            }
            else if (spawnType == LevelSpawnType.Spawn_Buff)
            {
                // 单机现在不处理直接掉doodad
            }
            else if(spawnType == LevelSpawnType.Spawn_NPC)
            {
                entity = CreateNPC(UID, rot, pos + new Vector3(0, 0.02f, 0), _id);
            }
            else //player or monster
            {
                // 属性和外形来自服务器
            }

            if (dInfo != null)
            {
                if (entity != null)
                {
                    dInfo.generateCount++;
                    dInfo.entityIds.Add(entity.EntityID);
                }
                if (dInfo.generateCount == dInfo.totalCount)
                {
                    dInfo.generatetime = time;
                }
                if (entity != null && entity.IsBoss)
                {
                    return false;
                }
            }
            return true;
        }
    }

    internal class XLevelScriptTask : XLevelBaseTask
    {
        public string _ScriptName;

        public XLevelScriptTask(XLevelSpawnInfo ls) : base(ls)
        {
        }

        public override bool Execute(float time)
        {
            base.Execute(time);
            XLevelScriptMgr.singleton.RunScript(_ScriptName);
            return false;
        }

    }

}