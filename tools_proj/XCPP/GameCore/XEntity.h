#ifndef  __XEntity__
#define  __XEntity__

#include "XObject.h"

class XEntity:public XObject
{
public:

	virtual void Update(float delta);
	virtual void LateUpdate();
	virtual void AttachToHost();
	virtual void DetachFromHost();
};

#endif