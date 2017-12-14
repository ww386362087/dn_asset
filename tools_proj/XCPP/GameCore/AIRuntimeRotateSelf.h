#ifndef  __AIRuntimeRotateSelf__
#define  __AIRuntimeRotateSelf__

#include "AIBehaviour.h"

class AIRuntimeRotateSelf:public AIBase
{
public:
	AIRuntimeRotateSelf();
	~AIRuntimeRotateSelf();
	virtual void Init(AITaskData* data);
	virtual AIStatus OnTick();
	

private:
	float mAIArgMax;
	float mAIArgMin;
};

#endif