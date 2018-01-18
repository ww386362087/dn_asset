#include "XLevelSpawnMgr.h"
#include "XEntityStatistics.h"
#include "XScene.h"
#include <assert.h>

extern std::string UNITY_STREAM_PATH;

XLevelSpawnMgr::XLevelSpawnMgr()
{
}

XLevelSpawnMgr::~XLevelSpawnMgr()
{
	ClearAll();
}

void XLevelSpawnMgr::ClearAll()
{
	delete _curSpawner;
	_curSpawner = NULL;
	for (std::map<uint, std::map<int, XLevelWave*>>::iterator itr = m_StaticWaves.begin();
		itr != m_StaticWaves.end(); itr++)
	{
		for (size_t i = 0; i < itr->second.size(); i++)
		{
			delete itr->second[i];
		}
		itr->second.clear();
	}
	m_StaticWaves.clear();
}

void XLevelSpawnMgr::OnEnterScene(uint sceneid)
{
	_time = 0;
	XLevelScriptMgr::Instance()->CommandCount = 0;
	SceneListRow* row = XScene::Instance()->getSceneRow();
	char* configFile = row->scenefile;
	if (configFile == NULL)
	{
		delete _curSpawner;
		_curSpawner = NULL;
		XLevelScriptMgr::Instance()->ClearWallInfo();
		XLevelScriptMgr::Instance()->Reset();
		return;
	}
	if (_curSpawner == NULL) _curSpawner = new XLevelSpawn();
	else _curSpawner->Clear();
	ifstream infile;
	std::string path = UNITY_STREAM_PATH + tostring(configFile)+".txt";
	infile.open(path.data(), ios::in);
	assert(infile.is_open());
	string line;
	getline(infile, line);
	int totalWave = atoi(line.c_str());
	getline(infile, line);
	int preloadwave = atoi(line.c_str());
	for (int i = 0; i < preloadwave; i++)
	{
		getline(infile, line);
		std::vector<string> info = split(line, ',');
		int enemyID = atoi(info[0].substr(3).c_str());
		int count = atoi(info[1].c_str());
		_curSpawner->preloadInfo.insert(std::make_pair(enemyID, count));
	}
	for (int i = 0; i < totalWave; i++)
	{
		XLevelWave* wave = new XLevelWave();
		wave->ReadFromFile(infile);
		_curSpawner->waves.push_back(wave);

		XLevelDynamicInfo* dInfo = new XLevelDynamicInfo();
		dInfo->id = wave->ID;
		dInfo->totalCount = wave->count;
		dInfo->Reset();
		_curSpawner->wavesDynamicInfo.insert(std::make_pair(wave->ID, dInfo));
	}
	infile.close();
}


void XLevelSpawnMgr::Update(float deltaT)
{
	if (IsCurrentLevelFinished) return;
	_time += deltaT;
	if (_curSpawner) _curSpawner->Update(_time);
}

void XLevelSpawnMgr::ForceLevelFinish(bool win)
{
	IsCurrentLevelFinished = true;
	if (win)
	{
		XLevelState* ls = XLevelStatistics::Instance()->ls;
		IsCurrentLevelWin = true;
		Vector3 v3 = Vector3(0.0f, ls->_lastDieEntityHeight*0.5f, 0.0f);
		OnLevelFinish(ls->_lastDieEntityPos + v3, ls->_lastDieEntityPos, 500, 0, true);
	}
	else
	{
		OnLevelFailed();
	}
}

void XLevelSpawnMgr::OnLevelFinish(Vector3 dropInitPos, Vector3 dropGounrdPos, uint money, uint itemCount, bool bKillOpponent)
{
	LOG("LEVEL FINISH");
}

void XLevelSpawnMgr::OnLevelFailed()
{
	LOG("LEVEL FAILED");
}
