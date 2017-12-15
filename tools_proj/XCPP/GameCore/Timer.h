#ifndef  __timer__
#define  __timer__

#include "Common.h"


class ITimer
{
public:
	virtual ~ITimer() {}
	
	virtual void OnTimer(uint dwID, uint dwCount) = 0;
};



class  ITimerMgr
{
public:
	virtual	~ITimerMgr() {}

	//poTimer:		Timer回调接口
	//dwID:			定时Identifier
	//dwInterval:	定时间隔，毫秒为单位	
	//nCount:		触发次数, -1为永远触发 
	//返回token
	virtual uint SetTimer(ITimer* pTimer, uint dwID, uint dwInterval, uint dwCount) = 0;

	virtual uint GetTimeLeft(uint token) = 0;

	virtual void KillTimer(uint token) = 0;

	virtual void Update() = 0;

	virtual void Release() = 0;
};


#endif