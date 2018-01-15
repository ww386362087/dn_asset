#ifndef  __XLevelSpawnMgr__
#define  __XLevelSpawnMgr__

#include "Singleton.h"
#include "Common.h"
#include "XLevelWave.h"
#include "XLevelScriptMgr.h"
#include "LevelSpawnType.h"
#include "XLevelSpawnInfo.h"

struct XLevelStatistic
{
	XLevelStatistic()
	{
		SvrMonsterCount = 0;
		SvrBossType = 0;
		SvrMonsterAtkMAX = 0;
		SvrMonsterSkillMAX = 0;
		SvrMonsterHpMax = 0;
		SvrMonsterHpMin = (uint)(-1);
		SvrBossAtkMAX = 0;
		SvrBossSkillMAX = 0;
		SvrBossHpMax = 0;
		SvrBossHpMin = (uint)(-1);
	}

	void Check()
	{
		if (SvrBossHpMin == (uint)(-1))
		{
			SvrBossHpMin = 0;
		}
		if (SvrMonsterHpMin == (uint)(-1))
		{
			SvrMonsterHpMin = 0;
		}
	}

	int SvrMonsterCount;       	///> 本关卡配置的怪物数量（包括小怪和boss）" 
	int SvrBossType;			///> 副本内BOSS编号,多个BOSS,仅记录最终BOSS"		
	int SvrMonsterAtkMAX;		///> 本关卡配置的小怪攻击力最大值" 
	int SvrMonsterSkillMAX;    	///> 本关卡配置的小怪技能伤害最大值" 		
	int SvrMonsterHpMax;		///> 本关卡配置的小怪生命值最大值" 
	int SvrMonsterHpMin;		///> 本关卡配置的小怪生命值最小值" 
	int SvrBossAtkMAX;        	///> 本关卡配置的BOSS攻击力最大值" 
	int SvrBossSkillMAX;		///> 本关卡配置的boss技能伤害最大值" 
	int SvrBossHpMax;         	///> 本关卡配置的Boss生命值最大值" 
	int SvrBossHpMin;         	///> 本关卡配置的Boss生命值最小值"
};


class XLevelSpawnMgr:Singleton<XLevelSpawnMgr>
{
public:
	XLevelSpawnMgr();
	~XLevelSpawnMgr();
	void OnEnterScene(uint sceneid);
	bool LoadFile();

private:
	void ParseWaves();
	void ParseWave(XLevelStatistic& info, XLevelWave* wave, uint sceneid);

private:
	std::map<uint, XLevelStatistic> m_sceneid2info;
	std::map<uint, std::map<int, XLevelWave*>> m_StaticWaves;
	float _time = 0;
	XLevelSpawnInfo* _curSpawner;
};

#endif