#include "AIRuntimeRotateToTarget.h"


void AIRuntimeRotateToTarget::Init(AITaskData* data)
{
	AIBase::Init(data);
	floatmAIArgAngle = data->vars["floatmAIArgAngle"]->val.get<double>(); 
	
}


AIStatus AIRuntimeRotateToTarget::OnTick()
{
	mAIArgTarget = _tree->GetGoVariable("target");
	
	return Success;
}