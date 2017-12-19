#ifndef  __TimerMgr__
#define  __TimerMgr__

#include "Common.h"
#include <vector>
#include "Singleton.h"
#include "ITimerCallback.h"

class Timer;

enum  enTimerType
{
	Normal,
	FrameSync
};

#define TIMERCNT  2

class XTimerMgr:public Singleton<XTimerMgr>
{
public:
	XTimerMgr();
	void Update(float delta);
	void UpdateLogic(int delta);
	uint SetTimer(float time, ITimerCallback* onTimeUpHandler);
	uint SetTimer(int time, ITimerCallback* onTimeUpHandler);
	uint SetTimer(float time, ITimerCallback* onTimeUpHandler, ITimerArg* arg);
	uint SetLoopTimer(float time, ITimerCallback* onTimeUpHandler, ITimerArg* arg);
	uint SetTimer(int time, ITimerCallback* onTimeUpHandler, ITimerArg* arg);
	uint SetTimer(int time, int loop, ITimerCallback* onTimeUpHandler, ITimerArg* arg);
	uint SetTimer(int time, int loop, ITimerCallback* onTimeUpHandler, ITimerArg* arg, bool useFrameSync);
	void RemoveTimer(uint sequence);
	void RemoveTimerSafely(uint& sequence);
	void PauseTimer(uint sequence);
	void ResumeTimer(uint sequence);
	void ResetTimer(uint sequence);
	int GetTimerCurrent(uint sequence);
	void RemoveTimer(ITimerCallback* onTimeUpHandler);
	void RemoveTimer(ITimerCallback* onTimeUpHandler, bool useFrameSync);
	void RemoveAllTimer(bool useFrameSync);
	void RemoveAllTimer();


private:
	void AdvanceTimer(int delta, enTimerType timerType);
	Timer* GetTimer(uint sequence);
	std::vector<Timer*> m_timers[TIMERCNT];
	uint m_timerSequence;
	
};

#endif