using UnityEngine;
using UnityEditor;
using XTable;

public class WaveWindow
{
    public Rect _rect;
    public LevelWave _wave;

    public static int width = 200;
    public static int height = 120;
    public static int height2 = 45;


    protected Texture2D _icon64 = null;

    private static GUIContent
        RemoveWaveButtonContent = new GUIContent("X", "remove wave");

    private static GUIContent
        ToggleWaveButtonContent = new GUIContent("V", "toggle visible");

    private static GUIContent
        SelectEnemyButtonContent = new GUIContent("S", "select enemy");

    public WaveWindow(LevelWave _wv)
    {
        int ht = (_wv._id >= 1000 ? height2 : height);
        _rect = new Rect(0, LevelLayout.minViewHeight, width, ht);
        _wave = _wv;
    }

    public void draw()
    {
        string name = (_wave._id >= 1000 ? "script " : "wave ") + _wave._id;
        _rect = GUI.Window(_wave._id, _rect, doWindow, name);

        int ht = (_wave._id >= 1000 ? height2 : height);
        _rect.height = ht;
        _rect.x = Mathf.Clamp(_rect.x, 0, 3000);
        _rect.y = Mathf.Clamp(_rect.y, 0, 3000);
    }

    public void doWindow(int id)
    {
        if ((Event.current.button == 0) && (Event.current.type == EventType.MouseDown))
        {
            _wave.LevelMgr.CurrentEdit = id;
        }
        if (_wave._id < 1000)//wave
        {
            if (_icon64 == null) RegenerateIcon64();
            GUILayout.BeginHorizontal();

            // icon & number
            GUILayout.BeginVertical(new GUILayoutOption[] { GUILayout.Width(70), GUILayout.Height(100) });
            GUILayout.Box(_icon64);

            GUILayout.BeginHorizontal();
            if (GUILayout.Button(SelectEnemyButtonContent, LevelLayout.miniButtonWidth))
            {
                _wave.LevelMgr.CurrentEdit = id;
                OpenEnemyListWindow();
            }
           
            GUIStyle gs = new GUIStyle();
            gs.alignment = TextAnchor.LowerRight;
            gs.normal.textColor = Color.white;
            XEntityStatistics.RowData enemyData = XEntityStatistics.sington.GetByID((int)_wave.EnemyID);
            if (enemyData != null && _wave.SpawnType == LevelWave.LevelSpawnType.Spawn_Source_Monster)
            {
                if (enemyData.Type == 1)
                {
                    gs.normal.textColor = Color.red;
                }
                else if (enemyData.Type == 6)
                {
                    gs.normal.textColor = Color.yellow;
                }
            }
            if (_wave._script != null && _wave._script.Length > 0)
            {
                int fileNamePos = _wave._script.LastIndexOf("/");
                GUILayout.Label(_wave._script.Substring(fileNamePos + 1), gs);
            }
            else
            {
                GUILayout.Label(_wave.EnemyID + "x" + (_wave._prefabSlot.Count + _wave.RoundCount), gs);
            }
            GUILayout.EndHorizontal();
            GUILayout.EndVertical();

            GUILayout.Box("", new GUILayoutOption[] { GUILayout.Width(1), GUILayout.Height(80) });

            GUILayout.BeginVertical();
            // spawn info
            string strSpawn = "";
            GUIStyle gsSpawn = new GUIStyle();
            gsSpawn.alignment = TextAnchor.LowerRight;

            if (_wave._preWaves != null && _wave._preWaves.Length > 0)
            {
                strSpawn = "After Wave:" + _wave._preWaves;
                gsSpawn.normal.textColor = Color.green;
            }
            if (_wave._exString != null && _wave._exString.Length > 0)
            {
                strSpawn = "\nES:" + _wave._exString;
                gsSpawn.normal.textColor = Color.green;
            }
            strSpawn += "\nTime: " + _wave._time;
            gsSpawn.normal.textColor = Color.white;

            GUILayout.Label(strSpawn, gs, new GUILayoutOption[] { GUILayout.Width(100) });

            GUILayout.BeginHorizontal(new GUILayoutOption[] { GUILayout.Height(30) });
            if (_wave.HasDoodad) 
            {
                Texture item = AssetDatabase.LoadAssetAtPath("Assets/Editor/LevelEditor/res/bx2.png", typeof(Texture)) as Texture;
                GUILayout.Box(item);
            }
            GUILayout.EndHorizontal();

            GUILayout.Box("", new GUILayoutOption[] { GUILayout.Width(100), GUILayout.Height(1) });

            // operation
            GUILayout.BeginHorizontal();
            if (GUILayout.Button(ToggleWaveButtonContent, LevelLayout.miniButtonWidth))
            {
                _wave.VisibleInEditor = !_wave.VisibleInEditor;
            }

            if (GUILayout.Button(RemoveWaveButtonContent, LevelLayout.miniButtonWidth))
            {
                _wave.LevelMgr.RemoveWave(_wave._id);
                _wave.RemoveSceneViewInstance();
            }
            GUILayout.EndHorizontal();
            GUILayout.EndVertical();

            GUILayout.EndHorizontal();
        }
        else //script
        {
            GUILayout.BeginHorizontal();
            _wave._script = EditorGUILayout.TextField(_wave._script, new GUILayoutOption[] { GUILayout.Width(100), GUILayout.Height(16) });
            if (GUILayout.Button(RemoveWaveButtonContent, LevelLayout.miniButtonWidth))
            {
                _wave.LevelMgr.RemoveWave(_wave._id);
            }
            _wave._repeat = GUILayout.Toggle(_wave._repeat, "repeat", new GUILayoutOption[] { GUILayout.Width(100), GUILayout.Height(16) });
            GUILayout.EndHorizontal();
        }

        GUI.DragWindow();
    }

