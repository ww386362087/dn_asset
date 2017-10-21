using System.Collections.Generic;
using System.IO;
using UnityEngine;


namespace Level
{
    public class XLevelWave : BaseWave
    {
        public List<int> _preWave = new List<int>();
        public Dictionary<int, Vector3> _monsterPos = new Dictionary<int, Vector3>();
        public Dictionary<int, Vector3> _monsterRot = new Dictionary<int, Vector3>();

        protected override void ParseInfo(string data)
        {
            base.ParseInfo(data);
            switch (_infotype)
            {
                case InfoType.TypePreWave:
                    if (!string.IsNullOrEmpty(_preWaves))
                    {
                        string[] strPreWaves = _preWaves.Split(',');
                        for (int i = 0; i < strPreWaves.Length; i++)
                        {
                            int preWave = 0;
                            if (int.TryParse(strPreWaves[i], out preWave))
                            {
                                _preWave.Add(preWave);
                            }
                        }
                    }
                    break;
                case InfoType.TypeMonsterInfo:
                    _monsterPos.Add(_index, _pos);
                    Vector3 rot = new Vector3(0, _rotateY, 0);
                    _monsterRot.Add(_index, rot);
                    break;
            }
        }

        public void ReadFromFile(StreamReader sr)
        {
            string strLine = sr.ReadLine();
            if (strLine != "bw") return;
            while (true)
            {
                strLine = sr.ReadLine();
                if (strLine == "ew") break;
                ParseInfo(strLine);
            }
        }

        public bool IsScriptWave()
        {
            if (_levelscript != null && _levelscript.Length > 0) return true;
            return false;
        }
    }

    public class XLevelDynamicInfo
    {
        public int _id;
        public bool _pushIntoTask = false;
        public float _generatetime = 0f;
        public float _prewaveFinishTime = 0f;
        public float _exStringFinishTime = 0f;
        public int _TotalCount = 0;
        public int _generateCount = 0;
        public int _dieCount = 0;
        public List<uint> _enemyIds = new List<uint>();

        public void Reset()
        {
            _pushIntoTask = false;
            _generatetime = 0f;
            _generateCount = 0;
            _prewaveFinishTime = 0f;
            _exStringFinishTime = 0f;
            _dieCount = 0;
            _enemyIds.Clear();
        }
    }

    public class XLevelSpawnInfo
    {
        public List<XLevelWave> _waves = new List<XLevelWave>();

        public Dictionary<int, XLevelDynamicInfo> _wavesDynamicInfo = new Dictionary<int, XLevelDynamicInfo>();

        public Dictionary<int, int> _preloadInfo = new Dictionary<int, int>();

        private Queue<XLevelBaseTask> _tasks = new Queue<XLevelBaseTask>();

        private const int spawn_monster_per_frame = 2;

        public void Clear()
        {
            _waves.Clear();
            foreach (var info in _wavesDynamicInfo)
            {
                info.Value.Reset();
            }
            _wavesDynamicInfo.Clear();
            _preloadInfo.Clear();
            _tasks.Clear();
        }

        public void ResetDynamicInfo()
        {
            foreach (KeyValuePair<int, XLevelDynamicInfo> xLevelDynamicInfo in _wavesDynamicInfo)
            {
                xLevelDynamicInfo.Value.Reset();
            }
        }

        public void KillSpawn(int waveid)
        {
            //to-do KillSpawn
        }

        public void ShowBubble(int typeid, string text, float exist)
        {
            //to-do ShowBubble
        }

        public void Update(float time)
        {
            if (!XScene.singleton.SyncMode)
            {
                _SoloUpdate(time);
            }
        }

