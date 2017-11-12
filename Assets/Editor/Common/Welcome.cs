using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using XEditor;

[InitializeOnLoad]
public class Welcome
{
    static Welcome()
    {
        XConfig.Initial(LogLevel.Log, LogLevel.Error);
        XTableMgr.Initial();
        XResources.Init();

        int isShow = PlayerPrefs.GetInt("ShowWelcomeScreen", 1);
        
        if (isShow == 1)
        {
            EditorApplication.update += Update;
        }
    }

    static void Update()
    {
        bool isSuccess = EditorApplication.ExecuteMenuItem("XEditor/Welcome Screen");
        if (isSuccess) EditorApplication.update -= Update;
    }

}


public class WelcomeScreen : EditorWindow
{
    private bool flag = true;
    private Rect mCutsceneDescriptionRect = new Rect(70f, 344f, 250f, 30f);
    private Rect mCutsceneHeaderRect = new Rect(70f, 324f, 250f, 20f);
    private Texture mCutSceneImage;
    private Rect mCutsceneImageRect = new Rect(15f, 322f, 50f, 50f);
    private Rect mLevelDescriptionRect = new Rect(70f, 143f, 260f, 30f);
    private Rect mLevelHeaderRect = new Rect(70f, 123f, 350f, 20f);
    private Texture mLevelImage;
    private Rect mLevelImageRect = new Rect(15f, 124f, 53f, 50f);
    private Rect mAIDescriptionRect = new Rect(70f, 278f, 380f, 30f);
    private Rect mAIHeaderRect = new Rect(70f, 258f, 250f, 20f);
    private Texture mAIImage;
    private Rect mAIImageRect = new Rect(15f, 256f, 50f, 50f);
    private Rect mUIDescriptionRect = new Rect(70f, 77f, 350f, 70f);
    private Rect mUIHeaderRect = new Rect(70f, 57f, 250f, 20f);
    private Texture mUIImage;
    private Rect mUIImageRect = new Rect(15f, 58f, 50f, 50f);
    private Rect mToggleButtonRect = new Rect(280f, 385f, 125f, 20f);
    private Rect mVersionRect = new Rect(5f, 385f, 225f, 20f);
    private Rect mSkillDescriptionRect = new Rect(70f, 209f, 380f, 30f);
    private Rect mSkillHeaderRect = new Rect(70f, 189f, 350f, 20f);
    private Texture mSkillImage;
    private Rect mSkillImageRect = new Rect(15f, 190f, 50f, 50f);
    private Rect mWelcomeIntroRect = new Rect(15f, 12f, 400f, 40f);
    private Texture mWelcomeScreenImage;
    private string mWelComeText;
    private string mUITitle;
    private string mUIContent;
    private string mLevelTitle;
    private string mLevelContent;
    private string mSkillTitle;
    private string mSkillContent;
    private string mAITitle;
    private string mAIContent;
    private string mCutsceTitle;
    private string mCutsceContent;
    private string mStartText;
    private string mDriverText;

    public void OnEnable()
    {
        flag = PlayerPrefs.GetInt("ShowWelcomeScreen", 1) == 1;
        mUIImage = LoadTexture("WelcomeScreenSamplesIcon.png");
        mLevelImage = LoadTexture("WelcomeLevelIcon.png");
        mSkillImage = LoadTexture("WelcomeSkillIcon.png");
        mAIImage = LoadTexture("Behavior Designer Scene Icon.png");
        mCutSceneImage = LoadTexture("WelcomeScreenContactIcon.png");
        LoadConfig();
    }

    Texture LoadTexture(string name)
    {
        string path = "Assets/Editor/EditorResources/Welcome/";
        return (Texture)AssetDatabase.LoadAssetAtPath(path + name, typeof(Texture));
    }

    void LoadConfig()
    {
        Queue<string> conf = new Queue<string>();
        string path = "Assets/Editor/EditorResources/Welcome/welcome.txt";
        StreamReader stream  = File.OpenText(path);
        while (true)
        {
            string str = stream.ReadLine();
            if (string.IsNullOrEmpty(str)) break;
            conf.Enqueue(str);
        }
        stream.Close();
        
        mWelComeText = conf.Dequeue();
        mUITitle = conf.Dequeue();
        mUIContent = conf.Dequeue();
        mLevelTitle = conf.Dequeue();
        mLevelContent = conf.Dequeue();
        mSkillTitle = conf.Dequeue();
        mSkillContent = conf.Dequeue();
        mAITitle = conf.Dequeue();
        mAIContent = conf.Dequeue();
        mCutsceTitle = conf.Dequeue();
        mCutsceContent = conf.Dequeue();
        mStartText = conf.Dequeue();
        mDriverText = conf.Dequeue();
    }
    
