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

    string path1 = "UI/Canvas12";
    string path2 = "UI/Canvas1";

    public void Start()
    {
        XTimerMgr.singleton.Init();
        XConfig.Initial(LogLevel.Log, LogLevel.Error);
        XGlobalConfig.Initial();

        ABManager.singleton.Initial();
    }


    public void OnGUI()
    {
        if (GUI.Button(new Rect(30, 30, 200, 100), "Load1"))
        {
            GameObject go = XResourceMgr.Load<GameObject>(path1, AssetType.Prefab);
            go.name = "******* ab ******";
        }
        if (GUI.Button(new Rect(30, 180, 200, 100), "Load2"))
        {
            GameObject go = XResourceMgr.Load<GameObject>(path2, AssetType.Prefab);
            go.name = "******* ab ******";
        }
    }

    public void Update()
    {
        XTimerMgr.singleton.Update(Time.deltaTime);
        ABManager.singleton.Update();
        XResourceMgr.Update();
    }
    
}

#endif