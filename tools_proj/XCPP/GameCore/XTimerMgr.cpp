#include "XTimerMgr.h"
#include "Timer.h"


XTimerMgr::XTimerMgr()
{
	m_timerSequence = 0;
}

void XTimerMgr::Update(float delta)
{
	AdvanceTimer((int)(delta * 1000), Normal);
}

void XTimerMgr::UpdateLogic(int delta)
{
	AdvanceTimer(delta, FrameSync);
}

void XTimerMgr::AdvanceTimer(int delta, enTimerType timerType)
{
	if (m_timers)
	{
		std::vector<Timer*> timers = m_timers[(int)timerType];
		for (size_t i = 0; i < timers.size();)
		{
			if (timers[i]->IsFinished())
			{
				delete timers[i];
				timers.erase(timers.begin() + i);
				timers[i] = NULL;
				continue;
			}

			timers[i]->Update(delta);
			i++;
		}
	}
}

uint XTimerMgr::SetTimer(float time, ITimerCallback* onTimeUpHandler)
{
	return SetTimer((int)(time * 1000), onTimeUpHandler);
}

uint XTimerMgr::SetTimer(int time, ITimerCallback* onTimeUpHandler)
{
	return SetTimer(time, onTimeUpHandler, 0);
}

uint XTimerMgr::SetTimer(float time, ITimerCallback* onTimeUpHandler, ITimerArg* arg)
{
	return SetTimer((int)(time * 1000), 1, onTimeUpHandler, arg);
}

uint XTimerMgr::SetLoopTimer(float time, ITimerCallback* onTimeUpHandler, ITimerArg* arg)
{
	return SetTimer((int)(time * 1000), -1, onTimeUpHandler, arg);
}

uint XTimerMgr::SetTimer(int time, ITimerCallback* onTimeUpHandler, ITimerArg* arg)
{
	return SetTimer(time, 1, onTimeUpHandler, arg);
}

uint XTimerMgr::SetTimer(int time, int loop, ITimerCallback* onTimeUpHandler, ITimerArg* arg)
{
	return SetTimer(time, loop, onTimeUpHandler, arg, false);
}

uint XTimerMgr::SetTimer(int time, int loop, ITimerCallback* onTimeUpHandler, ITimerArg* arg, bool useFrameSync)
{
	m_timerSequence++;
	m_timers[(int)(useFrameSync ? FrameSync : Normal)].push_back(new Timer(time, loop, onTimeUpHandler, m_timerSequence, arg));
	return m_timerSequence;
}

void XTimerMgr::RemoveTimer(uint sequence)
{
	for (int i = 0; i < TIMERCNT; i++)
	{
		std::vector<Timer*> timers = m_timers[i];
		for (size_t j = 0; j < timers.size();)
		{
			if (timers[j]->IsSequenceMatched(sequence))
			{
				delete timers[j];
				timers.erase(timers.begin() + j);
				timers[j] = NULL;
				return;
			}
			j++;
		}
	}
}

void XTimerMgr::RemoveTimerSafely(uint& sequence)
{
	if (sequence != 0)
	{
		RemoveTimer(sequence);
		sequence = 0;
	}
}

void XTimerMgr::PauseTimer(uint sequence)
{
	Timer* timer = GetTimer(sequence);
	if (timer)
	{
		timer->Pause();
	}
}

void XTimerMgr::ResumeTimer(uint sequence)
{
	Timer* timer = GetTimer(sequence);
	if (timer)
	{
		timer->Resume();
	}
}

void XTimerMgr::ResetTimer(uint sequence)
{
	Timer* timer = GetTimer(sequence);
	if (timer)
	{
		timer->Reset();
	}
}

int XTimerMgr::GetTimerCurrent(uint sequence)
{
	Timer* timer = GetTimer(sequence);
	if (timer)
	{
		return timer->GetCurrentTime();
	}
	return -1;
}

void XTimerMgr::RemoveTimer(ITimerCallback* onTimeUpHandler)
{
	RemoveTimer(onTimeUpHandler, false);
}

void XTimerMgr::RemoveTimer(ITimerCallback* onTimeUpHandler, bool useFrameSync)
{
	std::vector<Timer*> timers = m_timers[(int)(useFrameSync ? FrameSync : Normal)];
	for (size_t i = 0; i < timers.size();)
	{
		if (timers[i]->IsDelegateMatched(onTimeUpHandler))
		{
			delete timers[i];
			timers.erase(timers.begin() + i);
			timers[i] = NULL;
			continue;
		}
		i++;
	}
}

void XTimerMgr::RemoveAllTimer(bool useFrameSync)
{
	m_timers[(int)(useFrameSync ? FrameSync : Normal)].clear();
}

void XTimerMgr::RemoveAllTimer()
{
	for (int i = 0; i < TIMERCNT; i++)
	{
		std::vector<Timer*> timers = m_timers[i];
		for (size_t j = 0; j < timers.size(); j++)
		{
			delete timers[j];
			timers[j] = NULL;
		}
		timers.clear();
	}
}


Timer* XTimerMgr::GetTimer(uint sequence)
{
	for (int i = 0; i < TIMERCNT; i++)
	{
		std::vector<Timer*> timer = m_timers[i];
		for (size_t j = 0; j < timer.size(); j++)
		{
			Timer* t = timer[j];
			if (t->IsSequenceMatched(sequence))
			{
				return t;
			}
		}
	}
	return NULL;
}