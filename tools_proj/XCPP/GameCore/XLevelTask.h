#ifndef  __XLevelTask__
#define  __XLevelTask__

#include "Common.h"
#include <string>
#include "Vector3.h"
#include "LevelSpawnType.h"

class XLevelSpawn;
class XEntity;

class XLevelBaseTask
{
public :
	XLevelBaseTask(XLevelSpawn* spawn) { _spawner = spawn; }
	virtual bool Execute(float time) { return true; }

public:
	int _id;

protected:
	XLevelSpawn* _spawner;
};


class XLevelSpawnTask : public XLevelBaseTask
{
public :
	uint UID;
	int rot;
	Vector3 pos;
	LevelSpawnType spawnType;
	bool isSummonTask;

public:
	XLevelSpawnTask(XLevelSpawn* ls) :XLevelBaseTask(ls) { _spawner = ls; }
	XEntity& CreateMonster(uint id, int yRotate, Vector3 pos, int _waveid);
	XEntity& CreateNPC(uint id, int yRotate, Vector3 pos, int _waveid);
	virtual bool Execute(float time);
};

class XLevelScriptTask : public XLevelBaseTask
{
public:
	std::string _ScriptName;
	XLevelScriptTask(XLevelSpawn* ls) :XLevelBaseTask(ls) { _spawner = ls; }
	virtual bool Execute(float time);
};




#endif