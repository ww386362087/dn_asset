#include "NativeInterface.h"
#include "QteStatusList.h"
#include "Common.h"

NativeInterface::NativeInterface(void)
{
}


NativeInterface::~NativeInterface(void)
{
}


extern "C"
{
	std::string UNITY_STREAM_PATH;
	std::string UNITY_CACHE_PATH;

	int iAdd(int a, int b)
	{
		return a+b;
	}

	int iSub(int* a, int* b)
	{
		return *a - *b;
	}

	void iInitial(const char* stream,const char* cache)
	{
		std::string s = cache;
		UNITY_STREAM_PATH = stream;
		UNITY_CACHE_PATH = cache;
		LOG(UNITY_STREAM_PATH);
		InitLogger(s+"Log/info.txt",s+"Log/warn.txt",s+"Log/error.txt");
		LOG("c++ initial success with path: "+ s);
	}

	void iInitCallbackCommand(CALLBACK cb)
	{
		callback = cb;   
	}

}