#ifndef  __AIRuntimeNavToTarget__
#define  __AIRuntimeNavToTarget__

#include "../AIBehaviour.h"
#include "../GameObject.h"
#include "../Vector3.h"


class AIRuntimeNavToTarget:public AIBase
{
public:
	virtual void Init(AITaskData* data);
	virtual AIStatus OnTick();
	

private:
	GameObject mAIArgTarget;
	GameObject mAIArgNavTarget;
	Vector3 mAIArgNavPos;
	
};

#endif