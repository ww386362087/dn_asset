using Level;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using XTable;

namespace XEditor
{

    public class EditorWave : BaseWave
    {
        public uint _entity_id;
        public GameObject _prefab;
        public GameObject go;
        public string _buff_id;
        public int _buff_percent;
        public List<int> _prefabSlot;
        public bool _bVisibleInEditor = true;
        public float rectX = 0, rectY = 90;
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
            get { return spawnType; }
            set
            {
                spawnType = value;
                if (spawnType == LevelSpawnType.Spawn_Role)
                {
                    _prefab = Resources.Load("Prefabs/NPC_pope") as GameObject;
                    _entity_id = 0;
                    if (_window != null) _window.GenerateIcon();
                }
                if (spawnType == LevelSpawnType.Spawn_NPC)
                {
                    _prefab = Resources.Load("Prefabs/NPC_Velskud_Wing") as GameObject;
                    _entity_id = 0;
                    if (_window != null) _window.GenerateIcon();
                }
            }
        }

        public bool HasBuff
        {
            get
            {
                if (string.IsNullOrEmpty(_buff_id)) return false;
                int doodad = 0;
                if (int.TryParse(_buff_id, out doodad))
                {
                    if (doodad == 0) return false;
                }
                return true;
            }
        }

        public uint EntityID
        {
            get { return _entity_id; }
            set
            {
                if (_entity_id != value)
                {
                    if (value > 0)
                    {
                        _entity_id = value;
                        levelscript = null;
                        if (SpawnType == LevelSpawnType.Spawn_Monster)
                        {
                            XEntityStatistics.RowData row = LevelMgr.EnemyList.GetByID((int)_entity_id);
                            if (row == null) return;
                            _prefab = Resources.Load("Prefabs/" + XTableMgr.GetTable<XEntityPresentation>().GetItemID((uint)row.PresentID).Prefab) as GameObject;
                            if (_window != null) _window.GenerateIcon();
                        }
                        if (SpawnType == LevelSpawnType.Spawn_Buff)
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
            get { return levelscript; }
            set
            {
                if (levelscript != value)
                {
                    levelscript = value;
                    _entity_id = 0;
                    if (_window != null) _window.GenerateIcon();
                }
            }
        }

        public WaveWindow LayoutWindow
        {
            get { return _window; }
        }

        public static string GetMonsterName(int wave)
        {
            return "Wave" + wave;
        }

        public EditorWave(int id)
        {
            if (_prefabSlot == null) _prefabSlot = new List<int>();
            InitWindow(id);
        }

        public EditorWave(SerializeLevel mgr, StreamReader sr)
        {
            if (_prefabSlot == null) _prefabSlot = new List<int>();
            LevelMgr = mgr;
            ReadFromFile(sr);
            InitWindow(_id);
        }

        private void InitWindow(int id)
        {
            if (_window == null)
            {
                if (id >= 1000) _window = new ScriptWaveWindow(this);
                else _window = new EntityWaveWindow(this);
            }
        }

        public static void ExtractInfoFromName(string name, out int wave, out int index)
        {
            int i = name.IndexOf("_monster");
            string strWave = name.Substring(4, i - 4);
            string strIndex = name.Substring(i + 8);
            wave = int.Parse(strWave);
            index = int.Parse(strIndex);
        }

        public bool ValidWave()
        {
            if (!string.IsNullOrEmpty(levelscript)) return true;
            return _prefab != null && roundCount > 0;
        }

        public void WriteToFile(StreamWriter sw)
        {
            if (ValidWave())
            {
                sw.WriteLine("bw");
                sw.WriteLine("id:{0}", _id);
                sw.WriteLine("st:{0}", (int)spawnType);

                if (string.IsNullOrEmpty(_buff_id)) _buff_id = "0";
                sw.WriteLine("bi:{0},{1},{2},{3},{4},{5},{6},{7},{8},{9},{10},{11}", time, loopInterval, _entity_id, _prefabSlot.Count, _bVisibleInEditor, yRotate, roundRidus, roundCount, isAroundPlayer, _buff_id, _buff_percent, repeat);
                if (levelscript != null && levelscript.Length > 0) sw.WriteLine("si:{0}", levelscript);

                if (preWaves != null && preWaves.Length > 0)
                {
                    string[] strPreWaves = preWaves.Split(',');
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
                    preWaves = _finalStr;
                    sw.WriteLine("pw:{0}", preWaves);
                }
                if (exString != null && exString.Length > 0) sw.WriteLine("es:{0}", exString);
                sw.WriteLine("ei:{0},{1}", _window._rect.x, _window._rect.y);

                if (go == null)
                {
                    string goName = GetMonsterName(_id);
                    go = GameObject.Find(goName);
                }
                if (go != null)
                {
                    Transform t = go.transform;
                    sw.WriteLine("mi:{0},{1},{2},{3},{4}", index, t.position.x, t.position.y, t.position.z, t.localRotation.eulerAngles.y);
                }
                else
                {
                    sw.WriteLine("mi:{0},{1},{2},{3},{4}", index, 0, 0, 0, 0);
                }
                sw.WriteLine("ew");
            }
        }

        protected override void ParseInfo(string data)
        {
            base.ParseInfo(data);
            string rawData = data.Substring(3);
            switch (infotype)
            {
                case InfoType.TypeSpawnType:
                    SpawnType = spawnType;
                    break;
                case InfoType.TypeBaseInfo:
                    EntityID = entityid;
                    break;
                case InfoType.TypeExString:
                    string[] strInfos = rawData.Split('|');
                    if (strInfos.Length > 0) exString = strInfos[0];
                    break;
                case InfoType.TypeEditor:
                    if (rawData.Length > 0)
                    {
                        string[] strEditorCoords = rawData.Split(',');
                        rectX = int.Parse(strEditorCoords[0]);
                        rectY = int.Parse(strEditorCoords[1]);
                    }
                    break;
                case InfoType.TypeMonsterInfo:
                    string[] strFloats = rawData.Split(',');
                    int index = int.Parse(strFloats[0]);
                    _prefabSlot.Add(index);
                    if (_prefab != null)
                    {
                        if (go != null) GameObject.DestroyImmediate(go);
                        go = GameObject.Instantiate(_prefab);
                        go.name = GetMonsterName(_id);
                        go.transform.position = pos;
                        go.transform.Rotate(0, rotateY, 0);

                        XEntityStatistics.RowData sData = XTableMgr.GetTable<XEntityStatistics>().GetByID((int)_entity_id);
                        if (sData == null)
                        {
                            XDebug.Log("enemy id not exist:", _entity_id);
                            break;
                        }
                        if (sData.PresentID > 0)
                        {
                            XEntityPresentation.RowData pData = XTableMgr.GetTable<XEntityPresentation>().GetItemID((uint)sData.PresentID);
                            go.transform.localScale = Vector3.one * pData.Scale;
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
            if (go != null) GameObject.DestroyImmediate(go);
        }

        public void DrawWaveWindow()
        {
            _window.Draw();
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

        public void GenerateMonster()
        {
            if (go != null) GameObject.DestroyImmediate(go);
            go = GameObject.Instantiate(_prefab);
            go.name = GetMonsterName(_id);
            go.transform.position = pos;
            go.transform.Rotate(0, rotateY, 0);
        }

    }

}