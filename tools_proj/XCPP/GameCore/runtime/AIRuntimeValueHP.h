#ifndef  __AIRuntimeValueHP__
#define  __AIRuntimeValueHP__

#include "../AIBehaviour.h"
#include "../GameObject.h"
#include "../Vector3.h"


class AIRuntimeValueHP:public AIBase
{
public:
	virtual void Init(AITaskData* data);
	virtual AIStatus OnTick();
	

private:
	int mAIArgMaxHP;
	int mAIArgMinHP;
	
};

#endif