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

	int iInitial()
	{
		std::cout<<"****** hello-world ******"<<std::endl;
		InitLogger("Log/info.txt","Log/warn.txt","Log/error.txt");
		return 1;
	}

}