#ifndef  __XLevelWave__
#define  __XLevelWave__

#include <string>
#include <vector>
#include <map>
#include <fstream>
#include "Vector3.h"
#include "Common.h"
#include "LevelSpawnType.h"

class XLevelWave
{
public:
	int ID;
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

	float radius;
	int count;

	std::vector<int> m_DoodadID;
	float m_DoodadPercent;
	int m_Repeat;
	std::string m_Levelscript;

public:
	void ReadFromFile(std::ifstream& infile);

protected:
	void ParseInfo(const std::string &data);

};

#endif