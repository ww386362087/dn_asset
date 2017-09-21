using UnityEngine;

/// <summary>
/// 游戏逻辑的唯一入口
/// </summary>
public class GameEntrance : MonoBehaviour
{
    void Awake()
    {
        XTableMgr.tableLoaded += ToStartTest;
    }


    void Start()
    {
        GameEnine.Init(this);  
    }


    void ToStartTest()
    {
#if TEST
        TestManager.Get().Start();
#endif
    }


    void Update()
    {
        GameEnine.Update(Time.deltaTime);

#if TEST
        TestManager.Get().Update();
#endif
    }


    void LateUpdate()
    {
        GameEnine.LateUpdate();


#if TEST
        TestManager.Get().Update();
#endif
    }

    void OnGUI()
    {
#if TEST
        TestManager.Get().OnGUI();
#endif
    }


    void OnDestroy()
    {
        GameEnine.OnUnintial();
    }


}
