using System;
using System.Collections.Generic;
using UnityEngine;

public class XLevelSpawnMgr : XSingleton<XLevelSpawnMgr>
{
    private XLevelSpawnInfo _curSpawner;
    public bool BossExtarScriptExecuting = false;
    public bool NeedCheckLevelfinishScript { get; set; }
    public bool IsCurrentLevelWin { get; set; }
    public bool IsCurrentLevelFinished { get; set; }

    public void Update(float deltaT)
    {
        if (NeedCheckLevelfinishScript)
        {
            if (!XLevelSpawnMgr.singleton.BossExtarScriptExecuting)
            {
                NeedCheckLevelfinishScript = false;
                ForceLevelFinish(true);
            }
        }
    }
    

    public void ForceLevelFinish(bool win)
    {
        IsCurrentLevelFinished = true;
        if (win)
        {
            XLevelState ls = XLevelStatistics.singleton.ls;
            IsCurrentLevelWin = true;
            OnLevelFinish(ls._lastDieEntityPos + new Vector3(0.0f, ls._lastDieEntityHeight, 0.0f) / 2, ls._lastDieEntityPos, 500, 0, true);
        }
        else
        {
            OnLevelFailed();
        }

    }

    public void OnLevelFinish(Vector3 dropInitPos, Vector3 dropGounrdPos, uint money, uint itemCount, bool bKillOpponent)
    {
    }


    public void OnLevelFailed()
    {
    }


}
