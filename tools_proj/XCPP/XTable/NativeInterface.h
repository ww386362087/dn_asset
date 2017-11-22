#pragma once
#include "Common.h"
#include <string>
#include <iostream>

class NativeInterface
{
public:
	NativeInterface(void);
	~NativeInterface(void);
};


extern "C"
{
	typedef void(*CALLBACK)(const char*);
	extern CALLBACK callback;

	ENGINE_INTERFACE_EXPORT void iInitCallbackCommand(CALLBACK cb);
	ENGINE_INTERFACE_EXPORT int iAdd(int, int);
	ENGINE_INTERFACE_EXPORT int iSub(int*, int*);
	ENGINE_INTERFACE_EXPORT char* GetStr();
	ENGINE_INTERFACE_EXPORT void iInitial(const char*);
};
