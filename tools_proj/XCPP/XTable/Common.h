#ifndef __Common__
#define __Common__

#if defined(__CYGWIN32__)
#define ENGINE_INTERFACE_EXPORT __declspec(dllexport)
#elif defined(WIN32) || defined(_WIN32) || defined(__WIN32__) || defined(_WIN64) || defined(WINAPI_FAMILY)
#define ENGINE_INTERFACE_EXPORT __declspec(dllexport)
#elif defined(__MACH__) || defined(__ANDROID__) || defined(__linux__) || defined(__QNX__)
#define ENGINE_INTERFACE_EXPORT
#else
#define ENGINE_INTERFACE_EXPORT
#endif

#include <sstream> 
#include<string>
#include"picojson.h"

typedef unsigned int  uint;
#define MaxStringSize 64
#define MaxArraySize 16

#define CLog 'L'
#define CWarn 'W'
#define CError 'E'

extern std::string UNITY_STREAM_PATH;
extern std::string UNITY_CACHE_PATH;

template<typename T>
std::string tostring(T val)
{
	std::stringstream ss;
	std::string str;
	ss<<val;
	ss>>str;
	return str;
}

template<typename T>
T Add(T a,T b)
{
	return a+b;
}

void tobytes(std::string str);

void InitPath(std::string stream,std::string cache);

uint xhash(const char* ch);

bool isNumber(const std::string& value);

int countUTF8Char(const std::string &s);

#endif