using UnityEngine;
using UnityEditor;


[ExecuteInEditMode]
public class LevelEditor : EditorWindow
{
    [SerializeField]
    static LevelEditor _windowInstance;
    private const string _autoSaveFile = "./Temp/__auto__leveleditor.txt";
    private SerializeLevel _SerialziedLevel;
    public Vector2 scrollPosition;
    RaycastHit _hitInfo;
    private float _lastLBClickedTime = 0;

    public SerializeLevel LevelMgr
    {
        get { return _SerialziedLevel; }
    }


    [MenuItem("Window/LevelEditor %L")]
    static void Init()
    {
        _windowInstance = (LevelEditor)GetWindow(typeof(LevelEditor));
    }


    void StateChange()
    {
        if (EditorApplication.isPlayingOrWillChangePlaymode && EditorApplication.isPlaying)
        {
            _SerialziedLevel.RemoveSceneViewInstance();
        }
    }

    public void OnEnable()
    {
        hideFlags = HideFlags.HideAndDontSave;
        if (_SerialziedLevel == null)
        {
            _SerialziedLevel = ScriptableObject.CreateInstance<SerializeLevel>();
            _SerialziedLevel.Editor = this;
        }
        EditorApplication.hierarchyWindowItemOnGUI += HierarchyWindowItemOnGUI;
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
                    _SerialziedLevel.RemoveMonster(monster);
                }
            }
        }
    }

    void OnFocus()
    {
        SceneView.onSceneGUIDelegate -= OnSceneFunc;
        SceneView.onSceneGUIDelegate += OnSceneFunc;
        EditorApplication.playmodeStateChanged -= StateChange;
        EditorApplication.playmodeStateChanged += StateChange;
    }

    static public void OnSceneFunc(SceneView sceneView)
    {
        if (_windowInstance != null)
            _windowInstance.CustomSceneUpdate(sceneView);
    }

    void OnGUI()
    {
        _SerialziedLevel.OnGUI();
    }

    void CustomSceneUpdate(SceneView sceneView)
    {
        Event e = Event.current;
        if (e.type == EventType.MouseDown && e.button == 0)
        {
            if (Time.realtimeSinceStartup - _lastLBClickedTime < 0.2f)
            {
                //Camera cameara = sceneView.camera;  
                Ray ray = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);
                int layerMask = (1 << 9 | 1);
                if (Physics.Raycast(ray, out _hitInfo, Mathf.Infinity, layerMask))
                {
                    _SerialziedLevel.AddMonster(_hitInfo.point + new Vector3(0, 0.05f, 0));
                    Repaint();
                    _lastLBClickedTime = 0.0f;
                }
            }
            else
            {
                _lastLBClickedTime = Time.realtimeSinceStartup;
            }
        }
        if (e.isKey && e.keyCode == KeyCode.Delete)
        {
            foreach (GameObject obj in Selection.gameObjects)
            {
                GameObject monster = obj;
                while (monster.transform.parent != null) monster = monster.transform.parent.gameObject;
                if (monster.name.IndexOf("Wave") != -1)
                {
                    _SerialziedLevel.RemoveMonster(monster);
                }
            }
        }

    }

}
