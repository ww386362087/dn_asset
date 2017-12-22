#include "GameMain.h"
#include "XEntityMgr.h"

void GameMain::Run()
{
	while (m_start)
	{
		clock_t clck = clock();
		Ontick(m_diff);
		m_ex = clock() - clck;
		if (m_ex < GAME_TICK)
		{
			m_sleep = GAME_TICK - m_ex;
			GameTime::sleep(GAME_TICK - m_ex);
		}
		m_diff = clock() - clck;
	}
}


void GameMain::OnStart()
{
	XEntityMgr::Instance()->CreateEntity(2, Vector3::zero, Vector3::zero);
}

void GameMain::Ontick(long diff)
{
	//std::cout << " ticktime" << " : " << diff << " ex: " << m_ex << " sleep: " << m_sleep << std::endl;
	float ft = 1000;
	float delta = diff / ft;
	XTimerMgr::Instance()->Update(delta);
	XEntityMgr::Instance()->Update(delta);
	XEntityMgr::Instance()->LateUpdate();
}



void GameMain::Start()
{
	m_start = true;
	m_diff = 0;
	Run();
}


void GameMain::Stop()
{
	m_start = false;
}


extern "C"
{
	void iStartGame()
	{
		GameMain::Instance()->Start();
	}


	void iStopGame()
	{
		GameMain::Instance()->Stop();
	}
}