#ifndef  __AIRuntimeResetTarget__
#define  __AIRuntimeResetTarget__

#include "../AIBehaviour.h"
#include "../GameObject.h"
#include "../Vector3.h"


class AIRuntimeResetTarget :public AIBase
{
public:
	~AIRuntimeResetTarget();
	virtual void Init(AITaskData* data);
	virtual AIStatus OnTick();
	

private:
	
};

#endif