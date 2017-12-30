#ifndef  __AIRuntimeRotateToTarget__
#define  __AIRuntimeRotateToTarget__

#include "../AIBehaviour.h"
#include "../GameObject.h"
#include "../Vector3.h"


class AIRuntimeRotateToTarget:public AIBase
{
public:
	virtual void Init(AITaskData* data);
	virtual AIStatus OnTick();
	

private:
	GameObject* mAIArgTarget;
	float floatmAIArgAngle;
	
};

#endif