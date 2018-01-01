#include "GameMain.h"
#include "XEntityMgr.h"


void GameMain::OnStart()
{
	XEntityMgr::Instance()->CreateEntity(2, Vector3::zero, Vector3::zero);
	//XEntityMgr::Instance()->CreatePlayer();
}

void GameMain::Ontick(float delta)
{
	XTimerMgr::Instance()->Update(delta);
	XEntityMgr::Instance()->Update(delta);
	XEntityMgr::Instance()->LateUpdate();
}


void GameMain::Start()
{
	m_start = true;
	m_diff = 0;
	OnStart();
}


void GameMain::Stop()
{
	m_start = false;
}

void GameMain::Quit()
{
	GameObjectMgr::Instance()->Clear();
	XEntityMgr::Instance()->UnloadAll();
	XTimerMgr::Instance()->RemoveAllTimer();
}