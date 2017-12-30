#ifndef  __AIRuntimeRotateToTarget__
#define  __AIRuntimeRotateToTarget__

#include "../AIBehaviour.h"
#include "../GameObject.h"
#include "../Vector3.h"


class AIRuntimeRotateToTarget :public AIBase
{
public:
	~AIRuntimeRotateToTarget();
	virtual void Init(AITaskData* data);
	virtual AIStatus OnTick();
	

private:
	GameObject* GameObjectmAIArgTarget;
	float floatmAIArgAngle;
	
};

#endif