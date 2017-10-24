using UnityEngine;

namespace Level
{
    public class BaseWave
    {
        protected int _id;
        protected int index;
        protected int loopInterval;
        protected float radius;
        protected int count = 1;
        protected int yRotate;
        protected Vector3 pos;
        protected float rotateY;

        public string preWaves;
        public bool repeat;
        public int uid;
        public float time;
        public string exString;
        public string levelscript;
        public bool isAroundPlayer;
        public LevelSpawnType spawnType;
        protected InfoType infotype;
        
        public int ID
        {
            get { return _id; }
            set { _id = value; }
        }

        public float Radius
        {
            get { return radius; }
            set { radius = value; }
        }

        public int Count
        {
            get { return count; }
            set { count = value; }
        }

        public float Time
        {
            get { return time; }
            set { if (time != value) time = value; }
        }

        private InfoType PartType(string data)
        {
            InfoType type = InfoType.TypeNone;
            if (data.StartsWith("id")) type = InfoType.TypeId;
            else if (data.StartsWith("bi")) type = InfoType.BaseInfo;
            else if (data.StartsWith("pw")) type = InfoType.PreWave;
            else if (data.StartsWith("ei")) type = InfoType.EditorInfo;
            else if (data.StartsWith("ti")) type = InfoType.TransformInfo;
            else if (data.StartsWith("si")) type = InfoType.Script;
            else if (data.StartsWith("es")) type = InfoType.ExString;
            else if (data.StartsWith("st")) type = InfoType.SpawnType;
            return type;
        }

        protected virtual void ParseInfo(string data)
        {
            infotype = PartType(data);
            string rawData = data.Substring(3);
            switch (infotype)
            {
                case InfoType.TypeId:
                    _id = int.Parse(rawData);
                    break;
                case InfoType.SpawnType:
                    spawnType = (LevelSpawnType)(int.Parse(rawData));
                    break;
                case InfoType.BaseInfo:
                    string[] strInfos = rawData.Split(',');
                    time = float.Parse(strInfos[0]);
                    loopInterval = int.Parse(strInfos[1]);
                    uid = int.Parse(strInfos[2]);
                    yRotate = int.Parse(strInfos[5]);
                    if (strInfos.Length > 6)
                        radius = float.Parse(strInfos[6]);

                    if (strInfos.Length > 7)
                        count = int.Parse(strInfos[7]);

                    if (strInfos.Length > 8)
                        isAroundPlayer = bool.Parse(strInfos[8]);

                    if (strInfos.Length > 11)
                        repeat = bool.Parse(strInfos[11]);
                    break;
                case InfoType.ExString:
                    exString = rawData;
                    break;
                case InfoType.PreWave:
                    preWaves = rawData;
                    break;
                case InfoType.EditorInfo:
                    break;
                case InfoType.TransformInfo:
                    string[] strFloats = rawData.Split(',');
                    index = int.Parse(strFloats[0]);
                    float x = float.Parse(strFloats[1]);
                    float y = float.Parse(strFloats[2]);
                    float z = float.Parse(strFloats[3]);
                    pos = new Vector3(x, y, z);
                    rotateY = float.Parse(strFloats[4]);
                    break;
                case InfoType.Script:
                    strInfos = rawData.Split(',');
                    if (strInfos.Length > 0)
                        levelscript = strInfos[0];
                    break;
                default:
                    break;
            }
        }
    }

}
