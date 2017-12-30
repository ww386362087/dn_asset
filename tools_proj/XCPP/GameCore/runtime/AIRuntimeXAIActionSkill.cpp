#include "AIRuntimeXAIActionSkill.h"


AIRuntimeXAIActionSkill::~AIRuntimeXAIActionSkill()
{
	delete GameObjectmAIArgTarget;
}

void AIRuntimeXAIActionSkill::Init(AITaskData* data)
{
	AIBase::Init(data);
	StringmAIArgSkillScript = data->vars["StringmAIArgSkillScript"]->val.get<std::string>(); 
	
}


AIStatus AIRuntimeXAIActionSkill::OnTick()
{
	GameObjectmAIArgTarget = _tree->GetGoVariable("target");
	
	return Success;
}