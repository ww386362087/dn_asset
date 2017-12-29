#ifndef  __AIRuntimeRotateSelf__
#define  __AIRuntimeRotateSelf__

#include "../AIBehaviour.h"
#include "../GameObject.h"
#include "../Vector3.h"


class AIRuntimeRotateSelf:public AIBase
{
public:
	virtual void Init(AITaskData* data);
	virtual AIStatus OnTick();
	

private:
	float mAIArgMax;
	float mAIArgMin;
	
};

#endif