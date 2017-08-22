using UnityEngine;
using UnityEditor;


[ExecuteInEditMode]
public class LevelEditor : EditorWindow
{
    private const string _autoSaveFile = "./Temp/__auto__leveleditor.txt";
    private SerializeLevel _SerialziedLevel;
    public Vector2 scrollPosition;
    RaycastHit _hitInfo;

    public SerializeLevel LevelMgr
    {
        get { return _SerialziedLevel; }
    }


    [MenuItem("Window/LevelEditor %L")]
    static void Init()
    {
        GetWindow(typeof(LevelEditor));
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
        EditorApplication.playmodeStateChanged -= StateChange;
        EditorApplication.playmodeStateChanged += StateChange;
    }


    void OnGUI()
    {
        _SerialziedLevel.OnGUI();
    }
    
}
