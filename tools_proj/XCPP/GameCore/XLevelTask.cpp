#include "XLevelTask.h"
#include "XLevelScriptMgr.h"
#include "Vector3.h"
#include "XEntity.h"
#include "GameMain.h"
#include "XNpcList.h"

XEntity& XLevelSpawnTask::CreateMonster(uint id, float yRotate, Vector3 pos, int _waveid)
{
	Vector3 rot = Vector3(0, yRotate, 0);
	XEntity* entity = XEntityMgr::Instance()->CreateEntity(id, pos, rot);
	entity->Wave = _waveid;
	entity->CreateTime = GameMain::Instance()->Time();
	return *entity;
}

XEntity& XLevelSpawnTask::CreateNPC(uint id, float yRotate, Vector3 pos, int _waveid)
{
	return CreateMonster(id, yRotate, pos, _waveid);
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