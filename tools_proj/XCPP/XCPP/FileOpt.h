#pragma once
#include<string>
#include <fstream>
#include<iostream>
#include"BinaryOpt.h"
using namespace std;

class FileOpt:public BinaryOpt
{
public:
	void ReadBinary();
	void WriteBinary();

private:
	 static const int len = 256;
};

