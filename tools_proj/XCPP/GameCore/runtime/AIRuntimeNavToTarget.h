#ifndef  __AIRuntimeNavToTarget__
#define  __AIRuntimeNavToTarget__

#include "../AIBehaviour.h"
#include "../GameObject.h"
#include "../Vector3.h"


class AIRuntimeNavToTarget :public AIBase
{
public:
	~AIRuntimeNavToTarget();
	virtual void Init(AITaskData* data);
	virtual AIStatus OnTick();
	

private:
	GameObject* GameObjectmAIArgTarget;
	GameObject* GameObjectmAIArgNavTarget;
	Vector3 Vector3mAIArgNavPos;
	
};

#endif