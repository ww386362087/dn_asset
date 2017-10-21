using Level;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using XTable;

namespace XEditor
{
   
    public class LevelWave :BaseWave
    {
        public uint _EnemyID;
        public GameObject _prefab;
        public string _doodad_id;
        public int _doodad_percent;
        public List<int> _prefabSlot;
        public bool _bVisibleInEditor = true;
        SerializeLevel _levelMgr;
        [NonSerialized]
        private WaveWindow _window;

        public SerializeLevel LevelMgr
        {
            get { return _levelMgr; }
            set { _levelMgr = value; }
        }

        public LevelSpawnType SpawnType
        {
            get { return _spawnType; }
            set
            {
                _spawnType = value;
                if (_spawnType == LevelSpawnType.Spawn_Source_Player)
                {
                    _prefab = Resources.Load("Prefabs/NPC_pope") as GameObject;
                    _EnemyID = 0;
                    if (_window != null) _window.GenerateIcon();
                }
                if (_spawnType == LevelSpawnType.Spawn_Source_Random)
                {
                    _prefab = Resources.Load("Prefabs/NPC_Velskud_Wing") as GameObject;
                    _EnemyID = 0;
                    if (_window != null)
                        _window.GenerateIcon();
                }
            }
        }

        public bool HasDoodad
        {
            get
            {
                if (string.IsNullOrEmpty(_doodad_id)) return false;
                int doodad = 0;
                if (int.TryParse(_doodad_id, out doodad))
                {
                    if (doodad == 0) return false;
                }
                return true;
            }
        }

        public uint EntityID
        {
            get { return _EnemyID; }
            set
            {
                if (_EnemyID != value)
                {
                    if (value > 0)
                    {
                        _EnemyID = value;
                        _levelscript = null;
                        if (SpawnType == LevelSpawnType.Spawn_Source_Monster)
                        {
                            XEntityStatistics.RowData row = LevelMgr.EnemyList.GetByID((int)_EnemyID);
                            if (row == null) return;
                            _prefab = Resources.Load("Prefabs/" + XTableMgr.GetTable<XEntityPresentation>().GetItemID((uint)row.PresentID).Prefab) as GameObject;
                            if (_window != null) _window.GenerateIcon();
                        }
                        if (SpawnType == LevelSpawnType.Spawn_Source_Buff)
                        {
                            _prefab = Resources.Load("Effects/FX_Particle/Roles/Lzg_Ty/Ty_buff_jx_m") as GameObject;
                            if (_window != null) _window.GenerateIcon();
                        }
                    }
                }
            }
        }

        public string LevelScript
        {
            get { return _levelscript; }
            set
            {
                if (_levelscript != value)
                {
                    _levelscript = value;
                    _EnemyID = 0;
                    if (_window != null)
                        _window.GenerateIcon();
                }
            }
        }
        

        public bool VisibleInEditor
        {
            get { return _bVisibleInEditor; }
            set
            {
                _bVisibleInEditor = value;
                SetWaveVisible(_bVisibleInEditor);
            }
        }

        public float RoundRidous
        {
            get { return _roundRidus; }
            set { _roundRidus = value; }
        }

        public int RoundCount
        {
            get { return _roundCount; }
            set { _roundCount = value; }
        }

        public float Time
        {
            get { return _time; }
            set { if (_time != value) _time = value; }
        }

        public int Random
        {
            get { return _randomID; }
            set { _randomID = value; }
        }

        public WaveWindow LayoutWindow
        {
            get { return _window; }
        }

        public static string GetMonsterName(int wave, int index)
        {
            return "Wave" + wave + "_monster" + index;
        }

        public static void ExtractInfoFromName(string name, out int wave, out int index)
        {
            int i = name.IndexOf("_monster");
            string strWave = name.Substring(4, i - 4);
            string strIndex = name.Substring(i + 8);
            wave = int.Parse(strWave);
            index = int.Parse(strIndex);
        }

        // construct here
        public LevelWave()
        {
            if (_prefabSlot == null)
                _prefabSlot = new List<int>();
            if (_window == null)
                _window = new WaveWindow(this);
        }

        public bool ValidWave()
        {
            if (!string.IsNullOrEmpty(_levelscript)) return true;
            if (_prefab != null && (_prefabSlot.Count > 0 || (_roundRidus > 0 && _roundCount > 0))) return true;
            return false;
        }

