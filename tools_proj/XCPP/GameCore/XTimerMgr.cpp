#include "XTimerMgr.h"
#include "Timer.h"


XTimerMgr::XTimerMgr()
{
	m_timerSequence = 0;
}

void XTimerMgr::Update(float delta)
{
	AdvanceTimer((int)(delta * 1000));
}

void XTimerMgr::UpdateLogic(int delta)
{
	AdvanceTimer(delta);
}

void XTimerMgr::AdvanceTimer(int delta)
{
	for (size_t i = 0; i < m_timers.size();)
	{
		if (m_timers[i]->IsFinished())
		{
			delete m_timers[i];
			m_timers.erase(m_timers.begin() + i);
			continue;
		}
		m_timers[i]->Update(delta);
		i++;
	}
}

uint XTimerMgr::SetTimer(float time, XDelegate* onTimeUpHandler)
{
	return SetTimer((int)(time * 1000), onTimeUpHandler);
}

uint XTimerMgr::SetTimer(int time, XDelegate* onTimeUpHandler)
{
	return SetTimer(time, onTimeUpHandler, 0);
}

uint XTimerMgr::SetTimer(float time, XDelegate* onTimeUpHandler, IArgs* arg)
{
	return SetTimer((int)(time * 1000), 1, onTimeUpHandler, arg);
}

uint XTimerMgr::SetLoopTimer(float time, XDelegate* onTimeUpHandler, IArgs* arg)
{
	return SetTimer((int)(time * 1000), -1, onTimeUpHandler, arg);
}

uint XTimerMgr::SetTimer(int time, XDelegate* onTimeUpHandler, IArgs* arg)
{
	return SetTimer(time, 1, onTimeUpHandler, arg);
}

uint XTimerMgr::SetTimer(int time, int loop, XDelegate* onTimeUpHandler, IArgs* arg)
{
	m_timerSequence++;
	m_timers.push_back(new Timer(time, loop, onTimeUpHandler, m_timerSequence, arg));
	return m_timerSequence;
}

void XTimerMgr::RemoveTimer(uint sequence)
{
	for (size_t j = 0; j < m_timers.size();)
	{
		if (m_timers[j]->IsSequenceMatched(sequence))
		{
			delete m_timers[j];
			m_timers.erase(m_timers.begin() + j);
			m_timers[j] = NULL;
			return;
		}
		j++;
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
	if (timer) timer->Pause();
}

void XTimerMgr::ResumeTimer(uint sequence)
{
	Timer* timer = GetTimer(sequence);
	if (timer) timer->Resume();
}

void XTimerMgr::ResetTimer(uint sequence)
{
	Timer* timer = GetTimer(sequence);
	if (timer) timer->Reset();
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

void XTimerMgr::RemoveAllTimer()
{
	for (size_t j = 0; j < m_timers.size(); j++)
	{
		delete m_timers[j];
		m_timers[j] = NULL;
	}
	m_timers.clear();
}


Timer* XTimerMgr::GetTimer(uint sequence)
{
	for (size_t j = 0; j < m_timers.size(); j++)
	{
		Timer* t = m_timers[j];
		if (t->IsSequenceMatched(sequence))
		{
			return t;
		}
	}
	return NULL;
}