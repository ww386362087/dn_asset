#pragma once
#include<iostream>

class LibTable
{
public:
	LibTable(void);
	LibTable(char*);
	~LibTable(void);
	char* Read();

private:
	std::string mTableName;
};

