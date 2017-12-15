#include "GameTime.h"


static int g_timeOffSet = 0;

namespace GameTime
{

	time_t GetMSTime()
	{
		return time(NULL) + g_timeOffSet;
	}


	void SetTimeOffset(int offset)
	{
		g_timeOffSet = offset;
	}

	void sleep(unsigned milliseconds)
	{
#ifdef _WIN32
		Sleep(milliseconds);
#else
		usleep(milliseconds * 1000);
#endif
	}


	bool IsInSameHour(time_t t1, time_t t2)
	{
		return t1 / 3600 == t2 / 3600;
	}

	bool IsInSameDay(time_t t1, time_t t2, bool offset)
	{
		uint time1 = (uint)t1 + SECONDS_TIME_ZONE;
		uint time2 = (uint)t2 + SECONDS_TIME_ZONE;
		if (offset)
		{
			time1 -= SECONDS_OFFSET;
			time2 -= SECONDS_OFFSET;
		}
		return time1 / SECONDS_ONE_DAY == time2 / SECONDS_ONE_DAY;
	}

	bool IsInSameWeek(time_t t1, time_t t2, bool offset)
	{
		uint time1 = (uint)t1 + SECONDS_TIME_ZONE;
		uint time2 = (uint)t2 + SECONDS_TIME_ZONE;
		if (offset)
		{
			time1 -= SECONDS_OFFSET;
			time2 -= SECONDS_OFFSET;
		}
		int week1 = (int)(time1 / SECONDS_ONE_DAY + BASE_WEEK_DAY - 1) / 7;
		int week2 = (int)(time2 / SECONDS_ONE_DAY + BASE_WEEK_DAY - 1) / 7;
		return week1 == week2;
	}

	uint GetWeekDay(bool offset)
	{
		time_t now = GetMSTime();
		return GetWeekDay(now, offset);
	}

	uint GetWeekDay(time_t t, bool offset)
	{
		/*INT64 time = (INT64)t + SECONDS_TIME_ZONE - SECONDS_OFFSET;
		INT32 wday = (time / SECONDS_ONE_DAY + BASE_WEEK_DAY) % 7;
		return wday == 0 ? 7 : wday;*/

		if (t < SECONDS_OFFSET) return 0;
		if (offset)
		{
			t -= SECONDS_OFFSET;
		}
		tm* pstTime = localtime(&t);
		return pstTime->tm_wday == 0 ? 7 : pstTime->tm_wday;
	}

	uint GetDiffDayCount(time_t t1, time_t t2)
	{
		uint time1 = (uint)t1 + SECONDS_TIME_ZONE - SECONDS_OFFSET;
		uint time2 = (uint)t2 + SECONDS_TIME_ZONE - SECONDS_OFFSET;
		int nNum1 = (int)(time1 / SECONDS_ONE_DAY);
		int nNum2 = (int)(time2 / SECONDS_ONE_DAY);
		return (uint)abs(nNum1 - nNum2);
	}

	int GetDayUnique(time_t t, bool offset)
	{
		uint time = 0;
		if (offset)
		{
			time = (uint)t + SECONDS_TIME_ZONE - SECONDS_OFFSET;
		}
		else
		{
			time = (uint)t + SECONDS_TIME_ZONE;
		}
		return (int)(time / SECONDS_ONE_DAY);
	}

	int GetWeekUnique(time_t t, bool offset)
	{
		uint time = 0;
		if (offset)
		{
			time = (uint)t + SECONDS_TIME_ZONE + (SECONDS_ONE_DAY * 3) - SECONDS_OFFSET;
		}
		else
		{
			time = (uint)t + SECONDS_TIME_ZONE + (SECONDS_ONE_DAY * 3);
		}
		return (int)(time / SECONDS_ONE_WEEK);
	}

	int GetMonthUnique(time_t t, bool offset)
	{
		if (t < SECONDS_OFFSET) return 0;
		if (offset)
		{
			t -= SECONDS_OFFSET;
		}
		struct tm *timeinfo = localtime(&t);
		return timeinfo->tm_year * 1000 + timeinfo->tm_mon;
	}

