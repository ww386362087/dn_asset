#ifndef  __Timer__
#define  __Timer__

#include "Common.h"
#include "XTimerMgr.h"
#include "XDelegate.h"

class Timer
{
public:
	Timer(int time, int loop, XDelegate* handler, uint sequence, IArgs* arg);
	void Update(int deltaTime);
	bool IsFinished();
	int GetCurrentTime();
	void Pause();
	void Resume();
	void Reset();
	bool IsSequenceMatched(uint sequence);


private:
	XDelegate* m_handler;
	IArgs* m_arg;
	int m_loop;
	int m_totalTime;
	int m_currentTime;
	bool m_isFinished;
	bool m_isRunning;
	uint m_sequence;
};

#endif