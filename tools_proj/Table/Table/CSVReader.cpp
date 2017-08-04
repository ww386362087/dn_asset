#include "CSVReader.h"
#include <sstream> 
#include<string>

CSVReader::~CSVReader(void)
{
}

bool CSVReader::LoadFile(const std::string &filename)
{
	

	return true;
}

bool CSVReader::LoadBuffer(std::string &buffer, const char *filename)
{
	return true;
}


//½âÎö±íÍ·
bool CSVReader::MapColHeader(const char **ColHeader, int HeaderCount, char **Fields, int FieldCount)
{
	return true;
}

bool CSVReader::Parse(const char *Field, bool &b)
{
	std::string s=Field;
	std::stringstream ss;
	ss<<s;
	ss>>b;
	return true;
}

bool CSVReader::Parse(const char *Field, int &output)
{
	std::string s=Field;
	std::stringstream ss;
	ss<<s;
	ss<<output;
	return true;
}

bool CSVReader::Parse(const char *Field, uint &b)
{
	std::string s=Field;
	std::stringstream ss;
	ss<<s;
	ss<<b;
	return true;
}

bool CSVReader::Parse(const char *Field, float &output)
{
	std::string s=Field;
	std::stringstream ss;
	ss<<s;
	ss<<output;
	return true;
}

bool CSVReader::Parse(const char *Field, double &b)
{
	std::string s=Field;
	std::stringstream ss;
	ss<<s;
	ss<<b;
	return true;
}

bool CSVReader::Parse(const char *Field, long long &output)
{
	std::string s=Field;
	std::stringstream ss;
	ss<<s;
	ss<<output;
	return true;
}

bool CSVReader::Parse(const char *Field, std::string &output)
{
	std::string s=Field;
	std::stringstream ss;
	ss<<s;
	ss<<output;
	return true;
}