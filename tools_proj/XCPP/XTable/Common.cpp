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