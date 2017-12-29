#ifndef  __AIRuntimeRotateToTarget__
#define  __AIRuntimeRotateToTarget__

#include "GameObject.h"
#include "AIBehaviour.h"

class AIRuntimeRotateToTarget:public AIBase
{
public:
	virtual void Init(AITaskData* data);
	virtual AIStatus OnTick();

private:
	GameObject mAIArgTarget;
	float floatmAIArgAngle;
};


#endif