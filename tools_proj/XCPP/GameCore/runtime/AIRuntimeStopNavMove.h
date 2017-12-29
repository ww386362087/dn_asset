#ifndef  __AIRuntimeStopNavMove__
#define  __AIRuntimeStopNavMove__

#include "../AIBehaviour.h"
#include "../GameObject.h"
#include "../Vector3.h"


class AIRuntimeStopNavMove:public AIBase
{
public:
	virtual void Init(AITaskData* data);
	virtual AIStatus OnTick();
	

private:
	
};

#endif