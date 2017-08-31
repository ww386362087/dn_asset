using UnityEngine;
using UnityEditor;

namespace XEditor
{

    [ExecuteInEditMode]
    public class LevelEditor : EditorWindow
    {
        private const string _autoSaveFile = "./Temp/__auto__leveleditor.txt";
        private SerializeLevel _serial;
        public Vector2 scrollPosition;
        RaycastHit _hitInfo;

        public SerializeLevel LevelMgr
        {
            get { return _serial; }
        }

        [MenuItem("XEditor/LevelEditor %L")]
        static void Init()
        {
            GetWindow(typeof(LevelEditor));
        }

        void StateChange()
        {
            if (EditorApplication.isPlayingOrWillChangePlaymode && EditorApplication.isPlaying)
            {
                _serial.RemoveSceneViewInstance();
            }
        }

        void OnDestroy()
        {
            _serial.RemoveSceneViewInstance();
        }

        public void OnEnable()
        {
            hideFlags = HideFlags.HideAndDontSave;
            if (_serial == null)
            {
                _serial = CreateInstance<SerializeLevel>();
                _serial.Editor = this;
            }
            EditorApplication.hierarchyWindowItemOnGUI += HierarchyWindowItemOnGUI;
        }

        public void OnDisable()
        {
            _serial.SaveToFile(_autoSaveFile, true);
        }

        void HierarchyWindowItemOnGUI(int instanceID, Rect selectionRect)
        {
            Event e = Event.current;
            if (e.isKey && e.keyCode == KeyCode.Delete)
            {
                foreach (GameObject obj in Selection.gameObjects)
                {
                    GameObject monster = obj;
                    while (monster.transform.parent != null) monster = monster.transform.parent.gameObject;
                    if (monster.name.IndexOf("Wave") != -1)
                    {
                        _serial.RemoveMonster(monster);
                    }
                }
            }
        }

        void OnFocus()
        {
            EditorApplication.playmodeStateChanged -= StateChange;
            EditorApplication.playmodeStateChanged += StateChange;
        }


        void OnGUI()
        {
            _serial.OnGUI();
        }

    }

}