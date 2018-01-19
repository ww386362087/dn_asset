#ifndef  __XMonster__
#define  __XMonster__

#include "XEntity.h"


class XMonster :public XEntity
{
public:
	XMonster();
	~XMonster();
	virtual EntityType GetType();
};

#endif