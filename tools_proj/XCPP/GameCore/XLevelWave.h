#ifndef  __XLevelWave__
#define  __XLevelWave__

#include <string>
#include <vector>
#include <map>
#include "Vector3.h"
#include "Common.h"
#include "LevelSpawnType.h"



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