    public void OnGUI()
    {
        GUI.Label(mWelcomeIntroRect,mWelComeText);
        GUI.DrawTexture(mUIImageRect, mUIImage);
        GUI.Label(mUIHeaderRect, mUITitle);
        GUI.Label(mUIDescriptionRect, mUIContent);
        GUI.DrawTexture(mLevelImageRect, mLevelImage);
        GUI.Label(mLevelHeaderRect, mLevelTitle);
        GUI.Label(mLevelDescriptionRect, mLevelContent);
        GUI.DrawTexture(mSkillImageRect, mSkillImage);
        GUI.Label(mSkillHeaderRect, mSkillTitle);
        GUI.Label(mSkillDescriptionRect, mSkillContent);
        GUI.DrawTexture(mAIImageRect, mAIImage);
        GUI.Label(mAIHeaderRect, mAITitle);
        GUI.Label(mAIDescriptionRect, mAIContent);
        GUI.DrawTexture(mCutsceneImageRect, mCutSceneImage);
        GUI.Label(mCutsceneHeaderRect, mCutsceTitle);
        GUI.Label(mCutsceneDescriptionRect, mCutsceContent);
        GUI.Label(mVersionRect, mDriverText);

        flag = GUI.Toggle(mToggleButtonRect, flag, mStartText);
        PlayerPrefs.SetInt("ShowWelcomeScreen", flag ? 1 : 0);

        EditorGUIUtility.AddCursorRect(mUIImageRect, MouseCursor.Link);
        EditorGUIUtility.AddCursorRect(mUIHeaderRect, MouseCursor.Link);
        EditorGUIUtility.AddCursorRect(mUIDescriptionRect, MouseCursor.Link);
        EditorGUIUtility.AddCursorRect(mLevelImageRect, MouseCursor.Link);
        EditorGUIUtility.AddCursorRect(mLevelHeaderRect, MouseCursor.Link);
        EditorGUIUtility.AddCursorRect(mLevelDescriptionRect, MouseCursor.Link);
        EditorGUIUtility.AddCursorRect(mSkillImageRect, MouseCursor.Link);
        EditorGUIUtility.AddCursorRect(mSkillHeaderRect, MouseCursor.Link);
        EditorGUIUtility.AddCursorRect(mSkillDescriptionRect, MouseCursor.Link);
        EditorGUIUtility.AddCursorRect(mAIImageRect, MouseCursor.Link);
        EditorGUIUtility.AddCursorRect(mAIHeaderRect, MouseCursor.Link);
        EditorGUIUtility.AddCursorRect(mAIDescriptionRect, MouseCursor.Link);
        EditorGUIUtility.AddCursorRect(mCutsceneImageRect, MouseCursor.Link);
        EditorGUIUtility.AddCursorRect(mCutsceneHeaderRect, MouseCursor.Link);
        EditorGUIUtility.AddCursorRect(mCutsceneDescriptionRect, MouseCursor.Link);

        if (Event.current.type == EventType.MouseUp)
        {
            Vector2 mousePosition = Event.current.mousePosition; if ((mLevelImageRect.Contains(mousePosition) || mLevelHeaderRect.Contains(mousePosition)) || mLevelDescriptionRect.Contains(mousePosition))
            {
                GetWindow(typeof(LevelEditor));
            }
            else if ((mUIImageRect.Contains(mousePosition) || mUIHeaderRect.Contains(mousePosition)) || mUIDescriptionRect.Contains(mousePosition))
            {
                string path = Path.Combine(Application.dataPath, "Resources/UI");
                HelperEditor.Open(path);
            }
            else if ((mSkillImageRect.Contains(mousePosition) || mSkillHeaderRect.Contains(mousePosition)) || mSkillDescriptionRect.Contains(mousePosition))
            {
                EditorApplication.ExecuteMenuItem("XEditor/Open Skill");
            }
            else if ((mAIImageRect.Contains(mousePosition) || mAIHeaderRect.Contains(mousePosition)) || mAIDescriptionRect.Contains(mousePosition))
            {
                EditorApplication.ExecuteMenuItem("Tools/Behavior Designer/Editor");
            }
            else if ((mCutsceneImageRect.Contains(mousePosition) || mCutsceneHeaderRect.Contains(mousePosition)) || mCutsceneDescriptionRect.Contains(mousePosition))
            {
                EditorApplication.ExecuteMenuItem("XEditor/Cut Scene");
            }
        }
    }

    [MenuItem("XEditor/Welcome Screen", false, 3)]
    public static void ShowWindow()
    {
        WelcomeScreen window = EditorWindow.GetWindow<WelcomeScreen>(true, "Welcome");
        window.minSize = window.maxSize = new Vector2(410f, 410f);
        Object.DontDestroyOnLoad(window);
    }

}