#ifndef  __AIRuntimeValueDistance__
#define  __AIRuntimeValueDistance__

#include "../AIBehaviour.h"
#include "../GameObject.h"
#include "../Vector3.h"


class AIRuntimeValueDistance:public AIBase
{
public:
	virtual void Init(AITaskData* data);
	virtual AIStatus OnTick();
	

private:
	GameObject mAIArgTarget;
	float floatmAIArgMaxDistance;
	
};

#endif