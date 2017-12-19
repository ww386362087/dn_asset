#include "XAttributes.h"


uint XAttributes::getid()
{
	return _id;
}

void XAttributes::setid(uint val)
{
	_id = val;
}

Vector3* XAttributes::getAppearPostion()
{
	return _pos;
}

void XAttributes::setAppearPosition(Vector3* val)
{
	_pos = val;
}

Vector3* XAttributes::getAppearQuaternion()
{
	return _rot;
}

void XAttributes::setAppearQuaternion(Vector3* rot)
{
	_rot = rot;
}

uint XAttributes::getPresentID()
{
	return _presentID;
}

void XAttributes::setPresentID(uint value)
{
	_presentID = value;
}

bool XAttributes::getDead()
{
	return _dead;
}

void XAttributes::setDead(bool val)
{
	_dead = val;
}

int XAttributes::getFightGroup()
{
	return _group;
}

void XAttributes::setFightGroup(int value)
{
	_group = value;
}

bool XAttributes::getBlocked()
{
	return _block;
}


void XAttributes::setBlocked(bool val)
{
	_block = val;
}