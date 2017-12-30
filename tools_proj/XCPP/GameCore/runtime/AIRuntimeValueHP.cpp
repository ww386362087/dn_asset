#include "AIRuntimeValueHP.h"


void AIRuntimeValueHP::Init(AITaskData* data)
{
	AIBase::Init(data);
	mAIArgMaxHP = data->vars[0]->val.get<double>(); 
	mAIArgMinHP = data->vars[1]->val.get<double>(); 
	
}


AIStatus AIRuntimeValueHP::OnTick()
{
	
	return Success;
}