using System.Collections.Generic;
using System.IO;
using UnityEngine;


namespace Level
{
    internal class XLevelWave
    {
        public int _id;

        public LevelSpawnType _spawnType;

        public float _time;

        public int _loopInterval;

        public uint _EnemyID;

        public int _randomID;

        public int _yRotate;

        public bool _repeat;

        public string _exString;
        public List<int> _preWave = new List<int>();
        public float _preWavePercent;

        public Dictionary<int, Vector3> _monsterPos = new Dictionary<int, Vector3>();

        public Dictionary<int, Vector3> _monsterRot = new Dictionary<int, Vector3>();

        public float _roundRidus;

        public int _roundCount;

        public string _levelscript;

        protected void ParseInfo(string data)
        {
            InfoType type = InfoType.TypeNone;
            if (data.StartsWith("id")) type = InfoType.TypeId;
            else if (data.StartsWith("bi")) type = InfoType.TypeBaseInfo;
            else if (data.StartsWith("pw")) type = InfoType.TypePreWave;
            else if (data.StartsWith("ei")) type = InfoType.TypeEditor;
            else if (data.StartsWith("mi")) type = InfoType.TypeMonsterInfo;
            else if (data.StartsWith("si")) type = InfoType.TypeScript;
            else if (data.StartsWith("es")) type = InfoType.TypeExString;
            else if (data.StartsWith("st")) type = InfoType.TypeSpawnType;

            string rawData = data.Substring(3);

            switch (type)
            {
                case InfoType.TypeId:
                    _id = int.Parse(rawData);
                    break;

                case InfoType.TypeSpawnType:
                    _spawnType = (LevelSpawnType)(int.Parse(rawData));
                    break;
                case InfoType.TypeBaseInfo:
                    string[] strInfos = rawData.Split(',');
                    _time = float.Parse(strInfos[0]);
                    _loopInterval = int.Parse(strInfos[1]);
                    _EnemyID = uint.Parse(strInfos[2]);
                    _yRotate = int.Parse(strInfos[5]);
                    if (strInfos.Length > 6)
                        _roundRidus = float.Parse(strInfos[6]);

                    if (strInfos.Length > 7)
                        _roundCount = int.Parse(strInfos[7]);

                    if (strInfos.Length > 8)
                        _randomID = int.Parse(strInfos[8]);

                    if (strInfos.Length > 11)
                        _repeat = bool.Parse(strInfos[11]);
                    break;
                case InfoType.TypeExString:
                    _exString = rawData;

                    break;
                case InfoType.TypePreWave:
                    strInfos = rawData.Split('|');

                    if (strInfos.Length > 0)
                    {
                        string strPreWave = strInfos[0];

                        if (strPreWave.Length > 0)
                        {
                            string[] strPreWaves = strPreWave.Split(',');

                            for (int i = 0; i < strPreWaves.Length; i++)
                            {
                                int preWave = 0;

                                if (int.TryParse(strPreWaves[i], out preWave))
                                {
                                    _preWave.Add(preWave);
                                }
                            }
                        }
                    }

                    if (strInfos.Length > 1)
                    {
                        int percent = int.Parse(strInfos[1]);
                        _preWavePercent = percent / 100.0f;
                    }
                    break;
                case InfoType.TypeEditor:
                    break;
                case InfoType.TypeMonsterInfo:
                    string[] strFloats = rawData.Split(',');

                    int index = int.Parse(strFloats[0]);

                    // generate gameobject in scene
                    float x = float.Parse(strFloats[1]);
                    float y = float.Parse(strFloats[2]);
                    float z = float.Parse(strFloats[3]);
                    Vector3 pos = new Vector3(x, y, z);
                    _monsterPos.Add(index, pos);
                    Vector3 rot = new Vector3(0, 0, 0);
                    if (strFloats.Length > 4)
                        rot.y = float.Parse(strFloats[4]);
                    _monsterRot.Add(index, rot);
                    break;
                case InfoType.TypeScript:
                    strInfos = rawData.Split(',');
                    if (strInfos.Length > 0)
                        _levelscript = strInfos[0];
                    break;
                default:
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

    internal class XLevelDynamicInfo
    {
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

    internal class XLevelSpawnInfo
    {
        public List<XLevelWave> _waves = new List<XLevelWave>();

        public Dictionary<int, XLevelDynamicInfo> _wavesDynamicInfo = new Dictionary<int, XLevelDynamicInfo>();

        public Dictionary<int, int> _preloadInfo = new Dictionary<int, int>();

        private Queue<XLevelBaseTask> _tasks = new Queue<XLevelBaseTask>();

        private const int spawn_monster_per_frame = 2;

        public void Clear()
        {
            _waves.Clear();
            foreach (KeyValuePair<int, XLevelDynamicInfo> xLevelDynamicInfo in _wavesDynamicInfo)
            {
                xLevelDynamicInfo.Value.Reset();
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

                if (dInfo == null) continue;

                if (dInfo._TotalCount != 0 && dInfo._generateCount == dInfo._TotalCount) continue;

                if (dInfo._pushIntoTask) continue;

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
                        TimerManager.singleton.AddTimer(_waves[i]._time, RunExtraScript, _waves[i]._levelscript);
                        return true;
                    }
                }
            }
            return false;
        }

        protected void RunExtraScript(int seq, object o)
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
            XLevelStatistics.singleton.ls._monster_refresh_time.Add((uint)(Time.time - XLevelStatistics.singleton.ls._start_time));
            foreach (KeyValuePair<int, Vector3> pair in wave._monsterPos)
            {
                XLevelSpawnTask task = new XLevelSpawnTask(this);
                task._id = wave._id;
                task._EnemyID = wave._EnemyID;
                task._MonsterRotate = (int)wave._monsterRot[pair.Key].y;
                task._MonsterIndex = pair.Key;
                task._MonsterPos = pair.Value;
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
                    task._EnemyID = wave._EnemyID;
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
                if (_waves[i]._EnemyID == monsterID)
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