        protected void _SoloUpdate(float time)
        {
            for (int i = 0; i < _waves.Count; i++)
            {
                XLevelDynamicInfo dInfo = GetWaveDynamicInfo(_waves[i]._id);
                if (dInfo == null || dInfo._pushIntoTask) continue;
                if (dInfo._TotalCount != 0 && dInfo._generateCount == dInfo._TotalCount) continue;

                bool preWaveFinished = true;
                for (int j = 0; j < _waves[i]._preWave.Count; j++)
                {
                    XLevelDynamicInfo predInfo;
                    if (_wavesDynamicInfo.TryGetValue(_waves[i]._preWave[j], out predInfo))
                    {
                        // 还没生成
                        if ((predInfo._generateCount != predInfo._TotalCount))
                        {
                            preWaveFinished = false;
                            break;
                        }
                        if (predInfo._enemyIds.Count == 0)
                        {
                            preWaveFinished = true;
                        }
                        else if (predInfo._enemyIds.Count == 1)
                        {
                            XEntity preEnemy = XEntityMgr.singleton.GetEntity(predInfo._enemyIds[0]);

                            if (preEnemy != null)
                            {
                                double maxHP = preEnemy.Attributes.GetAttr(XAttributeDefine.XAttr_MaxHP_Total);
                                double currentHP = preEnemy.Attributes.GetAttr(XAttributeDefine.XAttr_CurrentHP_Basic);
                                if (currentHP > maxHP * _waves[i]._preWavePercent)
                                {
                                    preWaveFinished = false;
                                    break;
                                }
                            }
                        }
                        else
                        {
                            if (predInfo._generateCount != predInfo._dieCount)
                            {
                                preWaveFinished = false;
                                break;
                            }
                        }
                    }
                }

                if (!preWaveFinished) continue;

                bool exStringExists = true;
                if (_waves[i]._exString != null && _waves[i]._exString.Length > 0)
                {
                    if (!XLevelScriptMgr.singleton.QueryExternalString(_waves[i]._exString, false))
                    {
                        exStringExists = false;
                    }
                }

                if (exStringExists && dInfo._exStringFinishTime == 0f) dInfo._exStringFinishTime = time;

                if (preWaveFinished && XCommon.singleton.IsEqual(dInfo._prewaveFinishTime, 0f)) dInfo._prewaveFinishTime = time;

                bool bCanGenerate = false;

                if (_waves[i]._exString != null && _waves[i]._exString.Length > 0)
                {
                    if (time - dInfo._exStringFinishTime >= _waves[i]._time && dInfo._exStringFinishTime > 0)
                    {
                        bCanGenerate = true;
                    }
                }
                else if (_waves[i].IsScriptWave() || (dInfo._generateCount < dInfo._TotalCount))
                {
                    if (preWaveFinished && time - dInfo._prewaveFinishTime >= _waves[i]._time)
                    {
                        bCanGenerate = true;
                    }
                }

                if (bCanGenerate)
                {
                    if (_waves[i].IsScriptWave())
                    {
                        GenerateScriptTask(_waves[i]);
                    }
                    else
                    {
                        // normal monsters
                        GenerateNormalTask(_waves[i]);
                        // round monsters
                        GenerateRoundTask(_waves[i]);
                    }

                    if (_waves[i]._repeat == false)
                        dInfo._pushIntoTask = true;

                    if (_waves[i]._repeat && _waves[i]._exString != null && _waves[i]._exString.Length > 0)
                    {
                        XLevelScriptMgr.singleton.QueryExternalString(_waves[i]._exString, true);
                        dInfo._exStringFinishTime = 0;
                    }
                }
            }

            ProcessTaskQueue(time);
        }

        public bool ExecuteWaveExtraScript(int wave)
        {
            for (int i = 0; i < _waves.Count; i++)
            {
                XLevelDynamicInfo dInfo = GetWaveDynamicInfo(_waves[i]._id);

                if (dInfo == null) continue;
                if (dInfo._pushIntoTask) continue;

                if (_waves[i]._preWave.Count > 0 && _waves[i]._preWave[0] == wave && _waves[i].IsScriptWave())
                {
                    if (XLevelScriptMgr.singleton.IsTalkScript(_waves[i]._levelscript))
                    {
                        XLevelSpawnMgr.singleton.BossExtarScriptExecuting = true;
                        XTimerMgr.singleton.SetTimer(_waves[i]._time, RunExtraScript, _waves[i]._levelscript);
                        return true;
                    }
                }
            }
            return false;
        }

        protected void RunExtraScript(object o)
        {
            string sc = (string)o;
            XLevelScriptMgr.singleton.RunScript(sc);
        }

