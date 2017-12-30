#include "AIRuntimeFindTargetByDistance.h"


void AIRuntimeFindTargetByDistance::Init(AITaskData* data)
{
	AIBase::Init(data);
	floatmAIArgDistance = data->vars["floatmAIArgDistance"]->val.get<double>(); 
	SinglemAIArgAngle = data->vars["SinglemAIArgAngle"]->val.get<double>(); 
	
}


AIStatus AIRuntimeFindTargetByDistance::OnTick()
{
	
	return Success;
}