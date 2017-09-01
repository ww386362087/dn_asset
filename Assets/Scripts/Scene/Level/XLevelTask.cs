using UnityEngine;


namespace Level
{

    internal class XLevelBaseTask
    {
        public int _id;
        protected XLevelSpawnInfo _spawner;

        public XLevelBaseTask(XLevelSpawnInfo ls)
        {
            _spawner = ls;
        }

        public virtual bool Execute(float time)
        {
            return true;
        }
    }


    internal class XLevelSpawnTask : XLevelBaseTask
    {
        // level id
        public uint _EnemyID;        // enemy template id 
        public int _MonsterRotate;
        public int _MonsterIndex;
        public Vector3 _MonsterPos;
        public LevelSpawnType _SpawnType;
        public bool _IsSummonTask;

        public XLevelSpawnTask(XLevelSpawnInfo ls)
            : base(ls)
        {
            _IsSummonTask = false;
        }

        public XEntity CreateClientMonster(uint id, float yRotate, Vector3 pos, int _waveid)
        {
            Quaternion rotation = Quaternion.Euler(0, yRotate, 0);

            XEntity entity = XEntityMgr.singleton.CreateEntity(id, pos, rotation, true);
            if (entity != null)
            {
                entity.Wave = _waveid;
                entity.CreateTime = Time.realtimeSinceStartup;
                XAIEventArgs aievent = new XAIEventArgs();
                aievent.DepracatedPass = true;
                aievent.EventType = 1;
                aievent.EventArg = "SpawnMonster";
                XEventMgr.singleton.FireEvent(aievent, 0.05f);
                return entity;
            }
            return null;
        }


        public XEntity CreateServerAttrMonster(object data, float yRotate, Vector3 pos, int _waveid)
        {
            //to-do CreateServerAttrMonster

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
            if (_SpawnType == LevelSpawnType.Spawn_Source_Monster)
            {
                // 从本地创建
                enemy = CreateClientMonster(_EnemyID, _MonsterRotate, _MonsterPos + new Vector3(0, 0.02f, 0), _id);
                XLevelStatistics.singleton.ls.AddLevelSpawnEntityCount(enemy.EntityID);
            }
            else if (_SpawnType == LevelSpawnType.Spawn_Source_Doodad)
            {
                // 单机现在不处理直接掉doodad
            }
            else
            {
                // 属性和外形来自服务器
                //UnitAppearance data = XLevelSpawnMgr.singleton.GetCacheServerMonster((uint)_id);
                //if (data != null) enemy = CreateServerAttrMonster(data, _MonsterRotate, _MonsterPos + new Vector3(0, 0.02f, 0), _id);
                //XLevelStatistics.singleton.ls.AddLevelSpawnEntityCount(enemy.EntityID);
            }

            if (dInfo != null)
            {
                if (enemy != null)
                {
                    dInfo._generateCount++;
                    dInfo._enemyIds.Add(enemy.EntityID);
                }
                if (dInfo._generateCount == dInfo._TotalCount)
                {
                    dInfo._generatetime = time;
                }
                if (enemy != null && enemy.IsBoss)
                {
                    //  XTutorialHelper.singleton.HasBoss = true;
                    return false;
                }
            }

            return true;
        }
    }

    internal class XLevelScriptTask : XLevelBaseTask
    {
        public string _ScriptName;

        public XLevelScriptTask(XLevelSpawnInfo ls)
            : base(ls)
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