#pragma once
#include<iostream>
#include<vector>

typedef unsigned int uint;

class CSVReader
{
public:
	CSVReader() { m_ColNum = 0;}
	virtual ~CSVReader();

	bool LoadFile(const std::string &filename);
	bool LoadBuffer(std::string &buffer, const char *filename="");

	virtual bool OnHeaderLine(char **Fields, int FieldCount) = 0;
	virtual bool OnCommentLine(char **Fields, int FieldCount) = 0;
	virtual bool OnLine(char **Fields, int FieldCount) = 0;

	bool MapColHeader(const char **ColHeader, int HeaderCount, char **Fields, int FieldCount);

	bool Parse(const char *Field, bool &b);
	bool Parse(const char *Field, int &output);
	bool Parse(const char *Field, uint &output);
	bool Parse(const char *Field, float &output);
	bool Parse(const char *Field, double &output);
	bool Parse(const char *Field, long long &output);
	bool Parse(const char *Field, std::string &output);



protected:
	int m_ColNum;
	std::vector<int> m_ColMap;
};

