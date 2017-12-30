#ifndef  __AIRuntimeDoSelectNearest__
#define  __AIRuntimeDoSelectNearest__

#include "../AIBehaviour.h"
#include "../GameObject.h"
#include "../Vector3.h"


class AIRuntimeDoSelectNearest :public AIBase
{
public:
	~AIRuntimeDoSelectNearest();
	virtual void Init(AITaskData* data);
	virtual AIStatus OnTick();
	

private:
	
};

#endif