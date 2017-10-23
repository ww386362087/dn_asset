using UnityEngine;


namespace Level
{
    public class BaseWave
    {
        public int _id;
        public float _time;
        public int _loopInterval;
        public string _preWaves;
        public int _preWavePercent;
        public int _yRotate;
        public bool _repeat;
        public int _randomID;
        public uint _entityid;
        public float _roundRidus;
        public string _exString;
        public int _roundCount;
        public string _levelscript;
        public int _index;
        public Vector3 _pos;
        public float _rotateY;
        public LevelSpawnType _spawnType;
        protected InfoType _infotype;


        private InfoType PartType(string data)
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
            return type;
        }

        protected virtual void ParseInfo(string data)
        {
            _infotype = PartType(data);
            string rawData = data.Substring(3);
            switch (_infotype)
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
                    _entityid = uint.Parse(strInfos[2]);
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
                        _preWaves = strInfos[0];
                    if (strInfos.Length > 1)
                        _preWavePercent = int.Parse(strInfos[1]);
                    break;
                case InfoType.TypeEditor:
                    break;
                case InfoType.TypeMonsterInfo:
                    string[] strFloats = rawData.Split(',');
                    _index = int.Parse(strFloats[0]);
                    float x = float.Parse(strFloats[1]);
                    float y = float.Parse(strFloats[2]);
                    float z = float.Parse(strFloats[3]);
                    _pos = new Vector3(x, y, z);
                    if (strFloats.Length > 4)
                        _rotateY = float.Parse(strFloats[4]);
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
    }

}
