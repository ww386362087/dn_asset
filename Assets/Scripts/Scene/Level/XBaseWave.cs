using UnityEngine;

namespace Level
{
    public class BaseWave
    {
        public int _id;
        public float time;
        public int loopInterval;
        public string preWaves;
        public int yRotate;
        public bool repeat;
        public int randomID;
        public uint entityid;
        public float roundRidus;
        public string exString;
        public int roundCount;
        public string levelscript;
        public int index;
        public Vector3 pos;
        public float rotateY;
        public LevelSpawnType spawnType;
        protected InfoType infotype;


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
            infotype = PartType(data);
            string rawData = data.Substring(3);
            switch (infotype)
            {
                case InfoType.TypeId:
                    _id = int.Parse(rawData);
                    break;
                case InfoType.TypeSpawnType:
                    spawnType = (LevelSpawnType)(int.Parse(rawData));
                    break;
                case InfoType.TypeBaseInfo:
                    string[] strInfos = rawData.Split(',');
                    time = float.Parse(strInfos[0]);
                    loopInterval = int.Parse(strInfos[1]);
                    entityid = uint.Parse(strInfos[2]);
                    yRotate = int.Parse(strInfos[5]);
                    if (strInfos.Length > 6)
                        roundRidus = float.Parse(strInfos[6]);

                    if (strInfos.Length > 7)
                        roundCount = int.Parse(strInfos[7]);

                    if (strInfos.Length > 8)
                        randomID = int.Parse(strInfos[8]);

                    if (strInfos.Length > 11)
                        repeat = bool.Parse(strInfos[11]);
                    break;
                case InfoType.TypeExString:
                    exString = rawData;
                    break;
                case InfoType.TypePreWave:
                    preWaves = rawData;
                    break;
                case InfoType.TypeEditor:
                    break;
                case InfoType.TypeMonsterInfo:
                    string[] strFloats = rawData.Split(',');
                    index = int.Parse(strFloats[0]);
                    float x = float.Parse(strFloats[1]);
                    float y = float.Parse(strFloats[2]);
                    float z = float.Parse(strFloats[3]);
                    pos = new Vector3(x, y, z);
                    if (strFloats.Length > 4)
                        rotateY = float.Parse(strFloats[4]);
                    break;
                case InfoType.TypeScript:
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
