#ifndef  __AIRuntimeWait__
#define  __AIRuntimeWait__

#include "../AIBehaviour.h"
#include "../GameObject.h"
#include "../Vector3.h"


class AIRuntimeWait :public AIBase
{
public:
	~AIRuntimeWait();
	virtual void Init(AITaskData* data);
	virtual AIStatus OnTick();
	

private:
	float floatwaitTime;
	bool boolrandomWait;
	float floatrandomWaitMin;
	float floatrandomWaitMax;
	
};

#endif