        protected void ProcessTaskQueue(float time)
        {
            bool bContinueExecuteTask = true;
            for (int i = 0; i < spawn_monster_per_frame; i++)
            {
                if (bContinueExecuteTask)
                {
                    if (_tasks.Count > 0)
                    {
                        XLevelBaseTask task = _tasks.Dequeue();
                        bContinueExecuteTask = task.Execute(time);
                    }
                }
            }
        }

        public XLevelDynamicInfo GetWaveDynamicInfo(int waveid)
        {
            XLevelDynamicInfo ret;
            if (_wavesDynamicInfo.TryGetValue(waveid, out ret))
            {
                return ret;
            }
            return null;
        }

        protected XLevelWave GetWaveInfo(int waveid)
        {
            for (int i = 0; i < _waves.Count; i++)
            {
                if (_waves[i]._id == waveid)
                    return _waves[i];
            }
            return null;
        }

        public void OnMonsterDie(XEntity entity)
        {
            XLevelDynamicInfo ret;
            if (_wavesDynamicInfo.TryGetValue(entity.Wave, out ret))
            {
                ret._dieCount += 1;
                ret._enemyIds.Remove(entity.EntityID);
            }
            // XLevelDoodadMgr.singleton.OnMonsterDie(entity);
        }

        public void GenerateExternalSpawnTask(uint enemyID, Vector3 pos, int rot)
        {
            XLevelSpawnTask task = new XLevelSpawnTask(this);
            task._id = -1;
            task._EnemyID = enemyID;
            task._MonsterRotate = rot;
            task._MonsterIndex = 0;
            task._MonsterPos = pos;
            task._SpawnType = LevelSpawnType.Spawn_Source_Monster;
            task._IsSummonTask = true;
            _tasks.Enqueue(task);
        }

        protected void GenerateNormalTask(XLevelWave wave)
        {
            XLevelState ls = XLevelStatistics.singleton.ls;
            ls._monster_refresh_time.Add((uint)(Time.time - ls._start_time));
            foreach (var item in wave._monsterPos)
            {
                XLevelSpawnTask task = new XLevelSpawnTask(this);
                task._id = wave._id;
                task._EnemyID = wave._entityid;
                task._MonsterRotate = (int)wave._monsterRot[item.Key].y;
                task._MonsterIndex = item.Key;
                task._MonsterPos = item.Value;
                task._SpawnType = wave._spawnType;
                task._IsSummonTask = false;
                _tasks.Enqueue(task);
            }
        }

        protected void GenerateRoundTask(XLevelWave wave)
        {
            if (wave._roundRidus > 0 && wave._roundCount > 0)
            {
                XLevelStatistics.singleton.ls._monster_refresh_time.Add((uint)(Time.time - XLevelStatistics.singleton.ls._start_time));

                float angle = 360.0f / wave._roundCount;
                Vector3 playerPos = XEntityMgr.singleton.Player.Position;

                for (int i = 0; i < wave._roundCount; i++)
                {
                    XLevelSpawnTask task = new XLevelSpawnTask(this);
                    task._id = wave._id;
                    task._EnemyID = wave._entityid;
                    task._MonsterIndex = 0; // no use now

                    task._MonsterPos = playerPos +
                                       Quaternion.Euler(0, angle * i, 0) * new Vector3(0, 0, 1) * wave._roundRidus;

                    task._MonsterRotate = (int)angle * i + 180;
                    task._SpawnType = wave._spawnType;
                    task._IsSummonTask = false;

                    _tasks.Enqueue(task);
                }
            }
        }

        protected void GenerateScriptTask(XLevelWave wave)
        {
            XLevelScriptTask task = new XLevelScriptTask(this);
            task._ScriptName = wave._levelscript;
            _tasks.Enqueue(task);
        }

        public bool QueryMonsterStaticInfo(uint monsterID, ref Vector3 position, ref float face)
        {
            for (int i = 0; i < _waves.Count; i++)
            {
                if (_waves[i]._entityid == monsterID)
                {
                    foreach (int key in _waves[i]._monsterPos.Keys)
                    {
                        position = _waves[i]._monsterPos[key];
                        face = _waves[i]._monsterRot[key].y;
                        return true;
                    }
                }
            }
            return false;
        }
    }

}