	int GetTodayUnique(bool offset)
	{
		time_t now = GetMSTime();
		return GetDayUnique(now, offset);
	}

	int GetThisMonthUnique(bool offset)
	{
		time_t now = GetMSTime();
		return GetMonthUnique(now);
	}

	time_t GetDayBeginTime(time_t t, bool offset)
	{
		if (t < SECONDS_OFFSET) return 0;
		uint value = 0;
		if (offset)
		{
			value = SECONDS_OFFSET;
		}
		t -= value;
		struct tm* pstTime = localtime(&t);
		return t - (pstTime->tm_hour * 3600 + pstTime->tm_min * 60 + pstTime->tm_sec) + value;
	}

	time_t GetWeekBeginTime(time_t t, bool offset)
	{
		if (t < SECONDS_OFFSET) return 0;
		uint value = 0;
		if (offset)
		{
			value = SECONDS_OFFSET;
		}
		t -= value;
		struct tm* pstTime = localtime(&t);
		uint week = (pstTime->tm_wday ? pstTime->tm_wday : 7);
		return t - (pstTime->tm_hour * 3600 + pstTime->tm_min * 60 + pstTime->tm_sec + (week - 1) * 86400) + value;
	}

	time_t GetTodayBeginTime(bool offset)
	{
		time_t now = GetMSTime();
		return GetDayBeginTime(now, offset);
	}

	time_t GetThisWeekBeginTime(bool offset)
	{
		time_t now = GetMSTime();
		return GetWeekBeginTime(now, offset);
	}


	time_t MakeTime(time_t dwNowTime, int nHour, int nMin, int nSec)
	{
		tm* stTime = localtime(&dwNowTime);
		stTime->tm_hour = nHour;
		stTime->tm_min = nMin;
		stTime->tm_sec = nSec;
		return mktime(stTime);
	}

	time_t MakeTime(uint year, uint month, uint day)
	{
		tm when;
		when.tm_year = year - 1900;
		when.tm_mon = month - 1;
		when.tm_mday = day;
		when.tm_hour = 0;
		when.tm_min = 0;
		when.tm_sec = 0;
		return mktime(&when);
	}

	time_t MakeTimeByDateTime(uint year, uint month, uint day, uint hour, uint min, uint sec)
	{
		tm when;
		when.tm_year = year - 1900;
		when.tm_mon = month - 1;
		when.tm_mday = day;
		when.tm_hour = hour;
		when.tm_min = min;
		when.tm_sec = sec;
		return mktime(&when);
	}

	bool IsWhithinTime(int nBeginHour, int nBeginMin, int nBeginSec, int nEndHour, int nEndMin, int nEndSec)
	{
		time_t dwNowTime = GetMSTime();
		tm* stTime = localtime(&dwNowTime);
		stTime->tm_hour = nBeginHour;
		stTime->tm_min = nBeginMin;
		stTime->tm_sec = nBeginSec;
		time_t dwBeginTime = mktime(stTime);

		stTime->tm_hour = nEndHour;
		stTime->tm_min = nEndMin;
		stTime->tm_sec = nEndSec;
		time_t dwEndTime = mktime(stTime);

		if (dwNowTime >= dwBeginTime && dwNowTime <= dwEndTime)
		{
			return true;
		}
		return false;
	}


	int GetTodayUniqueTag()
	{
		time_t now = GetMSTime();
		struct tm *timeinfo = localtime(&now);
		return timeinfo->tm_year * 1000 + timeinfo->tm_yday;
	}

#ifdef WIN32
#define snprintf _snprintf
#endif
	std::string GetDateStr(time_t t)
	{
		struct tm *timeinfo = localtime(&t);
		uint year = timeinfo->tm_year + 1900;
		uint month = timeinfo->tm_mon + 1;
		uint day = timeinfo->tm_mday;
		char buff[128] = { '\0' };
		snprintf(buff, sizeof(buff), "%d-%02d-%02d", year, month, day);
		return std::string(buff);
	}


}