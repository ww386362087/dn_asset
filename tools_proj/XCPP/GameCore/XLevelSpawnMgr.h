#ifndef  __XLevelSpawnMgr__
#define  __XLevelSpawnMgr__

#include "Singleton.h"
#include "Common.h"
#include "XLevelWave.h"
#include "XLevelScriptMgr.h"
#include "LevelSpawnType.h"
#include "XLevelSpawnInfo.h"
#include "XLevelStatistics.h"
#include <string>

class XLevelSpawnMgr:Singleton<XLevelSpawnMgr>
{
public:
	XLevelSpawnMgr();
	~XLevelSpawnMgr();
	void OnEnterScene(uint sceneid);
	void Update(float deltaT);
	void ForceLevelFinish(bool win);
	void OnLevelFinish(Vector3 dropInitPos, Vector3 dropGounrdPos, uint money, uint itemCount, bool bKillOpponent);
	void OnLevelFailed();

private:
	std::map<uint, XLevelStatistics> m_sceneid2info;
	std::map<uint, std::map<int, XLevelWave*>> m_StaticWaves;
	float _time = 0;
	XLevelSpawnInfo* _curSpawner;
	bool BossExtarScriptExecuting = false;
	bool IsCurrentLevelWin;
	bool IsCurrentLevelFinished;
};

#endif