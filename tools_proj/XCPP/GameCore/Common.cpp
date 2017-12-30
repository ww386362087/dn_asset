#include "Common.h"

std::string UNITY_STREAM_PATH;
std::string UNITY_CACHE_PATH;

void tobytes(std::string str)
{
	size_t len = str.length();
	for(size_t i=0;i<len;i++)
	{
		printf("0x%x,",(unsigned char)str[i]);  
	}
	printf("\n");
}

void InitPath(std::string stream,std::string cache)
{
	UNITY_STREAM_PATH = stream;
	UNITY_CACHE_PATH = cache;
}


uint xhash(const char* pChar)
{
	if(pChar == NULL) return 0;
	uint hash = 0;
	size_t len = strlen(pChar);
	for (size_t i = 0; i < len; ++i)
	{
		hash = (hash << 5) + hash + pChar[i];
	}
	return hash;
}

void split(const std::string& s, std::vector<std::string>& v, const std::string& c)
{
	std::string::size_type pos1, pos2;
	pos2 = s.find(c);
	pos1 = 0;
	while (std::string::npos != pos2)
	{
		v.push_back(s.substr(pos1, pos2 - pos1));

		pos1 = pos2 + c.size();
		pos2 = s.find(c, pos1);
	}
	if (pos1 != s.length())
		v.push_back(s.substr(pos1));
}

int new_id()
{
	return id++;
}


bool isNumber(const std::string& value)
{
	const char* str = value.c_str();
	while ((*str++ != 0) && (*str >='0') && (*str <='9'));
	return *str == 0;
}

int countUTF8Char(const std::string &s)
{
	const int mask = 0;
	int Count = 0;
	for (size_t i = 0; i < s.size(); ++i)
	{
		unsigned char c = (unsigned char)s[i];
		if (c <= 0x7f || c >= 0xc0)
		{
			++Count;
		}
	}
	return Count;
}


std::string readFile(const char* file)
{
	std::ifstream ifile(file);
	std::ostringstream buf;
	char ch;
	while (buf&&ifile.get(ch)) buf.put(ch);
	ifile.close();
	return buf.str();
}