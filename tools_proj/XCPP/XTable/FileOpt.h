#pragma once
#include<string>
#include <fstream>
#include<iostream>
#include"NativeReader.h"
#include <vector>
#include"Log.h"
#include"Common.h"

using namespace std;

struct FileRaw
{
	 string comment;
	 string name;
	 int val;
};

class FileOpt:public NativeReader
{
public:
	void ReadTable();
	void ReadTable(string name);

protected:
	vector<FileRaw> m_data;
};

extern "C"
{
	ENGINE_INTERFACE_EXPORT void iReadTable(const char* name);
};