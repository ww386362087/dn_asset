#include "NativeInterface.h"
#include "QteStatusList.h"

NativeInterface::NativeInterface(void)
{
}


NativeInterface::~NativeInterface(void)
{
}


extern "C"
{

	int iAdd(int a, int b)
	{
		return a+b;
	}

	int iSub(int* a, int* b)
	{
		return *a - *b;
	}

	char* GetStr()
	{
		return "ÖÐÎÄ";
	}

	void iInitial(const char* path)
	{
		string s = path;
		InitLogger(s+"Log/info.txt",s+"Log/warn.txt",s+"Log/error.txt");
		LOG("c++ initial success with path: "+ s);
	}

	void iInitCallbackCommand(CALLBACK cb)
	{
		callback = cb;   
	}

}