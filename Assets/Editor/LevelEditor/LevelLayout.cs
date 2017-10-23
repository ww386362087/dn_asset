using System.Linq;
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using Level;

namespace XEditor
{

    class LevelLayout
    {
        public SerializeLevel levelMgr;

        private static GUIContent AddWaveButtonContent = new GUIContent("add wave", "add wave");
        private static GUIContent AddScriptButtonContent = new GUIContent("add script", "add script");
        private static GUIContent EditLevelScriptButtonContent = new GUIContent("edit script", "edit script");
        private static GUIContent GenerateWallInfoButtonContent = new GUIContent("save wall info", "save wall info");
        private static GUIContent LoadWallInfoButtonContent = new GUIContent("load wall info", "load wall info");
        private static GUIContent SaveWaveButtonContent = new GUIContent("Save", "Save to file");
        private static GUIContent LoadWaveButtonContent = new GUIContent("Load", "Load from file");
        private static GUIContent ClearButtonContent = new GUIContent("Clear", "Clear");

        private static int tabLength = 420;
        private static int minViewHeight = 90;
        private static int maxViewHeight = 920;
        private static int minViewWidth = 5;
        private static int maxViewWidth = 1900;
        private Vector2 scrollPosition = Vector2.zero;

        public static GUILayoutOption miniButtonWidth = GUILayout.Width(20f);
        public static GUILayoutOption detailLayout = GUILayout.Width(tabLength);

        protected Texture2D _grayText = null;

        public LevelLayout(SerializeLevel mgr)
        {
            levelMgr = mgr;
            InitGrayTexture();
        }

        public void OnGUI()
        {
            GUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("current level : " + levelMgr.current_level);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            if (GUILayout.Button(AddWaveButtonContent, GUILayout.Width(100f)))
            {
                levelMgr.AddWave();
            }
            if (GUILayout.Button(AddScriptButtonContent, GUILayout.Width(100f)))
            {
                levelMgr.AddScript();
            }
            if (GUILayout.Button(ClearButtonContent, GUILayout.Width(100f)))
            {
                levelMgr.ClearWaves();
            }
            if (GUILayout.Button(EditLevelScriptButtonContent, GUILayout.Width(100f)))
            {
                levelMgr.OpenLevelScriptFile();
            }
            if (GUILayout.Button(GenerateWallInfoButtonContent, GUILayout.Width(150f)))
            {
                levelMgr.GenerateWallInfo();
            }
            if (GUILayout.Button(LoadWallInfoButtonContent, GUILayout.Width(150f)))
            {
                levelMgr.LoadWallInfo();
            }
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("TotalWave : " + levelMgr.WaveCount);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            if (GUILayout.Button(SaveWaveButtonContent))
            {
                levelMgr.SaveToFile();
            }
            if (GUILayout.Button(LoadWaveButtonContent))
            {
                levelMgr.LoadFromFile();
            }
            GUILayout.EndHorizontal();

            GUILayout.Box("", new GUILayoutOption[] { GUILayout.ExpandWidth(true), GUILayout.Height(1) });
            GUILayout.BeginHorizontal();
            DrawTab();
            scrollPosition = GUI.BeginScrollView(new Rect(tabLength + minViewWidth, minViewHeight, maxViewWidth, maxViewHeight), scrollPosition, new Rect(0, 0, 3000, 3000));
            levelMgr.Editor.BeginWindows();
            foreach (EditorWave _wave in levelMgr._waves) _wave.DrawWaveWindow();
            levelMgr.Editor.EndWindows();
            GUI.EndScrollView();
            GUILayout.EndHorizontal();

            DrawLinks();
        }

        public void InitGrayTexture()
        {
            _grayText = new Texture2D(128, 128, TextureFormat.ARGB32, true);
            _grayText.SetPixel(0, 1, Color.gray);
            _grayText.Apply();
        }

