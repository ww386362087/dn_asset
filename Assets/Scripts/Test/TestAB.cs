#if TEST

using UnityEngine;


/// <summary>
/// 利用UGUI 的依赖关系做测试
/// Canvas1 Canvas2 Canvas12 三个prefab共同引用2张图
/// 所以生成ab的时候是5个 
/// 正确的加载关系是先加载2张图 再加载prefab
/// 卸载顺序与加载顺序相同
/// </summary>
public class TestAB : MonoBehaviour
{

    string path1 = "UI/Canvas1";
    string path2 = "UI/Canvas2";
    string path12 = "UI/Canvas12";

    GameObject go1, go2, go12;

    void Start()
    {
        XTimerMgr.singleton.Init();
        XConfig.Initial(LogLevel.Log, LogLevel.Error);
        XGlobalConfig.Initial();
        ABManager.singleton.Initial();
    }

    void OnGUI()
    {
        if (GUI.Button(new Rect(30, 30, 140, 80), "Load1"))
        {
            go1 = XResources.Load<GameObject>(path1, AssetType.Prefab);
            go1.name = "Load1";
        }
        if (GUI.Button(new Rect(30, 130, 140, 80), "Load2"))
        {
            go2 = XResources.Load<GameObject>(path2, AssetType.Prefab);
            go2.name = "Load2";
        }
        if (GUI.Button(new Rect(30, 230, 140, 80), "Load12"))
        {
            go12 = XResources.Load<GameObject>(path12, AssetType.Prefab);
            go12.name = "Load12";
        }
        if (GUI.Button(new Rect(30, 330, 140, 80), "Unload1"))
        {
            XResources.SafeDestroy(ref go1);
        }
        if (GUI.Button(new Rect(30, 430, 140, 80), "Unload2"))
        {
            XResources.SafeDestroy(ref go2);
        }
        if (GUI.Button(new Rect(30, 530, 140, 80), "Unload12"))
        {
            XResources.SafeDestroy(ref go12);
        }
    }

    void Update()
    {
        XTimerMgr.singleton.Update(Time.deltaTime);
        XResources.Update();
    }

}

#endif