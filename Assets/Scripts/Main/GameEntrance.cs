using UnityEngine;

/// <summary>
/// 游戏逻辑的唯一入口
/// </summary>
public class GameEntrance : MonoBehaviour
{

    private bool start = false;

    void Awake()
    {
        XTableMgr.tableLoaded += ToStartTest;
    }


    void Start()
    {
        GameEnine.Init(this);  
    }


    void ToStartTest(bool st)
    {
#if TEST
        XDebug.LogGreen("callback: " + st);
        start = true;
        TestManager.Get().Start();
#endif
    }


    void Update()
    {

        XTableMgr.Update();

         if (start)
        {
            GameEnine.Update(Time.deltaTime);

#if TEST
            TestManager.Get().Update();
#endif
        }
    }


    void LateUpdate()
    {
        if (start)
        {
            GameEnine.LateUpdate();


#if TEST
            TestManager.Get().Update();
#endif
        }
    }

    void OnGUI()
    {
        if (start)
        {
#if TEST
            TestManager.Get().OnGUI();
#endif
        }
    }


    void OnDestroy()
    {
        GameEnine.OnUnintial();
    }


    void OnApplicationQuit()
    {

    }


}
