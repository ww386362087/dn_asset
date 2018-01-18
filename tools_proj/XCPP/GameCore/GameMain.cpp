#include "GameMain.h"
#include "XEntityMgr.h"
#include "XScene.h"

void GameMain::OnStart()
{
	XScene::Instance()->Enter(401);
}

void GameMain::Ontick(float delta)
{
	m_delta = delta;
	m_time += delta;
	XTimerMgr::Instance()->Update(delta);
	XEntityMgr::Instance()->Update(delta);
	XScene::Instance()->Update(delta);
}

void GameMain::Start()
{
	m_start = true;
	m_time = 0.0f;
	m_delta = 0;
	OnStart();
}

void GameMain::Stop()
{
	m_start = false;
}

void GameMain::Quit()
{
	m_start = false;
	m_time = 0.0f;
	GameObjectMgr::Instance()->Clear();
	XEntityMgr::Instance()->UnloadAll();
	XTimerMgr::Instance()->RemoveAllTimer();
}

float GameMain::Time()
{
	return m_time;
}

float GameMain::DeltaTime()
{
	return m_delta;
}