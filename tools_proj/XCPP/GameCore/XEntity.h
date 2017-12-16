#ifndef  __XEntity__
#define  __XEntity__

#include "XObject.h"
#include "XEntityMgr.h"


class XEntity:public XObject
{
public:
	XEntity();
	~XEntity();

	virtual void Update(float delta);
	virtual void LateUpdate();
	virtual void AttachToHost();
	virtual void DetachFromHost();
};

#endif