#pragma once
#include<string>
#include <fstream>
#include<iostream>
#include"BinaryOpt.h"
#include <vector>


using namespace std;

struct FileRaw
{
	 string comment;
	 string name;
	 int val;
};

class FileOpt : public BinaryOpt
{
public:
	void ReadBinary();
	void ReadTable();

protected:
	vector<FileRaw> m_data;
};

