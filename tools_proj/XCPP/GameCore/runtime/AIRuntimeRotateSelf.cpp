#include "AIRuntimeRotateSelf.h"


void AIRuntimeRotateSelf::Init(AITaskData* data)
{
	AIBase::Init(data);
	mAIArgMax = data->vars["mAIArgMax"]->val.get<double>(); 
	mAIArgMin = data->vars["mAIArgMin"]->val.get<double>(); 
	
}


AIStatus AIRuntimeRotateSelf::OnTick()
{
	
	return Success;
}