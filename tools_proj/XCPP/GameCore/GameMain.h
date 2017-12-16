#ifndef  __GameMain__
#define  __GameMain__
 
#include "Common.h"
#include "GameTime.h"
#include "Singleton.h"
#include <ctime> 

/*
	fps:30 tick = run + sleep
*/


//33ms tick once, and ensure game run as 30fps
#define GAME_TICK 33

class GameMain:public Singleton<GameMain>
{

public:
	void Run();
	void Ontick(long diff);
	void Start();
	void Stop();

private :
	long m_diff = 0;
	bool m_start = false;
};


extern "C"
{
	ENGINE_INTERFACE_EXPORT void iStartGame();
	ENGINE_INTERFACE_EXPORT void iStopGame();
}

#endif // ! __GameMain__