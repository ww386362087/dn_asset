#ifndef  __GameTime__
#define  __GameTime__

#include "Common.h"
#include <time.h>  

#ifdef _WIN32
#include <windows.h>
#else
#include <unistd.h>
#endif


#define HOUR_ONE_DAY_BEGIN	5
#define SECONDS_ONE_DAY     (3600 * 24)
#define SECONDS_ONE_WEEK    (3600 * 24 * 7)
#define SECONDS_TIME_ZONE	(3600 * 8)
#define SECONDS_OFFSET		(3600 * HOUR_ONE_DAY_BEGIN)
#define BASE_WEEK_DAY        4              /* 1970-01-01: Thursday */


namespace GameTime
{
	time_t GetMSTime();
	void SetTimeOffset(int offset);
	void sleep(unsigned milliseconds);

	//Same Check
	bool IsInSameHour(time_t t1, time_t t2);
	bool IsInSameDay(time_t t1, time_t t2, bool offset);
	bool IsInSameWeek(time_t t1, time_t t2, bool offset);

	//返回周几 [1, 7]
	uint GetWeekDay(bool offset = true);
	uint GetWeekDay(time_t t, bool offset = true);

	//获取相差天数
	uint GetDiffDayCount(time_t t1, time_t t2);


	//获取day/month唯一编号
	int GetDayUnique(time_t t, bool offset = true);
	int GetWeekUnique(time_t t, bool offset = true);
	int GetMonthUnique(time_t t, bool offset = true);
	int GetTodayUnique(bool offset = true);
	int GetThisMonthUnique(bool offset = true);
	int GetTodayUniqueTag();


	//获取一天/一周开始时间
	time_t GetDayBeginTime(time_t t, bool offset = true);
	time_t GetWeekBeginTime(time_t t, bool offset = true);
	time_t GetTodayBeginTime(bool offset = true);
	time_t GetThisWeekBeginTime(bool offset = true);


	time_t MakeTime(time_t dwNowTime, int nHour, int nMin, int nSec);
	time_t MakeTime(uint year, uint month, uint day);
	time_t MakeTimeByDateTime(uint year, uint month, uint day, uint hour = 0, uint min = 0, uint sec = 0);
	bool IsWhithinTime(int nBeginHour, int nBeginMin, int nBeginSec, int nEndHour, int nEndMin, int nEndSec);

	std::string GetDateStr(time_t t);

	static int g_timeOffSet;

};

#endif
