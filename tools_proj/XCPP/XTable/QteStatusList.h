#pragma once
#include<string>
#include <fstream>
#include<iostream>
#include"NativeReader.h"
#include <vector>
#include"Log.h"

using namespace std;

struct QteStatusListRow
{
	 char comment[MaxStringSize];
	 char name[MaxStringSize];
	 //string comment;
	 //string name;
	 int val;
};

class QteStatusList:public NativeReader
{
public:
	void ReadTable();
	void ReadTable(string name);
	void GetRow(int val,QteStatusListRow* row);

protected:
	vector<QteStatusListRow> m_data;
};

extern "C"
{
	ENGINE_INTERFACE_EXPORT void iReadTable(const char* name);
	ENGINE_INTERFACE_EXPORT void iGetRow(int val,QteStatusListRow* row);
};