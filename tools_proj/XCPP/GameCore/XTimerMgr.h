#ifndef  __TimerMgr__
#define  __TimerMgr__

#include "Common.h"
#include <vector>
#include "Singleton.h"
#include "XDelegate.h"

class Timer;

class XTimerMgr:public Singleton<XTimerMgr>
{
public:
	XTimerMgr();
	void Update(float delta);
	void UpdateLogic(int delta);
	uint SetTimer(float time, XDelegate* onTimeUpHandler);
	uint SetTimer(int time, XDelegate* onTimeUpHandler);
	uint SetTimer(float time, XDelegate* onTimeUpHandler, IArgs* arg);
	uint SetLoopTimer(float time, XDelegate* onTimeUpHandler, IArgs* arg);
	uint SetTimer(int time, XDelegate* onTimeUpHandler, IArgs* arg);
	uint SetTimer(int time, int loop, XDelegate* onTimeUpHandler, IArgs* arg);
	void RemoveTimer(uint sequence);
	void RemoveTimerSafely(uint& sequence);
	void PauseTimer(uint sequence);
	void ResumeTimer(uint sequence);
	void ResetTimer(uint sequence);
	int GetTimerCurrent(uint sequence);
	void RemoveAllTimer();
	
private:
	void AdvanceTimer(int delta);
	Timer* GetTimer(uint sequence);
	std::vector<Timer*> m_timers;
	uint m_timerSequence;
};

#endif