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
	ENGINE_INTERFACE_EXPORT std::string  iDesc();
	ENGINE_INTERFACE_EXPORT int iAdd(int *a, int* b);
	ENGINE_INTERFACE_EXPORT int iSub(int* a, int* b);
	ENGINE_INTERFACE_EXPORT bool iInitial();
};