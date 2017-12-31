#include "XAttributes.h"


XAttributes::XAttributes()
{
	AiBehavior = "hello world";
}

XAttributes::~XAttributes()
{

}

uint XAttributes::getid()
{
	return _id;
}

void XAttributes::setid(uint val)
{
	_id = val;
}

Vector3 XAttributes::getAppearPostion()
{
	return _pos;
}

void XAttributes::setAppearPosition(Vector3 val)
{
	_pos = val;
}

Vector3 XAttributes::getAppearQuaternion()
{
	return _rot;
}

void XAttributes::setAppearQuaternion(Vector3 rot)
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


const char* XAttributes::getName()
{
	return _name;
}

void XAttributes::setName(const char* val)
{
	_name = val;
}
const char* XAttributes::getPrefab()
{
	return _prefab_name;
}
void XAttributes::setPrefab(const char* val)
{
	_prefab_name = val;
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

double XAttributes::GetAttr(XAttributeDefine def)
{
	return 0;
}

uint  XAttributes::getTypeID()
{
	return _type_id;
}

const char* XAttributes::getAiBehaviour()
{
	return AiBehavior;
}


void XAttributes::setAIBehaviour(const char* ai)
{
	AiBehavior = ai;
}