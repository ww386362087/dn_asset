#include "AIRuntimeValueHP.h"


void AIRuntimeValueHP::Init(AITaskData* data)
{
	AIBase::Init(data);
	mAIArgMaxHP = data->vars["mAIArgMaxHP"]->val.get<double>(); 
	mAIArgMinHP = data->vars["mAIArgMinHP"]->val.get<double>(); 
	
}


AIStatus AIRuntimeValueHP::OnTick()
{
	
	return Success;
}