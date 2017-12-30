#include "AIRuntimeRotateToTarget.h"


AIRuntimeRotateToTarget::~AIRuntimeRotateToTarget()
{
	delete GameObjectmAIArgTarget;
}

void AIRuntimeRotateToTarget::Init(AITaskData* data)
{
	AIBase::Init(data);
	floatmAIArgAngle = data->vars["floatmAIArgAngle"]->val.get<double>(); 
	
}


AIStatus AIRuntimeRotateToTarget::OnTick()
{
	GameObjectmAIArgTarget = _tree->GetGoVariable("target");
	
	return Success;
}