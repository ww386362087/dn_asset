#if TEST

using UnityEngine;


/// <summary>
/// 利用UGUI 的依赖关系做测试
/// Canvas1 Canvas2 Canvas12 三个prefab共同引用2张图
/// 所以生成ab的时候是5个 
/// 正确的加载关系是先加载2张图 再加载prefab
/// 卸载 只卸载根 公共资源不释放（游戏中选一个合适的时机释放--切场景）
/// </summary>
public class TestAB : MonoBehaviour
{

    string path1 = "UI/Canvas1";
    string path2 = "UI/Canvas2";
    string path12 = "UI/Canvas12";

    GameObject go1, go2, go12;
    GUILayoutOption[] option;
    void Start()
    {
        option = new GUILayoutOption[] { GUILayout.Width(140), GUILayout.Height(80) };
        GameEnine.SetMonoForTest(this);
        XTimerMgr.singleton.Init();
        XConfig.Initial(LogLevel.Log, LogLevel.Error);
        XGlobalConfig.Initial();
        XResources.Init();
    }
    

    void OnGUI()
    {
        GUILayout.Space(10);
        GUILayout.BeginVertical();

        GUILayout.BeginHorizontal();
        GUILayout.Space(10);
        if (GUILayout.Button("AsynLoad1", option))
        {
            XResources.LoadAsync<GameObject>(path1, AssetType.Prefab, OnLoad1Complete);
        }
        if (GUILayout.Button("Unload1", option))
        {
            XResources.SafeDestroy(go1);
        }
        GUILayout.EndHorizontal();
        GUILayout.Space(20);

        GUILayout.BeginHorizontal();
        GUILayout.Space(10);
        if (GUILayout.Button("ImmLoad2", option))
        {
            go2 = XResources.Load<GameObject>(path2, AssetType.Prefab);
            go2.name = "Load2";
        }
        if (GUILayout.Button("Unload2", option))
        {
            XResources.SafeDestroy(go2);
        }
        GUILayout.EndHorizontal();
        GUILayout.Space(20);

        GUILayout.BeginHorizontal();
        GUILayout.Space(10);
        if (GUILayout.Button("ImmLoad12", option))
        {
            go12 = XResources.Load<GameObject>(path12, AssetType.Prefab);
            go12.name = "Load12";
        }
        if (GUILayout.Button("Unload12", option))
        {
            XResources.SafeDestroy(go12);
        }
        GUILayout.EndHorizontal();

        GUILayout.EndVertical();
    }

    void Update()
    {
        XTimerMgr.singleton.Update(Time.deltaTime);
        XResources.Update();
    }

    private void OnLoad1Complete(Object o)
    {
       // XDebug.Log("OnLoad1Complete");
        go1 = o as GameObject;
        go1.name = "load1";
    }

}

#endif