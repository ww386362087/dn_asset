using UnityEngine;
using UnityEditor;
using XTable;
using Level;

namespace XEditor
{
    public class EnemyListEditor : EditorWindow
    {

        private XEntityStatistics _data_info = null;

        public static int width = 100;
        public static int height = 45;

        private static GUIContent SearchButtonContent = new GUIContent("Search", "Search");

        private static GUIContent BuffButtonContent = new GUIContent("Buff", "Buff");

        private string _searchText = "";
        private string _buff = "";

        public void OnEnable()
        {
            LevelEditor _levelInstance = (LevelEditor)GetWindow(typeof(LevelEditor));
            if (_levelInstance == null) return;
            _data_info = _levelInstance.LevelMgr.EnemyList;
        }
        

        private void OnGUI()
        {
            int col = 11;
            int cnt = 1200;

            GUILayout.BeginHorizontal();
            _searchText = GUILayout.TextField(_searchText, GUILayout.Width(120f));
            if (GUILayout.Button(SearchButtonContent, GUILayout.Width(120f))) OnSearch();
            GUILayout.EndHorizontal();

            GUILayout.Space(20);

            GUILayout.BeginHorizontal();
            _buff = GUILayout.TextField(_buff, GUILayout.Width(120f));
            if (GUILayout.Button(BuffButtonContent, GUILayout.Width(120f))) OnSelectBuff();
            GUILayout.EndHorizontal();
            
            GUILayout.Space(20);
            GUILayout.Box("", new GUILayoutOption[] { GUILayout.ExpandWidth(true), GUILayout.Height(1) });

            BeginWindows();
            int i = 0;
            foreach (XEntityStatistics.RowData _row in _data_info.Table)
            {
                bool select = !string.IsNullOrEmpty(_searchText);
                if (select)
                {
                    if (_searchText != _row.UID.ToString()) continue;
                }
                Rect _rect = new Rect((i % col) * width, ((i / col) * height + 100), width, height);
                GUI.Window(_row.UID, _rect, DoWindow, _row.Name);
                i++;
                if (i >= cnt) break;
            }
            EndWindows();
        }

        public void DoWindow(int id)
        {
            XEntityStatistics.RowData npcInfo = _data_info.GetByID(id);
            GUILayout.BeginVertical();
            bool select = !string.IsNullOrEmpty(_searchText);
            if (GUILayout.Button(select ? "OK" : npcInfo.UID.ToString(), GUILayout.Width(80f)))
            {
                OnEnemyClick((uint)id);
            }
            GUILayout.EndVertical();
        }

        public void OnEnemyClick(uint id)
        {
            LevelEditor _levelInstance = (LevelEditor)GetWindow(typeof(LevelEditor));
            int curWave = _levelInstance.LevelMgr.CurrentEdit;
            EditorWave wv = _levelInstance.LevelMgr.GetWave(curWave);
            wv.EntityID = id;
            wv.GenerateMonster();
            Close();
        }

        public void OnSelectBuff()
        {
            if (_buff == "") return;
            LevelEditor _levelInstance = (LevelEditor)GetWindow(typeof(LevelEditor));
            int curWave = _levelInstance.LevelMgr.CurrentEdit;
            EditorWave wv = _levelInstance.LevelMgr.GetWave(curWave);
            wv.SpawnType = LevelSpawnType.Spawn_Buff;
            wv.EntityID = uint.Parse(_buff);
            Close();
        }
        protected void OnSelectScript()
        {
            string path = EditorUtility.OpenFilePanel("Select script", "./Temp", "txt");
            LevelEditor _levelInstance = (LevelEditor)GetWindow(typeof(LevelEditor));
            int curWave = _levelInstance.LevelMgr.CurrentEdit;
            EditorWave wv = _levelInstance.LevelMgr.GetWave(curWave);
            wv.LevelScript = ExtractScriptName(path);
            Close();
        }

        protected void OnSearch() { }

        protected string ExtractScriptName(string path)
        {
            int pos = path.IndexOf("Table/");
            return path.Substring(pos);
        }

    }

}
