#include "XEventArgs.h"



XEventArgs::XEventArgs()
{
}


XEventArgs::~XEventArgs()
{
}


void XEventArgs::Debug()
{
}


XEventDefine XEventArgs::GetEventDef()
{
	return _argsDefine;
}


void XEventArgs::SetEventDef(XEventDefine def)
{
	_eDefine = def;
}