    public static void Compress128To64(Texture2D src, Texture2D dst)
    {
        int x = 32;
        int y = 32;
        int width = 64;
        int height = 64;

        if (src == null) return;

        Color[] pix = src.GetPixels(x, y, width, height);
        dst.SetPixels(pix);
        dst.Apply();
    }

    public void RegenerateIcon64()
    {
        _icon64 = null;
        if (_wave.SpawnType == LevelWave.LevelSpawnType.Spawn_Source_Player)
        {
            Texture icon = AssetDatabase.LoadAssetAtPath("Assets/Editor/LevelEditor/res/LevelPlayer.png", typeof(Texture)) as Texture;
            Texture2D icon128 = AssetPreview.GetAssetPreview(icon);

            if (icon128 != null)
            {
                _icon64 = new Texture2D(64, 64);
                Compress128To64(icon128, _icon64);
            }
        }
        else if (_wave.SpawnType == LevelWave.LevelSpawnType.Spawn_Source_Random)
        {
            Texture icon = AssetDatabase.LoadAssetAtPath("Assets/Editor/LevelEditor/res/LevelRandom.png", typeof(Texture)) as Texture;
            Texture2D icon128 = AssetPreview.GetAssetPreview(icon);

            if (icon128 != null)
            {
                _icon64 = new Texture2D(64, 64);
                Compress128To64(icon128, _icon64);
            }
        }
        else if (_wave.SpawnType == LevelWave.LevelSpawnType.Spawn_Source_Buff)
        {
            Texture icon = AssetDatabase.LoadAssetAtPath("Assets/Editor/LevelEditor/res/buff.png", typeof(Texture)) as Texture;
            Texture2D icon128 = AssetPreview.GetAssetPreview(icon);

            if (icon128 != null)
            {
                _icon64 = new Texture2D(64, 64);
                Compress128To64(icon128, _icon64);
            }
        }
        else if (_wave.EnemyID > 0)
        {
            Texture2D icon128 = AssetPreview.GetAssetPreview(_wave._prefab);
            if (icon128 != null)
            {
                _icon64 = new Texture2D(64, 64);
                Compress128To64(icon128, _icon64);
            }
        }
    }

    protected void OpenEnemyListWindow()
    {
        EnemyListEditor _window = (EnemyListEditor)EditorWindow.GetWindow(typeof(EnemyListEditor));
        _window.Show();
    }
}

