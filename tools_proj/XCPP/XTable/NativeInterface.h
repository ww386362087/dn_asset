#pragma once
#include "Common.h"
#include <string>
#include <iostream>
#include "Log.h"


extern "C"
{
	typedef void(*CALLBACK)(unsigned char,const char*);
	extern CALLBACK callback;

	ENGINE_INTERFACE_EXPORT void iInitCallbackCommand(CALLBACK cb);
	ENGINE_INTERFACE_EXPORT int iAdd(int, int);
	ENGINE_INTERFACE_EXPORT int iSub(int*, int*);
	ENGINE_INTERFACE_EXPORT void iInitial(const char*,const char*);
	ENGINE_INTERFACE_EXPORT void iJson();
};
