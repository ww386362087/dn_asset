#include "NativeInterface.h"
#include "FileOpt.h"

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

	int iInitial(char* path)
	{
		LOG("****** hello-world ******");
		XCommand("****** hello-world ******");
		UNITY_STREAMING_PATH = path;
		LOG(path);
		InitLogger(UNITY_STREAMING_PATH+"Log/info.txt",
			UNITY_STREAMING_PATH+"Log/warn.txt",
			UNITY_STREAMING_PATH+"Log/error.txt");
		LOG("init finish");
		return 1;
	}

	void iInitCallbackCommand(CALLBACK cb)
	{
		callback = cb;   
	}

}