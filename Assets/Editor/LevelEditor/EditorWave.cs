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
        public int _uni_id;
        public GameObject _prefab;
        public GameObject go;
        public string _buff_id;
        public int _buff_percent;
        public List<int> _prefabSlot;
        public bool _bVisibleInEditor = true;
        public float rectX = 0, rectY = 90;
        public string name;
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
                    _uni_id = 0;
                    if (_window != null) _window.GenerateIcon();
                }
            }
        }

        public bool HasBuff = false;

        public int UID
        {
            get { return _uni_id; }
            set
            {
                if (_uni_id != value)
                {
                    if (value > 0)
                    {
                        _uni_id = value;
                        levelscript = null;
                        if (SpawnType == LevelSpawnType.Spawn_Monster)
                        {
                            XEntityStatistics.RowData row = LevelMgr.EntityList.GetByID((int)_uni_id);
                            if (row == null) return;
                            name = row.Name;
                            _prefab = Resources.Load("Prefabs/" + XTableMgr.GetTable<XEntityPresentation>().GetItemID((uint)row.PresentID).Prefab) as GameObject;
                            GenerateInstance();
                            if (_window != null) _window.GenerateIcon();
                        }
                        else if (SpawnType == LevelSpawnType.Spawn_Buff)
                        {
                            _prefab = Resources.Load("Effects/FX_Particle/Roles/Lzg_Ty/Ty_buff_jx_m") as GameObject;
                            if (_window != null) _window.GenerateIcon();
                            name = "buff";
                        }
                        else if(SpawnType == LevelSpawnType.Spawn_NPC)
                        {
                            XNpcList.RowData row = XTableMgr.GetTable<XNpcList>().GetByUID((int)_uni_id);
                            if (row == null) return;
                            name = row.NPCName;
                            _prefab = Resources.Load("Prefabs/" + XTableMgr.GetTable<XEntityPresentation>().GetItemID((uint)row.PresentID).Prefab) as GameObject;
                            GenerateInstance();
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
                    _uni_id = 0;
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

        public bool ValidWave()
        {
            if (!string.IsNullOrEmpty(levelscript)) return true;
            return _prefab != null && count > 0;
        }

        public void WriteToFile(StreamWriter sw)
        {
            if (ValidWave())
            {
                sw.WriteLine("bw");
                sw.WriteLine("id:{0}", _id);
                sw.WriteLine("st:{0}", (int)spawnType);

                if (string.IsNullOrEmpty(_buff_id)) _buff_id = "0";
                sw.WriteLine("bi:{0},{1},{2},{3},{4},{5},{6},{7},{8},{9},{10},{11}", time, loopInterval, _uni_id, _prefabSlot.Count, _bVisibleInEditor, yRotate, radius, count, isAroundPlayer, _buff_id, _buff_percent, repeat);
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
                    sw.WriteLine("ti:{0},{1},{2},{3},{4}", index, t.position.x, t.position.y, t.position.z, t.localRotation.eulerAngles.y);
                }
                else
                {
                    sw.WriteLine("ti:{0},{1},{2},{3},{4}", index, 0, 0, 0, 0);
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
                case InfoType.SpawnType:
                    SpawnType = spawnType;
                    break;
                case InfoType.BaseInfo:
                    UID = uid;
                    break;
                case InfoType.ExString:
                    string[] strInfos = rawData.Split('|');
                    if (strInfos.Length > 0) exString = strInfos[0];
                    break;
                case InfoType.EditorInfo:
                    if (rawData.Length > 0)
                    {
                        string[] strEditorCoords = rawData.Split(',');
                        rectX = int.Parse(strEditorCoords[0]);
                        rectY = int.Parse(strEditorCoords[1]);
                    }
                    break;
                case InfoType.TransformInfo:
                    string[] strFloats = rawData.Split(',');
                    int index = int.Parse(strFloats[0]);
                    _prefabSlot.Add(index);
                    if (_prefab != null)
                    {
                        if (go == null)
                            go = GameObject.Instantiate(_prefab);
                        go.name = GetMonsterName(_id);
                        go.transform.position = pos;
                        go.transform.Rotate(0, rotateY, 0);

                        XEntityStatistics.RowData sData = XTableMgr.GetTable<XEntityStatistics>().GetByID((int)_uni_id);
                        if (sData == null)
                        {
                            XDebug.Log("enemy id not exist:", _uni_id);
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

        public void Remove()
        {
            GameObject.DestroyImmediate(go);
            _prefabSlot.Remove(index);
        }

        public void GenerateInstance()
        {
            if (_prefab == null) return;
            if (go != null) GameObject.DestroyImmediate(go);
            go = GameObject.Instantiate(_prefab);
            go.name = GetMonsterName(_id);
            go.transform.position = pos;
            go.transform.Rotate(0, rotateY, 0);
        }

    }

}