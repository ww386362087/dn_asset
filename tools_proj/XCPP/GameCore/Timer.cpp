#include "Timer.h"
#include "XDelegate.h"

Timer::Timer(int time, int loop, XDelegate* handler, uint sequence, IArgs* arg)
{
	if (loop == 0) loop = -1;
	m_totalTime = time;
	m_arg = arg;
	m_loop = loop;
	m_handler = handler;
	m_sequence = sequence;

	m_currentTime = 0;
	m_isRunning = true;
	m_isFinished = false;
}

void Timer::Update(int deltaTime)
{
	if (m_isFinished || !m_isRunning)return;
	
	if (m_loop == 0)
	{
		m_isFinished = true;
	}
	else
	{
		m_currentTime += deltaTime;
		if (m_currentTime >= m_totalTime)
		{
			if (m_handler)(*m_handler)(m_arg, NULL);
			m_currentTime = 0;
			m_loop--;
		}
	}
}

bool Timer::IsFinished()
{
	return m_isFinished;
}

int Timer::GetCurrentTime()
{
	return m_currentTime;
}

void Timer::Pause()
{
	m_isRunning = false;
}

void Timer::Resume()
{
	m_isRunning = true;
}

void Timer::Reset()
{
	m_currentTime = 0;
}

bool Timer::IsSequenceMatched(uint sequence)
{
	return (m_sequence == sequence);
}

