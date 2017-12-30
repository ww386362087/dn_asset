#include "AIRuntimeRotateSelf.h"


void AIRuntimeRotateSelf::Init(AITaskData* data)
{
	AIBase::Init(data);
	mAIArgMax = data->vars[0]->val.get<double>(); 
	mAIArgMin = data->vars[1]->val.get<double>(); 
	
}


AIStatus AIRuntimeRotateSelf::OnTick()
{
	
	return Success;
}