using UnityEngine;
using UnityEditor;
using System;
using System.Collections.Generic;
using System.IO;
using XTable;

namespace XEditor
{

    [Serializable]
    public class SerializeLevel : ScriptableObject
    {
        [SerializeField]
        public List<EditorWave> _waves;
        [NonSerialized]
        private EditorWave _toBeRemoved;
        [SerializeField]
        private XEntityStatistics _data_info = null;
        [NonSerialized]
        private LevelLayout _layout;
        
        public static int maxSpawnTime = 180;

        private float _markGOHeight;
        private float _goStep = 0.0001f;
        public string current_level = "";
        
        private LevelEditor _editor;
        public LevelEditor Editor
        {
            get { return _editor; }
            set { _editor = value; }
        }

        public int WaveCount
        {
            get { return _waves.Count; }
        }

        public XEntityStatistics EntityList
        {
            get { return _data_info; }
        }
        
        int _currentEdit = -1;
        public int CurrentEdit
        {
            get { return _currentEdit; }
            set { _currentEdit = value; }
        }

        public void OnEnable()
        {
            hideFlags = HideFlags.HideAndDontSave;
            AssetPreview.SetPreviewTextureCacheSize(64);
            if (_waves == null) _waves = new List<EditorWave>();
            if (_layout == null) _layout = new LevelLayout(this);
            if (_data_info == null) _data_info = XTableMgr.GetTable<XEntityStatistics>();
            _currentEdit = -1;
            _markGOHeight = 2.0f;
        }
        

        public void OnGUI()
        {
            _layout.OnGUI();
            if (_toBeRemoved != null)
            {
                if (_toBeRemoved.ID == CurrentEdit) CurrentEdit = -1;
                _waves.Remove(_toBeRemoved);
                _toBeRemoved = null;
            }
        }

        public void Update()
        {
            float minh = 1.8f;
            float maxh = 2.2f;
            _markGOHeight += _goStep;
            if (_markGOHeight > maxh)
            {
                _markGOHeight = maxh;
                _goStep = -_goStep;
            }
            else if (_markGOHeight < minh)
            {
                _markGOHeight = minh;
                _goStep = -_goStep;
            }
        }

        public void SaveToFile()
        {
            string path = EditorUtility.SaveFilePanel("Select a file to save", XEditorLibrary.Lev, "temp.txt", "txt");
            SaveToFile(path, false);
        }

        public void SaveToFile(string path, bool bAutoSave)
        {
            SavePreprocess();
            StreamWriter sw = File.CreateText(path);
            sw.WriteLine("{0}", _waves.Count);

            LevelEntityStatistics.CulWaves(_waves);

            int preLoadCount = LevelEntityStatistics.suggest.Count;
            sw.WriteLine("{0}", preLoadCount);

            foreach (var item in LevelEntityStatistics.suggest)
            {
                if (item.Value > 0)
                {
                    sw.WriteLine("pi:" + item.Key + "," + item.Value);
                }
            }
            foreach (EditorWave _wave in _waves)
            {
                _wave.WriteToFile(sw);
            }
            sw.Flush();
            sw.Close();
            AssetDatabase.Refresh();
        }

        public void SavePreprocess()
        {
            List<EditorWave> _wavesToRemove = new List<EditorWave>();
            foreach (EditorWave _wave in _waves)
            {
                if (!_wave.ValidWave())
                    _wavesToRemove.Add(_wave);
            }
            foreach (EditorWave _remove in _wavesToRemove)
            {
                _waves.Remove(_remove);
            }
        }

        public void LoadFromFile()
        {
            current_level = "";
            string path = EditorUtility.OpenFilePanel("Select a file to load", XEditorLibrary.Lev, "txt");
            RemoveSceneViewInstance();
            _waves.Clear();
            _currentEdit = -1;
            int v = path.LastIndexOf("Level/");
            int dot = path.LastIndexOf(".");
            current_level = path.Substring(v + 6, dot - v - 6);
            LoadFromFile(path);
            LevelEntityStatistics.CulWaves(_waves);
        }

