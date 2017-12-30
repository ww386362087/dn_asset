#ifndef  __AIRuntimeXAIActionSkill__
#define  __AIRuntimeXAIActionSkill__

#include "../AIBehaviour.h"
#include "../GameObject.h"
#include "../Vector3.h"


class AIRuntimeXAIActionSkill :public AIBase
{
public:
	~AIRuntimeXAIActionSkill();
	virtual void Init(AITaskData* data);
	virtual AIStatus OnTick();
	

private:
	std::string StringmAIArgSkillScript;
	GameObject* GameObjectmAIArgTarget;
	
};

#endif