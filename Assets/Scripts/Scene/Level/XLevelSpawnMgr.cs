using UnityEngine;
using System.IO;

namespace Level
{
    public class XLevelSpawnMgr : XSingleton<XLevelSpawnMgr>
    {
        private XLevelSpawnInfo _curSpawner;
        private float _time = 0;
        public bool BossExtarScriptExecuting = false;
        public bool IsCurrentLevelWin { get; set; }
        public bool IsCurrentLevelFinished { get; set; }

        public XLevelSpawnInfo currSpawn { get { return _curSpawner; } }

        public void Update(float deltaT)
        {
            if (IsCurrentLevelFinished) return;
            _time += deltaT;
            if (_curSpawner != null) _curSpawner.Update(_time);
        }

        public void ForceLevelFinish(bool win)
        {
            IsCurrentLevelFinished = true;
            if (win)
            {
                XLevelState ls = XLevelStatistics.singleton.ls;
                IsCurrentLevelWin = true;
                OnLevelFinish(ls._lastDieEntityPos + new Vector3(0.0f, ls._lastDieEntityHeight, 0.0f) / 2, ls._lastDieEntityPos, 500, 0, true);
            }
            else
            {
                OnLevelFailed();
            }
        }

        public void OnLevelFinish(Vector3 dropInitPos, Vector3 dropGounrdPos, uint money, uint itemCount, bool bKillOpponent)
        {
        }

        public void OnLevelFailed()
        {
        }

        public void OnEnterScene(uint sceneid)
        {
            _time = 0;
            XLevelScriptMgr.singleton.CommandCount = 0;
            string configFile = XScene.singleton.SceneRow.SceneFile;
            if (configFile.Length == 0)
            {
                _curSpawner = null;
                XLevelScriptMgr.singleton.ClearWallInfo();
                XLevelScriptMgr.singleton.Reset();
                return;
            }

            if (_curSpawner == null) _curSpawner = new XLevelSpawnInfo();
            else _curSpawner.Clear();
            Stream s = XResources.ReadText("Table/" + configFile);
            //using (StreamReader sr = new StreamReader(s))
            StreamReader sr = new StreamReader(s);
            {
                string line = sr.ReadLine();
                int totalWave = int.Parse(line);
                line = sr.ReadLine();
                int PreloadWave = int.Parse(line);
                for (int i = 0; i < PreloadWave; i++)
                {
                    line = sr.ReadLine();
                    string[] info = line.Split(',');
                    int enemyID = int.Parse(info[0].Substring(3));
                    int count = int.Parse(info[1]);
                    _curSpawner.preloadInfo.Add(enemyID, count);
                }
                for (int id = 0; id < totalWave; id++)
                {
                    XLevelWave _wave = new XLevelWave();
                    _wave.ReadFromFile(sr);
                    _curSpawner.waves.Add(_wave);

                    XLevelDynamicInfo dInfo = new XLevelDynamicInfo();
                    dInfo.id = _wave.ID;
                    dInfo.totalCount = _wave.monsterPos.Count + _wave.RoundCount;
                    dInfo.Reset();
                    _curSpawner.wavesDynamicInfo.Add(_wave.ID, dInfo);
                }

                XResources.ClearStream(s);
            }
        }
    }
}