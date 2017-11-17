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
	static CALLBACK callback;    
	static std::string UNITY_STREAMING_PATH;

	ENGINE_INTERFACE_EXPORT void iInitCallbackCommand(CALLBACK cb);
	ENGINE_INTERFACE_EXPORT std::string  iDesc();
	ENGINE_INTERFACE_EXPORT int iAdd(int, int);
	ENGINE_INTERFACE_EXPORT int iSub(int*, int*);
	ENGINE_INTERFACE_EXPORT int iInitial(char*);
};

#define XCommand(text) callback(text)