#ifndef  __XLevelWave__
#define  __XLevelWave__

#include <string>
#include <vector>
#include <map>
#include "Vector3.h"
#include "Common.h"

enum LevelSpawnType
{
	Spawn_Source_Monster,
	Spawn_Source_Player,
	Spawn_Source_Random,
	Spawn_Source_Doodad,
	Spawn_Source_Robot,
};

enum LevelInfoType
{
	TYPE_NONE,
	TYPE_ID,
	TYPE_BASEINFO,
	TYPE_PREWAVE,
	TYPE_EDITOR,
	TYPE_MONSTERINFO,
	TYPE_SCRIPT,
	TYPE_EXSTRING,
	TYPE_SPAWNTYPE,
};

class XLevelWave
{
public:
	int m_Id;
	LevelSpawnType m_SpawnType;
	int m_Time;
	int m_LoopInterval;
	int m_EnemyID;
	int m_YRotate;
	int m_RandomID;

	std::string m_ExString;
	std::vector<int> m_PreWaves;
	float m_PreWavePercent;

	std::map<int, Vector3> m_Monsters;
	std::map<int, float> m_MonsterRotation;

	float m_RoundRidus;
	int m_RoundCount;

	std::vector<int> m_DoodadID;
	float m_DoodadPercent;
	int m_Repeat;
	std::string m_Levelscript;

protected:
	void ParseInfo(const std::string &data);

};

#endif