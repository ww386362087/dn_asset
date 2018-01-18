#ifndef  __XLevelSpawnInfo__
#define  __XLevelSpawnInfo__

#include <string>
#include <vector>
#include <deque>
#include "Common.h"
#include "XLevelWave.h"
#include "XLevelTask.h"

class XEntity;

class XLevelDynamicInfo
{
public :
	int id;
	bool pushIntoTask = false;
	float generatetime = 0.0f;
	float startTime = 0.0f;
	float exStringFinishTime = 0.0f;
	int totalCount = 0;
	int generateCount = 0;
	int dieCount = 0;
	std::vector<uint> entityIds ;

	void Reset()
	{
		pushIntoTask = false;
		generatetime = 0.0f;
		generateCount = 0;
		startTime = 0.0f;
		exStringFinishTime = 0.0f;
		dieCount = 0;
		entityIds.clear();
	}
};


class XLevelSpawn
{
public:
	XLevelSpawn();
	~XLevelSpawn();
	void Clear();
	void ResetDynamicInfo();
	void KillSpawn(int waveid);
	void Update(float time);
	void SoloUpdate(float time);
	bool ExecuteWaveExtraScript(int wave);
	XLevelDynamicInfo* GetWaveDynamicInfo(int waveid);

protected:
	void RunExtraScript(std::string o);
	void ProcessTaskQueue(float time);
	XLevelWave* GetWaveInfo(int waveid);
	void OnMonsterDie(XEntity* entity);
	void GenerateEntityTask(XLevelWave& wave);
	void GenerateScriptTask(XLevelWave& wave);


public:
	std::vector<XLevelWave*> waves;
	std::unordered_map<int, XLevelDynamicInfo*> wavesDynamicInfo;
	std::unordered_map<int, int> preloadInfo;
	std::deque<XLevelBaseTask*> _tasks;

private:
	const int spawn_monster_per_frame = 2;
};

#endif