        public void WriteToFile(StreamWriter sw)
        {
            if (ValidWave())
            {
                sw.WriteLine("bw");
                sw.WriteLine("id:{0}", _id);
                sw.WriteLine("st:{0}", (int)_spawnType);

                if (string.IsNullOrEmpty(_doodad_id)) _doodad_id = "0";
                sw.WriteLine("bi:{0},{1},{2},{3},{4},{5},{6},{7},{8},{9},{10},{11}", _time, _loopInterval, _EnemyID, _prefabSlot.Count, _bVisibleInEditor, 0, _roundRidus, _roundCount, _randomID, _doodad_id, _doodad_percent, _repeat);
                if (_levelscript != null && _levelscript.Length > 0) sw.WriteLine("si:{0}", _levelscript);

                if (_preWaves != null && _preWaves.Length > 0)
                {
                    string[] strPreWaves = _preWaves.Split(',');
                    string _finalStr = "";
                    for (int i = 0; i < strPreWaves.Length; i++)
                    {
                        int w = int.Parse(strPreWaves[i]);
                        if (w != _id)
                        {
                            if (_finalStr.Length == 0)
                                _finalStr += w;
                            else
                                _finalStr += (',' + w.ToString());
                        }
                    }
                    _preWaves = _finalStr;
                    sw.WriteLine("pw:{0}|{1}", _preWaves, _preWavePercent);
                }
                if (_exString != null && _exString.Length > 0) sw.WriteLine("es:{0}", _exString);
                sw.WriteLine("ei:{0},{1}", _window._rect.x, _window._rect.y);
                foreach (int index in _prefabSlot)
                {
                    string goName = GetMonsterName(_id, index);
                    GameObject go = GameObject.Find(goName);
                    if (go != null)
                    {
                        Transform t = go.transform;
                        sw.WriteLine("mi:{0},{1},{2},{3},{4}", index, t.position.x, t.position.y, t.position.z, t.localRotation.eulerAngles.y);
                    }
                }
                sw.WriteLine("ew");
            }
        }

        protected override void ParseInfo(string data)
        {
            base.ParseInfo(data);
            string rawData = data.Substring(3);
            switch (_infotype)
            {
                case InfoType.TypeSpawnType:
                    SpawnType = _spawnType;
                    break;
                case InfoType.TypeBaseInfo:
                    EntityID = _entityid;
                    break;
                case InfoType.TypeExString:
                    {
                        string[] strInfos = rawData.Split('|');
                        if (strInfos.Length > 0) _exString = strInfos[0];
                    }
                    break;
                case InfoType.TypeEditor:
                    if (rawData.Length > 0)
                    {
                        string[] strEditorCoords = rawData.Split(',');
                        if (_window != null)
                        {
                            _window._rect.x = int.Parse(strEditorCoords[0]);
                            _window._rect.y = int.Parse(strEditorCoords[1]);
                        }
                    }
                    break;
                case InfoType.TypeMonsterInfo:
                    {
                        string[] strFloats = rawData.Split(',');
                        int index = int.Parse(strFloats[0]);
                        _prefabSlot.Add(index);
                        if (_prefab != null)
                        {
                            GameObject go = GameObject.Instantiate(_prefab);
                            go.name = GetMonsterName(_id, index);
                            go.transform.position = _pos;
                            go.transform.Rotate(0, _rotateY, 0);

                            XEntityStatistics.RowData sData = XTableMgr.GetTable<XEntityStatistics>().GetByID((int)_EnemyID);
                            if (sData == null)
                            {
                                XDebug.Log("enemy id not exist:", _EnemyID);
                                break;
                            }
                            if (sData.PresentID > 0)
                            {
                                XEntityPresentation.RowData pData = XTableMgr.GetTable<XEntityPresentation>().GetItemID((uint)sData.PresentID);
                                go.transform.localScale = Vector3.one * pData.Scale;
                            }
                        }
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

        public void RemoveSceneViewInstance()
        {
            foreach (int i in _prefabSlot)
            {
                string goName = GetMonsterName(_id, i);
                GameObject go = GameObject.Find(goName);
                if (go != null) GameObject.DestroyImmediate(go);
            }
        }

        public void DrawWaveWindow()
        {
            _window.draw();
        }

        public void SetInstanceFace()
        {
            foreach (int i in _prefabSlot)
            {
                string name = GetMonsterName(_id, i);
                GameObject go = GameObject.Find(name);
                if (go != null)
                {
                    go.transform.rotation = Quaternion.Euler(0, _yRotate, 0);
                }
            }
        }

        public void SetWaveVisible(bool bShow)
        {
            foreach (int i in _prefabSlot)
            {
                string name = GetMonsterName(_id, i);
                GameObject go = GameObject.Find(name);
                if (go != null)
                {
                    SkinnedMeshRenderer[] smrs = go.GetComponentsInChildren<SkinnedMeshRenderer>();
                    foreach (SkinnedMeshRenderer smr in smrs)
                        smr.enabled = bShow;
                    MeshRenderer[] mrs = go.GetComponentsInChildren<MeshRenderer>();
                    foreach (MeshRenderer mr in mrs)
                        mr.enabled = bShow;
                }
            }
        }

        protected void MarkMonster(GameObject monster, GameObject mark)
        {
            GameObject markgo = GameObject.Instantiate(mark) as GameObject;
            markgo.transform.parent = monster.transform;
            markgo.transform.localPosition = new Vector3(0, LevelMgr.MarkHeight, 0);
            markgo.transform.localRotation = new Quaternion(0, 90, 90, 0);
            markgo.transform.localScale = Vector3.one;

            ParticleSystem[] systems = markgo.GetComponentsInChildren<ParticleSystem>();
            foreach (ParticleSystem system in systems) system.Play();
        }


        public void RemoveMonster(GameObject go)
        {
            int wave = 0;
            int index = 0;
            ExtractInfoFromName(go.name, out wave, out index);
            if (wave == _id)
            {
                GameObject.DestroyImmediate(go);
                _prefabSlot.Remove(index);
            }
        }

    }

}