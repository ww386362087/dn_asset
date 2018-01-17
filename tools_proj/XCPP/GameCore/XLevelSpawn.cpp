#include "XLevelSpawn.h"
#include "XEntity.h"
#include "XPlayer.h"
#include "XLevelStatistics.h"
#include "XLevelScriptMgr.h"

XLevelSpawn::XLevelSpawn()
{
}


XLevelSpawn::~XLevelSpawn()
{
	for (size_t i = 0; i < _tasks.size(); i++)
	{
		delete _tasks[i];
		_tasks[i] = NULL;
	}
	_tasks.clear();

	std::unordered_map<int, XLevelDynamicInfo*>::iterator itr = wavesDynamicInfo.begin();
	for (; itr != wavesDynamicInfo.end(); itr++)
	{
		delete itr->second;
	}
	wavesDynamicInfo.clear();
}


void XLevelSpawn::Clear()
{
	waves.clear();
	for (std::unordered_map<int, XLevelDynamicInfo*>::iterator itr = wavesDynamicInfo.begin(); itr != wavesDynamicInfo.end(); itr++)
	{
		itr->second->Reset();
	}
	wavesDynamicInfo.clear();
	preloadInfo.clear();
	_tasks.clear();
}

void XLevelSpawn::ResetDynamicInfo()
{
	for (std::unordered_map<int, XLevelDynamicInfo*>::iterator itr = wavesDynamicInfo.begin(); itr != wavesDynamicInfo.end(); itr++)
	{
		itr->second->Reset();
	}
}

void XLevelSpawn::KillSpawn(int waveid)
{
	//to-do KillSpawn
}

void XLevelSpawn::Update(float time)
{
	for (size_t i = 0; i < waves.size(); i++)
	{
		XLevelDynamicInfo* dinfo = GetWaveDynamicInfo(waves[i]->ID);
	}
}

void XLevelSpawn::SoloUpdate(float time)
{
	for (int i = 0; i < waves.size(); i++)
	{
		XLevelDynamicInfo* dInfo = GetWaveDynamicInfo(waves[i]->ID);
		if (dInfo == NULL || dInfo->pushIntoTask) continue;
		if (dInfo->totalCount != 0 && dInfo->generateCount == dInfo->totalCount) continue;

		bool preWaveFinished = true;
		for (int j = 0; j < waves[i]->preWave.size(); j++)
		{
			XLevelDynamicInfo* predInfo = wavesDynamicInfo[waves[i]->preWave[j]];
			if (predInfo)
			{
				if ((predInfo->generateCount != predInfo->totalCount))
				{ // 还没生成
					preWaveFinished = false;
					break;
				}
				if (predInfo->entityIds.size() > 0)
				{
					if (predInfo->generateCount != predInfo->dieCount)
					{
						preWaveFinished = false;
						break;
					}
				}
			}
		}
		if (!preWaveFinished) continue;

		bool exStringExists = true;
		if (!waves[i]->exString.empty())
		{
			if (!XLevelScriptMgr::Instance()->QueryExternalString(waves[i]->exString, false))
			{
				exStringExists = false;
			}
		}
		if (exStringExists && dInfo->exStringFinishTime == 0.0f) dInfo->exStringFinishTime = time;
		if (dInfo->startTime == 0.0f) dInfo->startTime = time;
		bool bCanGenerate = false;
		if (!waves[i]->exString.empty())
		{
			if (time - dInfo->exStringFinishTime >= waves[i]->time && dInfo->exStringFinishTime > 0)
			{
				bCanGenerate = true;
			}
		}
		else if (waves[i]->IsScriptWave() || dInfo->generateCount < dInfo->totalCount)
		{
			if (time - dInfo->startTime >= waves[i]->time)
			{
				bCanGenerate = true;
			}
		}

		if (bCanGenerate)
		{
			if (waves[i]->IsScriptWave())
			{
				GenerateScriptTask(*waves[i]);
			}
			else
			{
				GenerateEntityTask(*waves[i]);
			}
			if (!waves[i]->repeat) dInfo->pushIntoTask = true;
			if (waves[i]->repeat && !waves[i]->exString.empty())
			{
				XLevelScriptMgr::Instance()->QueryExternalString(waves[i]->exString, true);
				dInfo->exStringFinishTime = 0;
			}
		}
	}
	ProcessTaskQueue(time);
}

bool XLevelSpawn::ExecuteWaveExtraScript(int wave)
{
	return true;
}

void XLevelSpawn::RunExtraScript(std::string o)
{
	XLevelScriptMgr::Instance()->RunScript(o);
}

void XLevelSpawn::ProcessTaskQueue(float time)
{
	bool bContinueExecuteTask = true;
	for (int i = 0; i < spawn_monster_per_frame; i++)
	{
		if (bContinueExecuteTask)
		{
			if (_tasks.size() > 0)
			{
				XLevelBaseTask* task = _tasks.front();
				_tasks.pop_front();
				bContinueExecuteTask = task->Execute(time);
			}
		}
	}
}


XLevelDynamicInfo* XLevelSpawn::GetWaveDynamicInfo(int waveid)
{
	XLevelDynamicInfo ret;
	return wavesDynamicInfo[waveid];
}

XLevelWave* XLevelSpawn::GetWaveInfo(int waveid)
{
	for (size_t i = 0; i < waves.size(); i++)
	{
		if (waves[i]->ID == waveid)
		{
			return waves[i];
		}
	}
	return NULL;
}

void XLevelSpawn::OnMonsterDie(XEntity* entity)
{
	std::unordered_map<int, XLevelDynamicInfo*>::iterator itr = wavesDynamicInfo.find(entity->Wave);
	if (itr != wavesDynamicInfo.end())
	{
		XLevelDynamicInfo* ret = itr->second;
		ret->dieCount += 1;
		ret->entityIds.erase(ret->entityIds.begin() + entity->EntityID);
	}
}

void XLevelSpawn::GenerateEntityTask(XLevelWave& wave)
{
	if (wave.count > 0)
	{
		float angle = 360.0f / wave.count;
		XPlayer* player = XEntityMgr::Instance()->Player;
		Vector3 pos = wave.isAroundPlayer && player ? player->getPostion() : wave.pos;
		for (int i = 0; i < wave.count; i++)
		{
			XLevelSpawnTask* task = new XLevelSpawnTask(this);
			task->_id = wave.ID;
			task->UID = (uint)wave.uid;
			task->pos = pos;// +Quaternion.Euler(0, angle * i, 0) * new Vector3(0, 0, 1) * wave.radius;
			task->rot = (int)angle * i + 180;
			task->spawnType = wave.spawnType;
			task->isSummonTask = false;
			_tasks.push_back(task);
		}
	}
}


void XLevelSpawn::GenerateScriptTask(XLevelWave& wave)
{
	XLevelScriptTask* task = new XLevelScriptTask(this);
	task->_ScriptName = wave.levelscript;
	_tasks.push_back(task);
}