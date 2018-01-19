#ifndef  __XRole__
#define  __XRole__

#include "XEntity.h"


class XRole:public XEntity
{
public:
	XRole();
	~XRole();
	virtual EntityType GetType();
	
};

#endif