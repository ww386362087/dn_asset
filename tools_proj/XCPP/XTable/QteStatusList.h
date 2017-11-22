#pragma once
#include"NativeReader.h"
#include <vector>
#include"Log.h"
#include"Common.h"

struct QteStatusListRow
{
	 char comment[MaxStringSize];
	 char name[MaxStringSize];
	 int val;
};

class QteStatusList:public NativeReader
{
public:
	QteStatusList(void);
	void ReadTable();
	void GetRow(int val,QteStatusListRow* row);
	int GetLength();

protected:
	std::string name;
	std::vector<QteStatusListRow> m_data;
};


extern std::string UNITY_STREAM_PATH;

extern "C"
{
	ENGINE_INTERFACE_EXPORT int iGetQteStatueListLength();
	ENGINE_INTERFACE_EXPORT void iGetQteStatusListRow(int val,QteStatusListRow* row);
};