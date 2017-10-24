using UnityEngine;
using UnityEditor;
using XTable;
using Level;

namespace XEditor
{
    public abstract class WaveWindow
    {
        public Rect _rect;
        public EditorWave _wave;
        protected abstract int height { get; }
        protected abstract string title { get; }

        protected static GUIContent RemoveWaveButtonContent = new GUIContent("X", "remove wave");

        private int width = 200;

        public WaveWindow(EditorWave wv)
        {
            _rect = new Rect(wv.rectX, wv.rectY, width, height);
            _wave = wv;
        }

        public void Draw()
        {
            string name = title + _wave.ID;
            _rect = GUI.Window(_wave.ID, _rect, DoWindow, name);
            _rect.height = height;
            _rect.x = Mathf.Clamp(_rect.x, 0, 3000);
            _rect.y = Mathf.Clamp(_rect.y, 0, 3000);
        }

        public virtual void DoWindow(int id)
        {
            if ((Event.current.button == 0) && (Event.current.type == EventType.MouseDown))
            {
                _wave.LevelMgr.CurrentEdit = id;
            }
        }

        public virtual void GenerateIcon() { }
    }

    public class ScriptWaveWindow : WaveWindow
    {
        public ScriptWaveWindow(EditorWave wv) : base(wv) { }

        protected override int height { get { return 45; } }

        protected override string title { get { return "script"; } }

        public override void DoWindow(int id)
        {
            base.DoWindow(id);
            GUILayout.BeginHorizontal();
            _wave.levelscript = EditorGUILayout.TextField(_wave.levelscript, new GUILayoutOption[] { GUILayout.Width(100), GUILayout.Height(16) });
            if (GUILayout.Button(RemoveWaveButtonContent, LevelLayout.miniButtonWidth))
            {
                _wave.LevelMgr.RemoveWave(_wave.ID);
            }
            _wave.repeat = GUILayout.Toggle(_wave.repeat, "repeat", new GUILayoutOption[] { GUILayout.Width(100), GUILayout.Height(16) });
            GUILayout.EndHorizontal();
            GUI.DragWindow();
        }
    }

    public class EntityWaveWindow : WaveWindow
    {
        protected override int height { get { return 110; } }
        protected override string title { get { return "wave"; } }

        protected Texture2D _icon = null;

        public EntityWaveWindow(EditorWave wv) : base(wv)
        {
        }
        
        public override void DoWindow(int id)
        {
            base.DoWindow(id);
            if (_wave.go != null) EditorGUIUtility.PingObject(_wave.go);
            if (_icon == null) GenerateIcon();
            GUILayout.BeginHorizontal();
            
            GUILayout.BeginVertical(new GUILayoutOption[] { GUILayout.Width(70), GUILayout.Height(100) });
            GUILayout.Box(_icon);

            GUILayout.BeginHorizontal();
            GUIStyle gs = new GUIStyle();
            gs.alignment = TextAnchor.LowerRight;
            gs.normal.textColor = Color.white;
            XEntityStatistics.RowData eData = XTableMgr.GetTable<XEntityStatistics>().GetByID((int)_wave.UID);
            if (eData != null && _wave.SpawnType == LevelSpawnType.Spawn_Monster)
            {
                if (eData.Type == 1)
                {
                    gs.normal.textColor = Color.red;
                }
                else if (eData.Type == 6)
                {
                    gs.normal.textColor = Color.yellow;
                }
            }
            if (_wave.levelscript != null && _wave.levelscript.Length > 0)
            {
                int fileNamePos = _wave.levelscript.LastIndexOf("/");
                GUILayout.Label(_wave.levelscript.Substring(fileNamePos + 1), gs);
            }
            else
            {
                GUILayout.Label(_wave.UID + "x" + _wave.Count, gs);
            }
            GUILayout.EndHorizontal();
            GUILayout.EndVertical();

            GUILayout.Box("", new GUILayoutOption[] { GUILayout.Width(1), GUILayout.Height(80) });

            GUILayout.BeginVertical();
            // spawn info
            string strSpawn = "";
            GUIStyle gsSpawn = new GUIStyle();
            gsSpawn.alignment = TextAnchor.LowerRight;

            if (_wave.preWaves != null && _wave.preWaves.Length > 0)
            {
                strSpawn = "Pre Wave:" + _wave.preWaves;
                gsSpawn.normal.textColor = Color.green;
            }
            if (_wave.exString != null && _wave.exString.Length > 0)
            {
                strSpawn = "\nES:" + _wave.exString;
                gsSpawn.normal.textColor = Color.green;
            }
            strSpawn += "\nTime: " + _wave.time;
            gsSpawn.normal.textColor = Color.white;

            GUILayout.Label(strSpawn, gs, new GUILayoutOption[] { GUILayout.Width(100) });

            GUILayout.BeginHorizontal(new GUILayoutOption[] { GUILayout.Height(30) });
            if (_wave.HasBuff)
            {
                Texture item = AssetDatabase.LoadAssetAtPath("Assets/Editor/LevelEditor/res/bx2.png", typeof(Texture)) as Texture;
                GUILayout.Box(item);
            }
            GUILayout.EndHorizontal();

            GUILayout.Box("", new GUILayoutOption[] { GUILayout.Width(100), GUILayout.Height(1) });

            // operation
            GUILayout.BeginHorizontal();
            if (GUILayout.Button(RemoveWaveButtonContent, LevelLayout.miniButtonWidth))
            {
                _wave.LevelMgr.RemoveWave(_wave.ID);
                _wave.RemoveSceneViewInstance();
            }
            GUILayout.EndHorizontal();
            GUILayout.EndVertical();

            GUILayout.EndHorizontal();
            

            GUI.DragWindow();
        }

        public override void GenerateIcon()
        {
            if (_wave.SpawnType == LevelSpawnType.Spawn_Role)
            {
                Texture icon = AssetDatabase.LoadAssetAtPath("Assets/Editor/LevelEditor/res/LevelPlayer.png", typeof(Texture)) as Texture;
                _icon = AssetPreview.GetAssetPreview(icon);
            }
            else if (_wave.SpawnType == LevelSpawnType.Spawn_Buff)
            {
                Texture icon = AssetDatabase.LoadAssetAtPath("Assets/Editor/LevelEditor/res/buff.png", typeof(Texture)) as Texture;
                _icon = AssetPreview.GetAssetPreview(icon);
            }
            else if (_wave.UID > 0)
            {
                _icon = AssetPreview.GetAssetPreview(_wave._prefab);
            }
            if (_icon != null) CompressTo64();
        }


        public void CompressTo64()
        {
            Color[] pix = _icon.GetPixels(32, 64, 64, 64);
            _icon = new Texture2D(64, 64);
            _icon.SetPixels(pix);
            _icon.Apply();
        }
        
    }


}