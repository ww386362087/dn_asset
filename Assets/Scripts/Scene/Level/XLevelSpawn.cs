using System.Collections.Generic;
using System.IO;
using UnityEngine;


namespace Level
{
    public class XLevelWave : BaseWave
    {
        public List<int> preWave = new List<int>();
        public Dictionary<int, Vector3> monsterPos = new Dictionary<int, Vector3>();
        public Dictionary<int, Vector3> monsterRot = new Dictionary<int, Vector3>();

        protected override void ParseInfo(string data)
        {
            base.ParseInfo(data);
            switch (infotype)
            {
                case InfoType.TypePreWave:
                    if (!string.IsNullOrEmpty(preWaves))
                    {
                        string[] strPreWaves = preWaves.Split(',');
                        for (int i = 0; i < strPreWaves.Length; i++)
                        {
                            int preWave = 0;
                            if (int.TryParse(strPreWaves[i], out preWave))
                            {
                                this.preWave.Add(preWave);
                            }
                        }
                    }
                    break;
                case InfoType.TypeMonsterInfo:
                    if (!isAroundPlayer)
                    {
                        monsterPos.Add(index, pos);
                        Vector3 rot = new Vector3(0, rotateY, 0);
                        monsterRot.Add(index, rot);
                    }
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
            return !string.IsNullOrEmpty(levelscript);
        }
    }

    public class XLevelDynamicInfo
    {
        public int id;
        public bool pushIntoTask = false;
        public float generatetime = 0f;
        public float startTime = 0f;
        public float exStringFinishTime = 0f;
        public int totalCount = 0;
        public int generateCount = 0;
        public int dieCount = 0;
        public List<uint> entityIds = new List<uint>();

        public void Reset()
        {
            pushIntoTask = false;
            generatetime = 0f;
            generateCount = 0;
            startTime = 0f;
            exStringFinishTime = 0f;
            dieCount = 0;
            entityIds.Clear();
        }
    }

    public class XLevelSpawnInfo
    {
        public List<XLevelWave> waves = new List<XLevelWave>();
        public Dictionary<int, XLevelDynamicInfo> wavesDynamicInfo = new Dictionary<int, XLevelDynamicInfo>();
        public Dictionary<int, int> preloadInfo = new Dictionary<int, int>();
        private Queue<XLevelBaseTask> _tasks = new Queue<XLevelBaseTask>();
        private const int spawn_monster_per_frame = 2;

        public void Clear()
        {
            waves.Clear();
            foreach (var info in wavesDynamicInfo)
            {
                info.Value.Reset();
            }
            wavesDynamicInfo.Clear();
            preloadInfo.Clear();
            _tasks.Clear();
        }

        public void ResetDynamicInfo()
        {
            foreach (KeyValuePair<int, XLevelDynamicInfo> xLevelDynamicInfo in wavesDynamicInfo)
            {
                xLevelDynamicInfo.Value.Reset();
            }
        }

        public void KillSpawn(int waveid)
        {
            //to-do KillSpawn
        }
        
        public void Update(float time)
        {
            if (!XScene.singleton.SyncMode)
            {
                SoloUpdate(time);
            }
        }

        protected void SoloUpdate(float time)
        {
            for (int i = 0; i < waves.Count; i++)
            {
                XLevelDynamicInfo dInfo = GetWaveDynamicInfo(waves[i].ID);
                if (dInfo == null || dInfo.pushIntoTask) continue;
                if (dInfo.totalCount != 0 && dInfo.generateCount == dInfo.totalCount) continue;

                bool preWaveFinished = true;
                for (int j = 0; j < waves[i].preWave.Count; j++)
                {
                    XLevelDynamicInfo predInfo;
                    if (wavesDynamicInfo.TryGetValue(waves[i].preWave[j], out predInfo))
                    {
                        if ((predInfo.generateCount != predInfo.totalCount))
                        { // 还没生成
                            preWaveFinished = false;
                            break;
                        }
                        if (predInfo.entityIds.Count > 0)
                        {
                            if (predInfo.generateCount != predInfo.dieCount)
                            {
                                preWaveFinished = false;
                                break;
                            }
                        }
                    }
                }
                if (!preWaveFinished) continue;

                bool exStringExists = true;
                if (!string.IsNullOrEmpty(waves[i].exString))
                {
                    if (!XLevelScriptMgr.singleton.QueryExternalString(waves[i].exString, false))
                    {
                        exStringExists = false;
                    }
                }
                if (exStringExists && dInfo.exStringFinishTime == 0f) dInfo.exStringFinishTime = time;
                if (dInfo.startTime == 0f) dInfo.startTime = time;
                bool bCanGenerate = false;
                if (!string.IsNullOrEmpty(waves[i].exString))
                {
                    if (time - dInfo.exStringFinishTime >= waves[i].time && dInfo.exStringFinishTime > 0)
                    {
                        bCanGenerate = true;
                    }
                }
                else if (waves[i].IsScriptWave() || dInfo.generateCount < dInfo.totalCount)
                {
                    if (time - dInfo.startTime >= waves[i].time)
                    {
                        bCanGenerate = true;
                    }
                }

                if (bCanGenerate)
                {
                    if (waves[i].IsScriptWave())
                    {
                        GenerateScriptTask(waves[i]);
                    }
                    else
                    {
                        if (waves[i].isAroundPlayer) GenerateRoundTask(waves[i]);
                        else GenerateNormalTask(waves[i]);
                    }
                    if (!waves[i].repeat) dInfo.pushIntoTask = true;
                    if (waves[i].repeat && waves[i].exString != null && waves[i].exString.Length > 0)
                    {
                        XLevelScriptMgr.singleton.QueryExternalString(waves[i].exString, true);
                        dInfo.exStringFinishTime = 0;
                    }
                }
            }
            ProcessTaskQueue(time);
        }

