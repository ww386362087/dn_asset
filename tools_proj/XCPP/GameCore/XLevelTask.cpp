#include "XLevelTask.h"
#include "XLevelScriptMgr.h"
#include "Vector3.h"
#include "XEntity.h"
#include "GameMain.h"
#include "XLevelSpawn.h"
#include "XNpcList.h"
#include "XLevelStatistics.h"
#include "XNpcList.h"
#include "XMonster.h"

XEntity& XLevelSpawnTask::CreateMonster(uint id, int yRotate, Vector3 pos, int _waveid)
{
	Vector3 rot = Vector3(0, (float)yRotate, 0);
	XEntity* entity = XEntityMgr::Instance()->CreateEntity<XMonster>(id, pos, rot);
	entity->Wave = _waveid;
	entity->CreateTime = GameMain::Instance()->Time();
	return *entity;
}

XEntity& XLevelSpawnTask::CreateNPC(uint id, int yRotate, Vector3 pos, int _waveid)
{
	Vector3 rot = Vector3(0, yRotate, 0);
	XNpcListRow row;
	iGetXNpcListRowByID(id, &row);
	XEntity* entity = XEntityMgr::Instance()->CreateNPC(row, pos, rot);
	entity->Wave = _waveid;
	entity->CreateTime = GameMain::Instance()->Time();
	return *entity;
}

bool XLevelSpawnTask::Execute(float time)
{
	XLevelBaseTask::Execute(time);
	XLevelDynamicInfo* dInfo = _spawner->GetWaveDynamicInfo(_id);
	int entityid;
	if (spawnType == Spawn_Monster)
	{
		XEntity& e = CreateMonster(UID, rot, pos + Vector3(0, 0.02f, 0), _id);
		entityid = e.EntityID;
		XLevelStatistics::Instance()->ls->AddLevelSpawnEntityCount(entityid);
		if (e.IsBoss()) return false;
	}
	else if (spawnType == Spawn_Buff)
	{
		// to-do buff
	}
	else if (spawnType == Spawn_NPC)
	{
		XEntity& e = CreateNPC(UID, rot, pos + Vector3(0, 0.02f, 0), _id);
		entityid = e.EntityID;
	}
	else //player or monster
	{
		// to-do
	}

	if (dInfo)
	{
		dInfo->generateCount++;
		dInfo->entityIds.push_back(entityid);
		if (dInfo->generateCount == dInfo->totalCount)
		{
			dInfo->generatetime = time;
		}
	}
	return true;
}


bool XLevelScriptTask::Execute(float time)
{
	XLevelBaseTask::Execute(time);
	XLevelScriptMgr::Instance()->RunScript(_ScriptName);
	return false;
}