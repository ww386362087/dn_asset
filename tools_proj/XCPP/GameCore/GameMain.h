#ifndef  __GameMain__
#define  __GameMain__
 
#include "Common.h"
#include "GameTime.h"
#include "Singleton.h"

/*
	fps:30 tick = run + sleep
*/


//33ms tick once, and ensure game run as 30fps
#define GAME_TICK 33

class GameMain:public Singleton<GameMain>
{

public:
	void Run();
	void Ontick();
	void Start();
	void Stop();

private :
	uint diff = 0;
	bool start = false;
};


extern "C"
{
	ENGINE_INTERFACE_EXPORT void iStartGame();
	ENGINE_INTERFACE_EXPORT void iStopGame();
}

#endif // ! __GameMain__