#ifndef  __TimerMgr__
#define  __TimerMgr__

#include "Common.h"
#include<vector>
#include "Singleton.h"

class Timer;

typedef void(*OnTimerUpdate)(uint arg);

enum  enTimerType
{
	Normal,
	FrameSync
};

#define TIMERCNT  2

class TimerMgr:public Singleton<TimerMgr>
{
public:
	TimerMgr();
	void Update(float delta);
	void UpdateLogic(int delta);
	uint SetTimer(float time, OnTimerUpdate onTimeUpHandler);
	uint SetTimer(int time, OnTimerUpdate onTimeUpHandler);
	uint SetTimer(float time, OnTimerUpdate onTimeUpHandler, uint arg);
	uint SetLoopTimer(float time, OnTimerUpdate onTimeUpHandler, uint arg);
	uint SetTimer(int time, OnTimerUpdate onTimeUpHandler, uint arg);
	uint SetTimer(int time, int loop, OnTimerUpdate onTimeUpHandler, uint arg);
	uint SetTimer(int time, int loop, OnTimerUpdate onTimeUpHandler, uint arg, bool useFrameSync);
	void RemoveTimer(uint sequence);
	void RemoveTimerSafely(uint& sequence);
	void PauseTimer(uint sequence);
	void ResumeTimer(uint sequence);
	void ResetTimer(uint sequence);
	int GetTimerCurrent(uint sequence);
	void RemoveTimer(OnTimerUpdate onTimeUpHandler);
	void RemoveTimer(OnTimerUpdate onTimeUpHandler, bool useFrameSync);
	void RemoveAllTimer(bool useFrameSync);
	void RemoveAllTimer();


private:
	void AdvanceTimer(int delta, enTimerType timerType);
	Timer* GetTimer(uint sequence);
	std::vector<Timer*> m_timers[TIMERCNT];
	uint m_timerSequence;
	
};

#endif