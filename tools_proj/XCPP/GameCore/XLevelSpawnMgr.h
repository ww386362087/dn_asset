#ifndef  __XLevelSpawnMgr__
#define  __XLevelSpawnMgr__

#include "Singleton.h"
#include "Common.h"
#include "XLevelWave.h"
#include "XLevelScriptMgr.h"
#include "LevelSpawnType.h"
#include "XLevelSpawn.h"
#include "XLevelStatistics.h"
#include <string>

class XLevelSpawnMgr:public Singleton<XLevelSpawnMgr>
{
public:
	XLevelSpawnMgr();
	~XLevelSpawnMgr();
	void OnEnterScene(uint sceneid);
	void Update(float deltaT);
	void ForceLevelFinish(bool win);
	void OnLevelFinish(Vector3 dropInitPos, Vector3 dropGounrdPos, uint money, uint itemCount, bool bKillOpponent);
	void OnLevelFailed();
	void ClearAll();

private:
	std::map<uint, std::map<int, XLevelWave*>> m_StaticWaves;
	float _time = 0;
	XLevelSpawn* _curSpawner;
	bool BossExtarScriptExecuting = false;
	bool IsCurrentLevelWin;
	bool IsCurrentLevelFinished;
};

#endif