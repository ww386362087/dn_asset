#ifndef  __ITimerCallback__
#define  __ITimerCallback__


class ITimerArg
{
public:
	virtual ~ITimerArg() {};
};


class ITimerCallback
{
public:
	virtual ~ITimerCallback() {};
	virtual void TimerCallback(ITimerArg* arg) = 0;
};


#endif