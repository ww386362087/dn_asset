using UnityEngine;


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
        public uint _EnemyID;
        public int _MonsterRotate;
        public int _MonsterIndex;
        public Vector3 _MonsterPos;
        public LevelSpawnType _SpawnType;
        public bool _IsSummonTask;

        public XLevelSpawnTask(XLevelSpawnInfo spawn) : base(spawn)
        {
            _IsSummonTask = false;
        }

        public XEntity CreateMonster(uint id, float yRotate, Vector3 pos, int _waveid)
        {
            Quaternion rotation = Quaternion.Euler(0, yRotate, 0);
            XEntity entity = XEntityMgr.singleton.CreateEntity<XMonster>(id, pos, rotation);
            if (entity != null)
            {
                entity.Wave = _waveid;
                entity.CreateTime = Time.realtimeSinceStartup;
                entity.SetRelation(EnitityType.Enemy);
                return entity;
            }
            return null;
        }


        public XEntity CreateNPC(uint id, float yRotate, Vector3 pos, int _waveid)
        {
            Quaternion rotation = Quaternion.Euler(0, yRotate, 0);
            XEntity entity = XEntityMgr.singleton.CreateEntity<XNPC>(id, pos, rotation);
            if (entity != null)
            {
                entity.Wave = _waveid;
                entity.CreateTime = Time.realtimeSinceStartup;
                entity.SetRelation(EnitityType.Neutral);
                return entity;
            }
            return null;
        }

        public override bool Execute(float time)
        {
            base.Execute(time);
            XLevelDynamicInfo dInfo = null;
            if (!_IsSummonTask)
            {
                dInfo = _spawner.GetWaveDynamicInfo(_id);
                if (dInfo == null) return true;
            }
            XEntity enemy = null;
            if (_SpawnType == LevelSpawnType.Spawn_Monster)
            {
                // 从本地创建
                enemy = CreateMonster(_EnemyID, _MonsterRotate, _MonsterPos + new Vector3(0, 0.02f, 0), _id);
                XLevelStatistics.singleton.ls.AddLevelSpawnEntityCount(enemy.EntityID);
            }
            else if (_SpawnType == LevelSpawnType.Spawn_Buff)
            {
                // 单机现在不处理直接掉doodad
            }
            else //player or monster
            {
                // 属性和外形来自服务器
            }

            if (dInfo != null)
            {
                if (enemy != null)
                {
                    dInfo.generateCount++;
                    dInfo.entityIds.Add(enemy.EntityID);
                }
                if (dInfo.generateCount == dInfo.totalCount)
                {
                    dInfo.generatetime = time;
                }
                if (enemy != null && enemy.IsBoss)
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