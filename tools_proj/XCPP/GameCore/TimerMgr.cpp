#include "TimerMgr.h"
#include "Timer.h"


TimerMgr::TimerMgr()
{
	m_timerSequence = 0;
}

void TimerMgr::Update(float delta)
{
	AdvanceTimer((int)(delta * 1000), Normal);
}

void TimerMgr::UpdateLogic(int delta)
{
	AdvanceTimer(delta, FrameSync);
}

void TimerMgr::AdvanceTimer(int delta, enTimerType timerType)
{
	if (m_timers)
	{
		std::vector<Timer*> timers = m_timers[(int)timerType];
		for (size_t i = 0; i < timers.size();)
		{
			if (timers[i]->IsFinished())
			{
				timers.erase(timers.begin() + i);
				continue;
			}

			timers[i]->Update(delta);
			i++;
		}
	}
}

uint TimerMgr::SetTimer(float time, OnTimerUpdate onTimeUpHandler)
{
	return SetTimer((int)(time * 1000), onTimeUpHandler);
}

uint TimerMgr::SetTimer(int time, OnTimerUpdate onTimeUpHandler)
{
	return SetTimer(time, onTimeUpHandler, 0);
}

uint TimerMgr::SetTimer(float time, OnTimerUpdate onTimeUpHandler, uint arg)
{
	return SetTimer((int)(time * 1000), 1, onTimeUpHandler, arg);
}

uint TimerMgr::SetLoopTimer(float time, OnTimerUpdate onTimeUpHandler, uint arg)
{
	return SetTimer((int)(time * 1000), -1, onTimeUpHandler, arg);
}

uint TimerMgr::SetTimer(int time, OnTimerUpdate onTimeUpHandler, uint arg)
{
	return SetTimer(time, 1, onTimeUpHandler, arg);
}

uint TimerMgr::SetTimer(int time, int loop, OnTimerUpdate onTimeUpHandler, uint arg)
{
	return SetTimer(time, loop, onTimeUpHandler, arg, false);
}

uint TimerMgr::SetTimer(int time, int loop, OnTimerUpdate onTimeUpHandler, uint arg, bool useFrameSync)
{
	m_timerSequence++;
	m_timers[(int)(useFrameSync ? FrameSync : Normal)].push_back(new Timer(time, loop, onTimeUpHandler, m_timerSequence, arg));
	return m_timerSequence;
}

void TimerMgr::RemoveTimer(uint sequence)
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
				return;
			}
			j++;
		}
	}
}

void TimerMgr::RemoveTimerSafely(uint& sequence)
{
	if (sequence != 0)
	{
		RemoveTimer(sequence);
		sequence = 0;
	}
}

void TimerMgr::PauseTimer(uint sequence)
{
	Timer* timer = GetTimer(sequence);
	if (timer)
	{
		timer->Pause();
	}
}

void TimerMgr::ResumeTimer(uint sequence)
{
	Timer* timer = GetTimer(sequence);
	if (timer)
	{
		timer->Resume();
	}
}

void TimerMgr::ResetTimer(uint sequence)
{
	Timer* timer = GetTimer(sequence);
	if (timer)
	{
		timer->Reset();
	}
}

int TimerMgr::GetTimerCurrent(uint sequence)
{
	Timer* timer = GetTimer(sequence);
	if (timer)
	{
		return timer->GetCurrentTime();
	}
	return -1;
}

void TimerMgr::RemoveTimer(OnTimerUpdate onTimeUpHandler)
{
	RemoveTimer(onTimeUpHandler, false);
}

void TimerMgr::RemoveTimer(OnTimerUpdate onTimeUpHandler, bool useFrameSync)
{
	std::vector<Timer*> timers = m_timers[(int)(useFrameSync ? FrameSync : Normal)];
	for (size_t i = 0; i < timers.size();)
	{
		if (timers[i]->IsDelegateMatched(onTimeUpHandler))
		{
			delete timers[i];
			timers.erase(timers.begin() + i);
			continue;
		}
		i++;
	}
}

void TimerMgr::RemoveAllTimer(bool useFrameSync)
{
	m_timers[(int)(useFrameSync ? FrameSync : Normal)].clear();
}

void TimerMgr::RemoveAllTimer()
{
	for (int i = 0; i < TIMERCNT; i++)
	{
		std::vector<Timer*> timers = m_timers[i];
		for (size_t j = 0; j < timers.size(); j++)
		{
			delete timers[j];
		}
		timers.clear();
	}
}


Timer* TimerMgr::GetTimer(uint sequence)
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