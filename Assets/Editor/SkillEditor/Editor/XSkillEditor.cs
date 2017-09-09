using System.IO;
using System.Xml.Serialization;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using XEditor;


public class XSkillEditor : MonoBehaviour
{
    [MenuItem(@"XEditor/Create Skill %i")]
    static void SkillCreater()
    {
        EditorWindow.GetWindow<XInnerEditor>(@"Skill Editor");
    }

    [MenuItem(@"XEditor/Open Skill %u")]
    static void SkillOpener()
    {
        XInnerEditor.OpenSkill();
    }

    [MenuItem(@"XEditor/Selection #`")]
    static void SelectHot()
    {
        XSkillHoster[] hosters = GameObject.FindObjectsOfType<XSkillHoster>();

        if (hosters.Length > 0)
        {
            Selection.activeObject = hosters[0];
            System.Reflection.Assembly assembly = typeof(UnityEditor.EditorWindow).Assembly;
            System.Type type = assembly.GetType("UnityEditor.GameView");
            EditorWindow gameview = EditorWindow.GetWindow(type);
            gameview.Focus();
        }

        foreach (XSkillHoster hoster in hosters)
            EditorGUIUtility.PingObject(hoster);
    }
}


internal class XInnerEditor : EditorWindow
{
    private string _name = "default_skill";

    private int _id = 0;

    private string _directory = "/";
    private GameObject _prefab = null;

    public AnimationClip _skillClip = null;

    private bool _combined = false;
    private GUIStyle _labelstyle = null;

    void OnGUI()
    {
        if (_labelstyle == null)
        {
            _labelstyle = new GUIStyle(EditorStyles.boldLabel);
            _labelstyle.fontSize = 13;
        }

        GUILayout.Label(@"Create and edit a new skill:", _labelstyle);

        _name = EditorGUILayout.TextField("*Skill Name:", _name);
        _directory = EditorGUILayout.TextField("*Directory:", _directory);
        if (_directory.Length > 0 && _directory[_directory.Length - 1] == '/') _directory.Remove(_directory.Length - 1);
        EditorGUILayout.Space();

        EditorGUI.BeginChangeCheck();
        _id = EditorGUILayout.IntField("*Dummy ID", _id);
        if (EditorGUI.EndChangeCheck())
        {
            _prefab = XEditorLibrary.GetDummy((uint)_id);
        }

        EditorGUILayout.ObjectField("*Dummy Prefab:", _prefab, typeof(GameObject), true);

        AnimationClip skillClip = null;
        if (!_combined) skillClip = EditorGUILayout.ObjectField("*Skill Animation:", _skillClip, typeof(AnimationClip), true) as AnimationClip;
        _combined = EditorGUILayout.Toggle("Combined Skill", _combined);

        if (skillClip != null)
        {
            string location = AssetDatabase.GetAssetPath(skillClip);
            if (skillClip == null || location.IndexOf(".anim") != -1)
                _skillClip = skillClip;
            else
            {
                EditorUtility.DisplayDialog("Confirm your selection.",
                        "Please select extracted skill clip!",
                        "Ok");
            }
        }

        EditorGUILayout.Space();

        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Done"))
        {
            if (_prefab != null && (_combined || _skillClip != null))
            {
                Scene scene = EditorSceneManager.GetActiveScene();
                if (scene.name.Length > 0 && !EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
                {
                    Close();
                }
                else
                {
                    EditorSceneManager.NewScene(NewSceneSetup.DefaultGameObjects);
                    OnSkillPreGenerationDone();
                    return;
                }
            }
            else
            {
                EditorUtility.DisplayDialog("Confirm your selection.",
                    "Please select prefab and skill clip!",
                    "Ok");
            }
        }
        else if (GUILayout.Button("Cancel"))
        {
            Close();
        }
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.Space();
        GUILayout.Label("* means this section can not be empty.", EditorStyles.foldout);
    }

    public static void OpenSkill()
    {
        string file = EditorUtility.OpenFilePanel("Select skill file", XEditorPath.Skp, "txt");
        if (file.Length != 0)
        {
            Scene scene = EditorSceneManager.GetActiveScene();
            if (!string.IsNullOrEmpty(scene.name) && !EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
            {
                return;
            }
            else
            {
                EditorSceneManager.NewScene(NewSceneSetup.DefaultGameObjects);
                GameObject plane = GameObject.CreatePrimitive(PrimitiveType.Plane);
                plane.name = "Ground";
                plane.layer = LayerMask.NameToLayer("Terrain");
                plane.transform.position = new Vector3(0, -0.01f, 0);
                plane.transform.localScale = new Vector3(1000, 1, 1000);
                plane.GetComponent<Renderer>().sharedMaterial = new Material(Shader.Find("Diffuse"));
                plane.GetComponent<Renderer>().sharedMaterial.SetColor("_Color", new Color(90 / 255.0f, 90 / 255.0f, 90 / 255.0f));
                XDataBuilder.singleton.Load(file);
            }
        }
    }

    public static void OpenSkill(string file)
    {
        if (file.Length != 0)
        {
            XDataBuilder.singleton.Load(file);
        }
    }

    public static bool CheckPrefab(GameObject obj)
    {
        if (obj == null) return false;
        string path = AssetDatabase.GetAssetPath(obj);
        int last = path.LastIndexOf('.');
        string subfix = path.Substring(last, path.Length - last).ToLower();

        if (subfix != AssetType.Prefab)
        {
            EditorUtility.DisplayDialog("Confirm your selection.",
                "Please select a prefab file for this skill!",
                "Ok");

            return false;
        }
        return true;
    }

    private void OnSkillPreGenerationDone()
    {
        XDataBuilder.singleton.Load(PreStoreData());
    }

    private string PreStoreData()
    {
        string skp = XEditorPath.GetPath("Skill" + "/" + _directory) + _name + ".txt";
        string config = XEditorPath.GetEditorBasedPath("Skill" + "/" + _directory) + _name + ".config";

        XConfigData conf = new XConfigData();

        if (!_combined)
        {
            conf.SkillClip = AssetDatabase.GetAssetPath(_skillClip);
            conf.SkillClipName = _skillClip.name;
        }

        conf.Player = _id;
        conf.Directory = _directory;
        conf.SkillName = _name;

        XSkillData data = new XSkillData();
        data.Name = _name;

        if (!_combined)
        {
            data.ClipName = conf.SkillClip.Remove(conf.SkillClip.LastIndexOf('.'));
            data.ClipName = data.ClipName.Remove(0, 17);
        }

        data.TypeToken = _combined ? 3 : 0;
        XDataIO<XSkillData>.singleton.SerializeData(skp, data);

        using (FileStream writer = new FileStream(config, FileMode.Create))
        {
            XmlSerializer formatter = new XmlSerializer(typeof(XConfigData));
            formatter.Serialize(writer, conf);
        }
        AssetDatabase.Refresh();
        return skp;
    }

}