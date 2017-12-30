#include "AIRuntimeFindTargetByDistance.h"


void AIRuntimeFindTargetByDistance::Init(AITaskData* data)
{
	AIBase::Init(data);
	floatmAIArgDistance = data->vars[0]->val.get<double>(); 
	mAIArgAngle = data->vars[1]->val.get<double>(); 
	
}


AIStatus AIRuntimeFindTargetByDistance::OnTick()
{
	
	return Success;
}