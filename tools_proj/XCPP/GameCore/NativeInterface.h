#ifndef __NativeInterface__
#define __NativeInterface__


#include "Common.h"
#include <iostream>
#include "Log.h"
#include "Vector3.h"
#include "tinyxml2.h"
#include <string>

extern "C"
{
	typedef bool(*SharpCALLBACK)(unsigned char,const char*);
	typedef void(*EntyCallBack)(int entityid, const char* method, const char* arg);
	typedef void(*CompCallBack)(int entityid, const char* compnent, const char* arg);
	extern SharpCALLBACK callback;
	extern EntyCallBack eCallback;
	extern CompCallBack cCallback;

	ENGINE_INTERFACE_EXPORT void iInitCallbackCommand(SharpCALLBACK cb);
	ENGINE_INTERFACE_EXPORT void iInitEntityCall(EntyCallBack cb);
	ENGINE_INTERFACE_EXPORT void iInitCompnentCall(CompCallBack cb);
	ENGINE_INTERFACE_EXPORT int iAdd(int, int);
	ENGINE_INTERFACE_EXPORT int iSub(int*, int*);
	ENGINE_INTERFACE_EXPORT void iInitial(const char*,const char*);
	ENGINE_INTERFACE_EXPORT void iJson(const char*);
	ENGINE_INTERFACE_EXPORT void iPatch(const char*,const char*,const char*);
	ENGINE_INTERFACE_EXPORT void iVector();
	ENGINE_INTERFACE_EXPORT void iStartCore();
	ENGINE_INTERFACE_EXPORT void iStopCore();
	ENGINE_INTERFACE_EXPORT void iTickCore(float delta);
};


#endif