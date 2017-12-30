#ifndef  __AIRuntimeStatusRandom__
#define  __AIRuntimeStatusRandom__

#include "../AIBehaviour.h"
#include "../GameObject.h"
#include "../Vector3.h"


class AIRuntimeStatusRandom :public AIBase
{
public:
	~AIRuntimeStatusRandom();
	virtual void Init(AITaskData* data);
	virtual AIStatus OnTick();
	

private:
	int Int32mAIArgProb;
	
};

#endif