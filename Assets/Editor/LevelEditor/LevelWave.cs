using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using XTable;

[Serializable]
public class LevelWave : ScriptableObject
{
    public enum InfoType
    {
        TypeNone,
        TypeId,
        TypeBaseInfo,
        TypePreWave,
        TypeEditor,
        TypeMonsterInfo,
        TypeScript,
        TypeExString,
        TypeSpawnType,
    }

    public enum LevelSpawnType
    {
        Spawn_Source_Monster,
        Spawn_Source_Player,
        Spawn_Source_Random,
        Spawn_Source_Buff,
    }

    [SerializeField]
    public int _id;
    [SerializeField]
    public LevelSpawnType _spawnType;
    [SerializeField]
    public float _time;
    [SerializeField]
    public int _loopInterval;
    [SerializeField]
    public uint _enemyid;
    [SerializeField]
    public int _randomid;
    [SerializeField]
    public string _script;
    [SerializeField]
    public bool _repeat;
    [SerializeField]
    public GameObject _prefab;
    [SerializeField]
    public int _yRotate;
    [SerializeField]
    public string _preWaves;
    [SerializeField]
    public int _preWavePercent;
    [SerializeField]
    public string _doodad_id;
    [SerializeField]
    public int _doodad_percent;
    [SerializeField]
    public string _exString;
    [SerializeField]
    public List<int> _prefabSlot;
    [SerializeField]
    public bool _bVisibleInEditor = true;
    [SerializeField]
    public float _roundRidus;
    [SerializeField]
    public int _roundCount;
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
                _enemyid = 0;
                if (_window != null) _window.RegenerateIcon64();
            }
            if (_spawnType == LevelSpawnType.Spawn_Source_Random)
            {
                _prefab = Resources.Load("Prefabs/NPC_Velskud_Wing") as GameObject;
                _enemyid = 0;
                if (_window != null)
                    _window.RegenerateIcon64();
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

    public uint EnemyID
    {
        get { return _enemyid; }
        set
        {
            if (_enemyid != value)
            {
                if (value > 0)
                {
                    _enemyid = value;
                    _script = null;
                    if (SpawnType == LevelSpawnType.Spawn_Source_Monster)
                    {
                        XEntityStatistics.RowData npcInfo = LevelMgr.EnemyList.GetByID((int)_enemyid);
                        if (npcInfo == null) return;
                        _prefab = Resources.Load("Prefabs/" + XEntityPresentation.sington.GetItemID((uint)npcInfo.PresentID).Prefab) as GameObject;
                        if (_window != null) _window.RegenerateIcon64();
                    }
                    if (SpawnType == LevelSpawnType.Spawn_Source_Buff)
                    {
                        _prefab = Resources.Load("Effects/FX_Particle/Roles/Lzg_Ty/Ty_buff_jx_m") as GameObject;
                        if (_window != null) _window.RegenerateIcon64();
                    }
                }
            }
        }
    }

    public string LevelScript
    {
        get { return _script; }
        set
        {
            if (_script != value)
            {
                _script = value;
                _enemyid = 0;
                if (_window != null)
                    _window.RegenerateIcon64();
            }
        }
    }

    public int Rotation
    {
        get { return _yRotate; }
        set
        {
            _yRotate = value;
            SetInstanceFace();
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
        get { return _randomid; }
        set { _randomid = value; }
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
    public void OnEnable()
    {
        hideFlags = HideFlags.HideAndDontSave;
        if (_prefabSlot == null)
            _prefabSlot = new List<int>();
        if (_window == null)
            _window = new WaveWindow(this);
    }

    public bool ValidWave()
    {
        if (_script != null && _script.Length > 0) return true;
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
            sw.WriteLine("bi:{0},{1},{2},{3},{4},{5},{6},{7},{8},{9},{10},{11}", _time, _loopInterval, _enemyid, _prefabSlot.Count, _bVisibleInEditor, 0, _roundRidus, _roundCount, _randomid, _doodad_id, _doodad_percent, _repeat);
            if (_script != null && _script.Length > 0) sw.WriteLine("si:{0}", _script);

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
                    sw.WriteLine("mi:{0},{1},{2},{3},{4}", index,
                        go.transform.position.x, go.transform.position.y, go.transform.position.z,
                         go.transform.localRotation.eulerAngles.y);
                }
            }
            sw.WriteLine("ew");
        }
    }

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
                {
                    _id = int.Parse(rawData);
                }
                break;
            case InfoType.TypeSpawnType:
                {
                    SpawnType = (LevelSpawnType)(int.Parse(rawData));
                }
                break;
            case InfoType.TypeBaseInfo:
                {
                    string[] strInfos = rawData.Split(',');
                    _time = float.Parse(strInfos[0]);
                    _loopInterval = int.Parse(strInfos[1]);
                    EnemyID = uint.Parse(strInfos[2]);
                    _bVisibleInEditor = bool.Parse(strInfos[4]);
                    _yRotate = int.Parse(strInfos[5]);

                    if (strInfos.Length > 6)
                        _roundRidus = float.Parse(strInfos[6]);
                    if (strInfos.Length > 7)
                        _roundCount = int.Parse(strInfos[7]);
                    if (strInfos.Length > 8)
                        _randomid = int.Parse(strInfos[8]);
                    if (strInfos.Length > 9)
                        _doodad_id = strInfos[9];
                    if (strInfos.Length > 10)
                        _doodad_percent = int.Parse(strInfos[10]);
                    if (strInfos.Length > 11)
                        _repeat = bool.Parse(strInfos[11]);
                }
                break;
            case InfoType.TypePreWave:
                {
                    string[] strInfos = rawData.Split('|');
                    if (strInfos.Length > 0)
                        _preWaves = strInfos[0];
                    if (strInfos.Length > 1)
                        _preWavePercent = int.Parse(strInfos[1]);
                }
                break;
            case InfoType.TypeExString:
                {
                    string[] strInfos = rawData.Split('|');
                    if (strInfos.Length > 0) _exString = strInfos[0];
                }
                break;
            case InfoType.TypeEditor:
                {
                    string strEditorData = rawData;
                    if (strEditorData.Length > 0)
                    {
                        string[] strEditorCoords = strEditorData.Split(',');
                        if (_window != null)
                        {
                            _window._rect.x = int.Parse(strEditorCoords[0]);
                            _window._rect.y = int.Parse(strEditorCoords[1]);
                        }
                    }
                }
                break;
            case InfoType.TypeMonsterInfo:
                {
                    string[] strFloats = rawData.Split(',');
                    int index = int.Parse(strFloats[0]);
                    _prefabSlot.Add(index);

                    // generate gameobject in scene
                    float x = float.Parse(strFloats[1]);
                    float y = float.Parse(strFloats[2]);
                    float z = float.Parse(strFloats[3]);
                    Vector3 pos = new Vector3(x, y, z);

                    float rotateY = 0;
                    if (strFloats.Length > 4)
                    {
                        rotateY = float.Parse(strFloats[4]);
                    }
                    if (_prefab != null)
                    {
                        GameObject go = Instantiate(_prefab);
                        go.name = GetMonsterName(_id, index);
                        go.transform.position = pos;
                        go.transform.Rotate(0, rotateY, 0);

                        XEntityStatistics.RowData sData = XEntityStatistics.sington.GetByID((int)_enemyid);
                        if (sData == null)
                        {
                            Debug.Log("enemy id not exist:" + _enemyid);
                            break;
                        }
                        if (sData.PresentID > 0)
                        {
                            XEntityPresentation.RowData pData = XEntityPresentation.sington.GetItemID((uint)sData.PresentID);
                            go.transform.localScale = Vector3.one * pData.Scale;
                        }
                    }
                }
                break;
            case InfoType.TypeScript:
                {
                    string[] strInfos = rawData.Split(',');
                    if (strInfos.Length > 0) _script = strInfos[0];
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
            if (go != null)
            {
                DestroyImmediate(go);
            }
        }
    }

    public int FindEmptySlot()
    {
        for (int i = 0; i < 100; i++)
        {
            if (!_prefabSlot.Contains(i))
                return i;
        }
        return 0;
    }

    public int GetMaxSlot()
    {
        int max = 0;
        foreach (int i in _prefabSlot)
            if (i > max) max = i;
        return max;
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
        GameObject markgo = Instantiate(mark) as GameObject;
        markgo.transform.parent = monster.transform;
        markgo.transform.localPosition = new Vector3(0, LevelMgr.MarkHeight, 0);
        markgo.transform.localRotation = new Quaternion(0, 90, 90, 0);
        markgo.transform.localScale = Vector3.one;

        ParticleSystem[] systems = markgo.GetComponentsInChildren<ParticleSystem>();
        foreach (ParticleSystem system in systems) system.Play();
    }

    public void SetWaveMarked(GameObject markPrefab)
    {
        List<int> toberemove = new List<int>();
        foreach (int i in _prefabSlot)
        {
            string name = GetMonsterName(_id, i);
            GameObject go = GameObject.Find(name);
            if (go == null)
            {
                // go may be deleted 
                toberemove.Add(i);
            }
            else
            {
                MarkMonster(go, markPrefab);
            }
        }
        foreach (int j in toberemove)
        {
            _prefabSlot.Remove(j);
        }
    }


    

    public void RemoveMonster(GameObject go)
    {
        int wave = 0;
        int index = 0;
        ExtractInfoFromName(go.name, out wave, out index);
        if (wave == _id)
        {
            DestroyImmediate(go);
            _prefabSlot.Remove(index);
        }
    }

}
