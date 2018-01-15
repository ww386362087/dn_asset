#include "XLevelTask.h"
#include "XLevelScriptMgr.h"


XEntity* XLevelSpawnTask::CreateMonster(uint id, float yRotate, Vector3 pos, int _waveid)
{
	return 0;
}

XEntity* XLevelSpawnTask::CreateNPC(uint id, float yRotate, Vector3 pos, int _waveid)
{
	return 0;
}

bool XLevelSpawnTask::Execute(float time)
{
	XLevelBaseTask::Execute(time);
	return true;
}


bool XLevelScriptTask::Execute(float time)
{
	XLevelBaseTask::Execute(time);
	XLevelScriptMgr::Instance()->RunScript(_ScriptName);
	return false;
}