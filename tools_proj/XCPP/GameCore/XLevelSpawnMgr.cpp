#include "XLevelSpawnMgr.h"
#include "XEntityStatistics.h"


XLevelSpawnMgr::XLevelSpawnMgr()
{
}


XLevelSpawnMgr::~XLevelSpawnMgr()
{
}


bool XLevelSpawnMgr::Init()
{
	LoadFile();

	return true;
}

void XLevelSpawnMgr::Uninit()
{
	Release();
}

bool XLevelSpawnMgr::LoadFile()
{
	Release();
	ParseWaves();
	return true;
}


void XLevelSpawnMgr::Release()
{
}

void XLevelSpawnMgr::ParseWaves()
{
	for (auto i = m_StaticWaves.begin(); i != m_StaticWaves.end(); ++i)
	{
		// parse one scene
		XLevelStatistic info;
		for (auto j = i->second.begin(); j != i->second.end(); ++j)
		{
			ParseWave(info, j->second, i->first);
		}
		m_sceneid2info[i->first] = info;
	}
}

void XLevelSpawnMgr::ParseWave(XLevelStatistic& info, XLevelWave* wave, uint sceneid)
{
	if (0 == wave->m_EnemyID)
	{
		return;
	}
	if (wave->m_SpawnType == Spawn_Source_Monster)
	{
		XEntityStatisticsRow* data = new XEntityStatisticsRow();
		iGetXEntityStatisticsRowByID(wave->m_EnemyID, data);
		if (NULL == data)
		{
			PRINT << "enemy id NULL:" << wave->m_EnemyID << " sceneid:" << sceneid;
			return;
		}
		if (data->fightgroup == 0)
		{
			info.SvrBossType = data->uid;
			if ((int)data->attackbase > info.SvrBossAtkMAX)
			{
				info.SvrBossAtkMAX = (int)data->attackbase;
				info.SvrBossSkillMAX = (int)data->attackbase;
			}
			if ((int)data->maxhp > info.SvrBossHpMax)
			{
				info.SvrBossHpMax = (int)data->maxhp;
			}
			if ((uint)data->maxhp < (uint)info.SvrBossHpMin)
			{
				info.SvrBossHpMin = (int)data->maxhp;
			}
		}
		else
		{
			if ((int)data->attackbase > info.SvrMonsterAtkMAX)
			{
				info.SvrMonsterAtkMAX = (int)data->attackbase;
				info.SvrMonsterSkillMAX = (int)data->attackbase;
			}
			if ((int)data->maxhp > info.SvrMonsterHpMax)
			{
				info.SvrMonsterHpMax = (int)data->maxhp;
			}
			if ((uint)data->maxhp < (uint)info.SvrMonsterHpMin)
			{
				info.SvrMonsterHpMin = (int)data->maxhp;
			}
		}
		++info.SvrMonsterCount;
	}
}
