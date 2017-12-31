#ifndef  __GameMain__
#define  __GameMain__
 
#include "Common.h"
#include "Singleton.h"
#include "XTimerMgr.h"
#include <ctime> 
#include "Vector3.h"
#include "XDelegate.h"
/*
 *	fps:30 tick = run + sleep
 */


//33ms tick once, and ensure game run as 30fps
#define GAME_TICK 33

class GameMain:public Singleton<GameMain>
{
public:
	void Start();
	void Stop();
	void Quit();
	void Ontick(float delta);

private:
	void OnStart();
	
private :
	long m_diff = 0;
	bool m_start = false;
};


#endif // ! __GameMain__