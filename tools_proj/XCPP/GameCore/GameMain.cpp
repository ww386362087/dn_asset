#include "GameMain.h"
#include "XEntityMgr.h"

void GameMain::Run()
{
	while (m_start)
	{
		clock_t time = clock();
		Ontick(m_diff);
		long ex = clock() - time;
		if (ex < GAME_TICK)
		{
			GameTime::sleep(GAME_TICK - ex);
		}
		m_diff = clock() - time;
	}
}

int i = 0;
void GameMain::Ontick(long diff)
{
	i++;
	std::cout << " tick" << i << " : " << diff << std::endl;
	float ft = 1000;
	float delta = diff / ft;
	XEntityMgr::Instance()->Update(diff);
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