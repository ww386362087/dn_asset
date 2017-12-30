#include "AIRuntimeFindTargetByDistance.h"


void AIRuntimeFindTargetByDistance::Init(AITaskData* data)
{
	AIBase::Init(data);
	floatmAIArgDistance = data->vars["floatmAIArgDistance"]->val.get<double>(); 
	mAIArgAngle = data->vars["mAIArgAngle"]->val.get<double>(); 
	
}


AIStatus AIRuntimeFindTargetByDistance::OnTick()
{
	
	return Success;
}