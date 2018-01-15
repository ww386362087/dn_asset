#include "XLevelSpawnInfo.h"



XLevelSpawnInfo::XLevelSpawnInfo()
{
}


XLevelSpawnInfo::~XLevelSpawnInfo()
{
}


void XLevelSpawnInfo::Clear()
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

void XLevelSpawnInfo::ResetDynamicInfo()
{
	for (std::unordered_map<int, XLevelDynamicInfo*>::iterator itr = wavesDynamicInfo.begin(); itr != wavesDynamicInfo.end(); itr++)
	{
		itr->second->Reset();
	}
}

void XLevelSpawnInfo::KillSpawn(int waveid)
{

}

void XLevelSpawnInfo::Update(float time)
{

}

void XLevelSpawnInfo::SoloUpdate(float time)
{

}

bool XLevelSpawnInfo::ExecuteWaveExtraScript(int wave)
{
	return true;
}


void XLevelSpawnInfo::RunExtraScript(std::string o)
{

}
void XLevelSpawnInfo::ProcessTaskQueue(float time)
{

}
XLevelDynamicInfo* XLevelSpawnInfo::GetWaveDynamicInfo(int waveid)
{
	return 0;
}
XLevelWave* XLevelSpawnInfo::GetWaveInfo(int waveid)
{
	return 0;
}
void XLevelSpawnInfo::OnMonsterDie(XEntity* entity)
{

}
void XLevelSpawnInfo::GenerateEntityTask(XLevelWave& wave)
{

}
void XLevelSpawnInfo::GenerateScriptTask(XLevelWave& wave)
{

}