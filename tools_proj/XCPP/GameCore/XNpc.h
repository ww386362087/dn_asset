#ifndef  __XNpc__
#define  __XNpc__

#include "XEntity.h"


class XNpc :public XEntity
{
public:
	XNpc();
	~XNpc();

	virtual EntityType GetType();

};

#endif