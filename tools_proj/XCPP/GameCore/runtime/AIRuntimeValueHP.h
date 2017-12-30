#ifndef  __AIRuntimeValueHP__
#define  __AIRuntimeValueHP__

#include "../AIBehaviour.h"
#include "../GameObject.h"
#include "../Vector3.h"


class AIRuntimeValueHP :public AIBase
{
public:
	~AIRuntimeValueHP();
	virtual void Init(AITaskData* data);
	virtual AIStatus OnTick();
	

private:
	int Int32mAIArgMaxHP;
	int Int32mAIArgMinHP;
	
};

#endif