        private void DrawPreloadView()
        {
            Dictionary<uint, int> preloadTmp = new Dictionary<uint, int>();
            CalEnemyNum cen = new CalEnemyNum();
            Dictionary<uint, int> dic = cen.CalNum(levelMgr._waves);
            foreach (var item in dic)
            {
                int cur = item.Value;
                int last = 0;
                levelMgr._lastCachePreloadInfo.TryGetValue(item.Key, out last);
                if (last != cur)
                {
                    levelMgr._preLoadInfo[item.Key] = cur;
                }
            }
            foreach (var item in levelMgr._lastCachePreloadInfo)
            {
                int last = item.Value;
                int cur = 0;
                dic.TryGetValue(item.Key, out cur);
                if (last != cur)
                {
                    levelMgr._preLoadInfo[item.Key] = cur;
                }
            }

            GUIStyle style = new GUIStyle();
            EditorGUILayout.LabelField("Preload Monster : ");
            foreach (var item in levelMgr._preLoadInfo)
            {
                int preload = item.Value;
                int suggest = 0;
                dic.TryGetValue(item.Key, out suggest);
                if (preload > suggest || suggest == 0)
                {
                    style.normal.textColor = Color.red;
                }
                else if (preload < suggest)
                {
                    style.normal.textColor = Color.green;
                }
                else
                {
                    style.normal.textColor = Color.white;
                }

                GUILayout.BeginHorizontal();

                GUILayout.BeginVertical();
                EditorGUILayout.LabelField(string.Format("MonsterID: {0}   (推荐数量: {1})", item.Key, suggest), style);
                GUILayout.EndVertical();

                GUILayout.BeginVertical();
                preloadTmp[item.Key] = EditorGUILayout.IntField(preload);
                GUILayout.EndVertical();

                GUILayout.EndHorizontal();
                GUILayout.Box("", new GUILayoutOption[] { GUILayout.ExpandWidth(true), GUILayout.Height(1) });
            }

            levelMgr._preLoadInfo.Clear();
            foreach (var item in preloadTmp)
            {
                int suggest = 0;
                dic.TryGetValue(item.Key, out suggest);
                if (item.Value < 0)
                {
                    if (suggest != 0)
                    {
                        levelMgr._preLoadInfo[item.Key] = 0;
                    }
                    continue;
                }
                levelMgr._preLoadInfo[item.Key] = item.Value;
            }

            levelMgr._lastCachePreloadInfo = dic;
        }

        public void DrawTab()
        {
            GUILayout.BeginHorizontal(detailLayout);
            GUILayout.BeginVertical();
            if (levelMgr.CurrentEdit > -1)
            {
                GUILayout.Space(10);
                DrawDetailView();
            }
            GUILayout.Space(24);
            DrawPreloadView();
            GUILayout.EndHorizontal();
            GUILayout.EndHorizontal();
        }

        public void DrawDetailView()
        {
            EditorWave wv = levelMgr.GetWave(levelMgr.CurrentEdit);
            if (wv != null)
            {
                wv.SpawnType = (LevelSpawnType)EditorGUILayout.EnumPopup("Spawn Type", wv.SpawnType);
                wv.Time = EditorGUILayout.FloatField("Spawn Time(s):", wv.Time);

                GUILayout.Space(20);

                wv.exString = EditorGUILayout.TextField("ExString(,):", wv.exString);
                wv.preWaves = EditorGUILayout.TextField("PreWaves(,):", wv.preWaves);

                if (wv.spawnType == LevelSpawnType.Spawn_Buff)
                {
                    GUILayout.BeginHorizontal();
                    wv._buff_id = EditorGUILayout.TextField("Buff ID:", wv._buff_id);
                    wv._buff_percent = EditorGUILayout.IntSlider("Buff Percent(%):", wv._buff_percent, 0, 100);
                    GUILayout.EndHorizontal();
                }

                GUILayout.Space(20);
                GUILayout.BeginHorizontal();
                wv.isAroundPlayer = EditorGUILayout.Toggle("Around Player:",wv.isAroundPlayer);
                GUILayout.EndHorizontal();
                if (wv.isAroundPlayer)
                {
                    wv.RoundRidous = EditorGUILayout.FloatField("RoundRidous:", wv.RoundRidous);
                    wv.RoundCount = EditorGUILayout.IntSlider("RoundCount:", wv.RoundCount, 0, 10);
                }
                else
                {
                    if (wv.go != null)
                    {
                        Vector3 pos = wv.go.transform.position;
                        pos = EditorGUILayout.Vector3Field("Pos:", pos);
                        if (wv.go.transform.position != pos)
                            wv.go.transform.position = pos;
                    }
                }
            }
        }