        public void LoadFromFile(string path)
        {
            if (!File.Exists(path)) return;
            StreamReader sr = File.OpenText(path);
            string line = sr.ReadLine();
            int totalWave = int.Parse(line);
            line = sr.ReadLine();
            int PreloadWave = int.Parse(line);
            for (int i = 0; i < PreloadWave; i++)
            {
                line = sr.ReadLine();
            }
            for (int id = 0; id < totalWave; id++)
            {
                EditorWave newWave = new EditorWave(this, sr);
                _waves.Add(newWave);
                CurrentEdit = -1;
            }
            sr.Close();
            _editor.Repaint();
        }

        public void ClearWaves()
        {
            RemoveSceneViewInstance();
            _waves.Clear();
            _editor.Repaint();
        }

        public void OpenLevelScriptFile()
        {
            if (string.IsNullOrEmpty(current_level)) return;
            System.Diagnostics.Process.Start("notepad.exe", "./" + XEditorLibrary.Lev + current_level + "_sc.txt");
        }

        public void LoadWallInfo()
        {
            if (string.IsNullOrEmpty(current_level)) return;
            string fileName = "./" + XEditorLibrary.Lev + current_level + "_sc.txt";
            if (!File.Exists(fileName)) return;
            string content = File.ReadAllText(fileName);
            string[] commands = content.Split(new char[] { '\n' });
            GameObject dynamic = GameObject.Find("DynamicScene");
            if (dynamic == null) return;
            for (int i = 0; i < commands.Length; i++)
            {
                if (commands[i].StartsWith("info"))
                {
                    if (commands[i][commands[i].Length - 1] == '\r')
                        commands[i] = commands[i].Substring(0, commands[i].Length - 1);

                    string[] s = commands[i].Split(' ');
                    string wallName = s[0].Substring(5);
                    Transform t = XCommon.singleton.FindChildRecursively(dynamic.transform, wallName);
                    if (t != null)
                    {
                        string[] sp = s[1].Split('|');
                        t.position = new Vector3(float.Parse(sp[0]), float.Parse(sp[1]), float.Parse(sp[2]));
                        t.rotation = Quaternion.Euler(new Vector3(t.rotation.eulerAngles.x, float.Parse(sp[3]), t.rotation.eulerAngles.z));

                        string strState = s[2];
                        t.gameObject.SetActive(strState == "on");

                        if (s.Length > 5)
                        {
                            string strBoxX = s[3];
                            string strBoxY = s[4];
                            string strBoxZ = s[5];
                            BoxCollider c = t.GetComponent<BoxCollider>();
                            if (c != null)
                            {
                                float x = float.Parse(strBoxX) / t.localScale.x;
                                float y = (float.Parse(strBoxY) - t.position.y) / t.localScale.y * 2;
                                float z = float.Parse(strBoxZ) / t.localScale.z;
                                c.size = new Vector3(x, y, z);
                            }
                        }
                        if (s.Length > 6)
                        {
                            string strHorseIndex = s[6];
                            XHorseWall horseWall = t.GetComponent<XHorseWall>();
                            horseWall.index = int.Parse(strHorseIndex);
                        }
                    }
                }
            }
            UnityEditor.SceneManagement.EditorSceneManager.SaveScene(UnityEditor.SceneManagement.EditorSceneManager.GetActiveScene());
        }

