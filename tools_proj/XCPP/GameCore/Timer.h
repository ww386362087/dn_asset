#ifndef  __Timer__
#define  __Timer__

#include "Common.h"
#include "TimerMgr.h"

class Timer
{
public:
	Timer(int time, int loop, OnTimerUpdate handler, uint sequence, uint arg);
	void Update(int deltaTime);
	bool IsFinished();
	int GetCurrentTime();
	void Pause();
	void Resume();
	void Reset();
	bool IsSequenceMatched(uint sequence);
	bool IsDelegateMatched(OnTimerUpdate handler);


private:
	OnTimerUpdate m_timeUpHandler;
	uint m_arg;
	int m_loop;
	int m_totalTime;
	int m_currentTime;
	bool m_isFinished;
	bool m_isRunning;
	uint m_sequence;
};

#endif