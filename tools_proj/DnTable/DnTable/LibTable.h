#pragma once

class LibTable
{
public:
	LibTable(void);
	LibTable(char*);
	~LibTable(void);
	char* Read();

private:
	char* mTableName;

};

