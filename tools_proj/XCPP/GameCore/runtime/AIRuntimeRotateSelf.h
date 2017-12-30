#ifndef  __AIRuntimeRotateSelf__
#define  __AIRuntimeRotateSelf__

#include "../AIBehaviour.h"
#include "../GameObject.h"
#include "../Vector3.h"


class AIRuntimeRotateSelf :public AIBase
{
public:
	~AIRuntimeRotateSelf();
	virtual void Init(AITaskData* data);
	virtual AIStatus OnTick();
	

private:
	float SinglemAIArgMax;
	float SinglemAIArgMin;
	
};

#endif