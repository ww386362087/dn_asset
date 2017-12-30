using UnityEngine;

/// <summary>
/// 游戏逻辑的唯一入口
/// </summary>
public class GameEntrance : MonoBehaviour
{

    private bool start = false;

    void Awake()
    {
        try
        {
            XTableMgr.tableLoaded += ToStartTest;
        }
        catch(System.Exception e)
        {
            Debug.LogError("ERROR AWAKE:" + e.Message + "\n" + e.StackTrace);
        }
    }


    void Start()
    {
        try
        {
            Debug.Log("GameEntrance Start");
            GameEnine.Init(this);
        }
        catch (System.Exception e)
        {
            Debug.LogError("EROR START:" + e.Message + "\n" + e.StackTrace);
        }
    }


    void ToStartTest(bool st)
    {
#if TEST
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
#if TEST
        TestManager.Get().OnQuit();
#endif
    }


    void OnApplicationQuit()
    {
        GameEnine.OnApplicationQuit();
    }


}
