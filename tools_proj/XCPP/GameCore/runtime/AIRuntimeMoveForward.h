#ifndef  __AIRuntimeMoveForward__
#define  __AIRuntimeMoveForward__

#include "../AIBehaviour.h"
#include "../GameObject.h"
#include "../Vector3.h"


class AIRuntimeMoveForward:public AIBase
{
public:
	virtual void Init(AITaskData* data);
	virtual AIStatus OnTick();
	

private:
	
};

#endif