        public bool ExecuteWaveExtraScript(int wave)
        {
            for (int i = 0; i < waves.Count; i++)
            {
                XLevelDynamicInfo dInfo = GetWaveDynamicInfo(waves[i].ID);

                if (dInfo == null) continue;
                if (dInfo.pushIntoTask) continue;

                if (waves[i].preWave.Count > 0 && waves[i].preWave[0] == wave && waves[i].IsScriptWave())
                {
                    if (XLevelScriptMgr.singleton.IsTalkScript(waves[i].levelscript))
                    {
                        XLevelSpawnMgr.singleton.BossExtarScriptExecuting = true;
                        XTimerMgr.singleton.SetTimer(waves[i].time, RunExtraScript, waves[i].levelscript);
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
            if (wavesDynamicInfo.TryGetValue(waveid, out ret))
            {
                return ret;
            }
            return null;
        }

        protected XLevelWave GetWaveInfo(int waveid)
        {
            for (int i = 0; i < waves.Count; i++)
            {
                if (waves[i].ID == waveid)
                    return waves[i];
            }
            return null;
        }

        public void OnMonsterDie(XEntity entity)
        {
            XLevelDynamicInfo ret;
            if (wavesDynamicInfo.TryGetValue(entity.Wave, out ret))
            {
                ret.dieCount += 1;
                ret.entityIds.Remove(entity.EntityID);
            }
        }
        

        protected void GenerateNormalTask(XLevelWave wave)
        {
            XLevelState ls = XLevelStatistics.singleton.ls;
            ls._monster_refresh_time.Add((uint)(Time.time - ls._start_time));
            foreach (var item in wave.monsterPos)
            {
                XLevelSpawnTask task = new XLevelSpawnTask(this);
                task._id = wave.ID;
                task._EnemyID = wave.entityid;
                task._MonsterRotate = (int)wave.monsterRot[item.Key].y;
                task._MonsterIndex = item.Key;
                task._MonsterPos = item.Value;
                task._SpawnType = wave.spawnType;
                task._IsSummonTask = false;
                _tasks.Enqueue(task);
            }
        }

        protected void GenerateRoundTask(XLevelWave wave)
        {
            if (wave.RoundRidous > 0 && wave.RoundCount > 0)
            {
                XLevelStatistics.singleton.ls._monster_refresh_time.Add((uint)(Time.time - XLevelStatistics.singleton.ls._start_time));
                float angle = 360.0f / wave.RoundCount;
                Vector3 playerPos = XEntityMgr.singleton.Player.Position;
                for (int i = 0; i < wave.RoundCount; i++)
                {
                    XLevelSpawnTask task = new XLevelSpawnTask(this);
                    task._id = wave.ID;
                    task._EnemyID = wave.entityid;
                    task._MonsterIndex = 0; // no use now
                    task._MonsterPos = playerPos + Quaternion.Euler(0, angle * i, 0) * new Vector3(0, 0, 1) * wave.RoundRidous;
                    task._MonsterRotate = (int)angle * i + 180;
                    task._SpawnType = wave.spawnType;
                    task._IsSummonTask = false;
                    _tasks.Enqueue(task);
                }
            }
        }

        protected void GenerateScriptTask(XLevelWave wave)
        {
            XLevelScriptTask task = new XLevelScriptTask(this);
            task._ScriptName = wave.levelscript;
            _tasks.Enqueue(task);
        }

        public bool QueryMonsterStaticInfo(uint monsterID, ref Vector3 position, ref float face)
        {
            for (int i = 0; i < waves.Count; i++)
            {
                if (waves[i].entityid == monsterID)
                {
                    foreach (int key in waves[i].monsterPos.Keys)
                    {
                        position = waves[i].monsterPos[key];
                        face = waves[i].monsterRot[key].y;
                        return true;
                    }
                }
            }
            return false;
        }
    }

}