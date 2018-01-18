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
	int uid;
	LevelSpawnType spawnType;
	int time;
	int m_LoopInterval;
	int m_YRotate;
	bool isAroundPlayer;

	std::string exString;
	std::vector<int> m_PreWaves;
	float m_PreWavePercent;

	Vector3 pos;
	float rotateY;

	float radius;
	int count;

	std::vector<int> m_DoodadID;
	std::vector<int> preWave;
	float m_DoodadPercent;
	bool repeat;
	std::string levelscript;

public:
	void ReadFromFile(std::ifstream& infile);
	bool IsScriptWave();

protected:
	void ParseInfo(const std::string &data);

};

#endif