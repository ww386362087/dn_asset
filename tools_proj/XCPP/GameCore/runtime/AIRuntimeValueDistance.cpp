#include "AIRuntimeValueDistance.h"


void AIRuntimeValueDistance::Init(AITaskData* data)
{
	AIBase::Init(data);
	floatmAIArgMaxDistance = data->vars[1]->val.get<double>(); 
	
}


AIStatus AIRuntimeValueDistance::OnTick()
{
	mAIArgTarget = _tree->GetGoVariable("target");
	
	return Success;
}