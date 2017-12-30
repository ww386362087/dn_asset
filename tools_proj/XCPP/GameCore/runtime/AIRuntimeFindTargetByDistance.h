#ifndef  __AIRuntimeFindTargetByDistance__
#define  __AIRuntimeFindTargetByDistance__

#include "../AIBehaviour.h"
#include "../GameObject.h"
#include "../Vector3.h"


class AIRuntimeFindTargetByDistance :public AIBase
{
public:
	~AIRuntimeFindTargetByDistance();
	virtual void Init(AITaskData* data);
	virtual AIStatus OnTick();
	

private:
	float floatmAIArgDistance;
	float SinglemAIArgAngle;
	
};

#endif