        public void GenerateWallInfo()
        {
            if (string.IsNullOrEmpty(current_level)) return;
            string fileName = "./" + XEditorLibrary.Lev + current_level + "_sc.txt";
            if (!File.Exists(fileName)) return;
            string content = File.ReadAllText(fileName);
            string final = "";
            string[] commands = content.Split(new char[] { '\n' });
            string append = "";
            bool HasPreInfo = false;
            List<string> RecordWalls = new List<string>();
            for (int i = 0; i < commands.Length; i++)
            {
                if (commands[i].StartsWith("info"))
                {
                    HasPreInfo = true;
                    continue;
                }
                if (!commands[i].StartsWith("opendoor"))
                {
                    final += commands[i] + '\n';
                    continue;
                }
                if (commands[i][commands[i].Length - 1] == '\r')
                    commands[i] = commands[i].Substring(0, commands[i].Length - 1);

                string[] s = commands[i].Split(' ');
                s = commands[i].Split(' ');
                string wallName = s[1];
                GameObject dynamic = GameObject.Find("DynamicScene");
                if (dynamic != null)
                {
                    Transform t = XCommon.singleton.FindChildRecursively(dynamic.transform, wallName);
                    if (t != null)
                    {
                        XSpawnWall spawnWall = t.GetComponent<XSpawnWall>();
                        XTransferWall transferWall = t.GetComponent<XTransferWall>();
                        XHorseWall horseWall = t.GetComponent<XHorseWall>();
                        if (spawnWall != null || transferWall != null)
                        {
                            final += commands[i] + "\r\n";
                            continue;
                        }
                        if (RecordWalls.Contains(wallName))
                        {
                            final += commands[i] + "\r\n";
                            continue;
                        }

                        RecordWalls.Add(wallName);
                        Vector3 pos = t.position;
                        float r = t.rotation.eulerAngles.y;
                        append += ("info:" + wallName);
                        append += " ";
                        append += pos.x;
                        append += "|";
                        append += pos.y;
                        append += "|";
                        append += pos.z;
                        append += "|";
                        append += r;
                        if (t.gameObject.activeInHierarchy)
                            append += " on";
                        else
                            append += " off";

                        BoxCollider c = t.GetComponent<BoxCollider>();
                        if (c != null)
                        {
                            append += " " + (c.size.x * t.localScale.x);
                            append += " " + (t.position.y + c.size.y / 2 * t.localScale.y);
                            append += " " + (c.size.z * t.localScale.z);
                        }
                        if (horseWall != null)
                        {
                            append += " " + horseWall.index;
                        }
                        append += "\r\n";
                    }
                }
                commands[i] += '\r';
                final += commands[i] + '\n';
            }
            if (!HasPreInfo) final += "\r\n";
            final += append;
            File.WriteAllText(fileName, final);
        }

        public void RemoveSceneViewInstance()
        {
            foreach (EditorWave _wave in _waves)
            {
                _wave.RemoveSceneViewInstance();
            }
        }

        public void AddWave()
        {
            int newid = GetEmptySlot(0, 100);
            if (newid >= 0)
            {
                EditorWave newWave = new EditorWave(newid);
                newWave.ID = newid;
                newWave.LevelMgr = this;
                _waves.Add(newWave);
                CurrentEdit = newWave.ID;
            }
            else
            {
                XDebug.Log("More than 100 waves?");
            }
        }

        public void AddScript()
        {
            int newid = GetEmptySlot(1000, 1100);
            if (newid >= 0)
            {
                EditorWave newWave = new EditorWave(newid);
                newWave.ID = newid;
                newWave.LevelMgr = this;
                _waves.Add(newWave);
                CurrentEdit = newWave.ID;
            }
        }

        protected int GetEmptySlot(int startIndex, int endIndex)
        {
            for (int i = startIndex; i < endIndex; i++)
            {
                bool bOccupied = false;
                foreach (EditorWave _wave in _waves)
                {
                    if (_wave.ID == i)
                    {
                        bOccupied = true;
                        break;
                    }
                }
                if (!bOccupied) return i;
            }
            return -1;
        }

        public void RemoveWave(int id)
        {
            foreach (EditorWave _wave in _waves)
            {
                if (_wave.ID == id)
                {
                    _toBeRemoved = _wave;
                }
            }
        }

        public EditorWave GetWave(int id)
        {
            foreach (EditorWave _wave in _waves)
            {
                if (_wave.ID == id)
                {
                    return _wave;
                }
            }
            return null;
        }


        public void RemoveMonster(GameObject go)
        {
            string[] wave_info = go.name.Replace("Wave", "").Replace("monster", "").Split('_');
            int wave = int.Parse(wave_info[0]);

            foreach (EditorWave _wave in _waves)
            {
                if (_wave.ID == wave)
                {
                    _wave.RemoveMonster(go);
                }
            }
        }
    }


}