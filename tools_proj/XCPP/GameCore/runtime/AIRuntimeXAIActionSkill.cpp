#include "AIRuntimeXAIActionSkill.h"


void AIRuntimeXAIActionSkill::Init(AITaskData* data)
{
	AIBase::Init(data);
	mAIArgSkillScript = data->vars["mAIArgSkillScript"]->val.get<std::string>(); 
	
}


AIStatus AIRuntimeXAIActionSkill::OnTick()
{
	mAIArgTarget = _tree->GetGoVariable("target");
	
	return Success;
}