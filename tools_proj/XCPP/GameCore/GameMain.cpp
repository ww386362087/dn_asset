#include "GameMain.h"


void GameMain::Run()
{
	while (start)
	{
		time_t time = GameTime::GetMSTime();

		Ontick();

		time_t _now = GameTime::GetMSTime();
		diff = (uint)(_now - time);
		if (diff < GAME_TICK)
		{
			GameTime::sleep((GAME_TICK - diff));
		}
		time = GameTime::GetMSTime();
	}
}

int i = 0;
void GameMain::Ontick()
{
	i++;
	std::cout << "i is: "<<i << std::endl;
}


void GameMain::Start()
{
	start = true;
	diff = 0;
	Run();
}


void GameMain::Stop()
{
	start = false;
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