        public void DrawLinks()
        {
            if (levelMgr.CurrentEdit > -1)
            {
                EditorWave _wave = levelMgr.GetWave(levelMgr.CurrentEdit);
                if (_wave == null) return;
                if (_wave.preWaves != null && _wave.preWaves.Length > 0)
                {
                    string[] strPreWaves = _wave.preWaves.Split(',');
                    for (int i = 0; i < strPreWaves.Length; i++)
                    {
                        int preWaveId;
                        bool b = int.TryParse(strPreWaves[i], out preWaveId);
                        if (b) curveFromTo(preWaveId, _wave.ID, Color.green, Color.yellow);
                    }
                }
                foreach (EditorWave wv in levelMgr._waves)
                {
                    if (wv.preWaves != null && wv.preWaves.Length > 0)
                    {
                        string[] strPreWaves = wv.preWaves.Split(',');
                        if (strPreWaves.Contains(levelMgr.CurrentEdit.ToString()))
                            curveFromTo(levelMgr.CurrentEdit, wv.ID, Color.green, Color.yellow);
                    }
                }
            }
        }

        protected void curveFromTo(int parentID, int childID, Color color, Color color2)
        {
            EditorWave parent = levelMgr.GetWave(parentID);
            EditorWave child = levelMgr.GetWave(childID);
            if (parent != null && child != null)
            {
                Rect parentRect = parent.LayoutWindow._rect;
                Rect childRect = child.LayoutWindow._rect;
                parentRect.x += tabLength;
                childRect.x += tabLength;
                curveFromTo(parentRect, childRect, color, color2);
            }
        }

        protected void curveFromTo(Rect from, Rect to, Color color, Color color2)
        {
            Vector2 offset = new Vector2(minViewWidth, minViewHeight);
            if (from.y + from.height <= to.y)
            {// up
                Drawing.bezierLine(
                new Vector2(from.x + from.width / 2, from.y + from.height) + offset - scrollPosition,
                new Vector2(from.x + from.width / 2, from.y + from.height + Mathf.Abs(to.y - (from.y + from.height))) + offset - scrollPosition,
                new Vector2(to.x + to.width / 2, to.y) + offset - scrollPosition,
                new Vector2(to.x + to.width / 2, to.y - Mathf.Abs(to.y - (from.y + from.height)) / 2) + offset - scrollPosition,
                color, color2, 2, true, 30);
            }
            else if (to.y + to.height <= from.y)
            {// down
                Drawing.bezierLine(
                    new Vector2(from.x + from.width / 2, from.y) + offset - scrollPosition,
                    new Vector2(from.x + from.width / 2, from.y - Mathf.Abs(from.y - (to.y + to.height)) / 2) + offset - scrollPosition,

                    new Vector2(to.x + to.width / 2, to.y + to.height) + offset - scrollPosition,
                    new Vector2(to.x + to.width / 2, to.y + to.height + Mathf.Abs(to.y - (to.y + to.height))) + offset - scrollPosition,

                    color, color2, 2, true, 30);
            }
            else if (from.x + from.width <= to.x)
            {// left
                Drawing.bezierLine(
                    new Vector2(from.x + from.width, from.y + from.height / 2) + offset - scrollPosition,
                    new Vector2(from.x + from.width + Mathf.Abs(to.x - (from.x + from.width)), from.y + from.height / 2) + offset - scrollPosition,

                    new Vector2(to.x, to.y + to.height / 2) + offset - scrollPosition,
                    new Vector2(to.x - Mathf.Abs(to.x - (from.x + from.width)), to.y + to.height / 2) + offset - scrollPosition,

                    color, color2, 2, true, 30);
            }
            else if (to.x + to.width <= from.x)
            {// right
                Drawing.bezierLine(
                    new Vector2(from.x, from.y + from.height / 2) + offset - scrollPosition,
                    new Vector2(from.x - Mathf.Abs(from.x - (to.x + to.width)), from.y + from.height / 2) + offset - scrollPosition,

                    new Vector2(to.x + to.width, to.y + to.height / 2) + offset - scrollPosition,
                    new Vector2(to.x + to.width + Mathf.Abs(from.x - (to.x + to.width)), to.y + to.height / 2) + offset - scrollPosition,

                    color, color2, 2, true, 30);
            }
            else
            {// overlap
                Drawing.bezierLine(
                    new Vector2(from.x, from.y) + offset - scrollPosition,
                    new Vector2(from.x, from.y) + offset - scrollPosition,

                    new Vector2(to.x, to.y) + offset - scrollPosition,
                    new Vector2(to.x, to.y) + offset - scrollPosition,

                    color, color2, 2, true, 10);
            }
        }

    }
}