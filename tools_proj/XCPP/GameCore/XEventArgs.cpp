#include "XEventArgs.h"



XEventDefine XEventArgs::getEventDef()
{
	return _argsDefine;
}


void XEventArgs::setEventDef(XEventDefine def)
{
	_eDefine = def;
}