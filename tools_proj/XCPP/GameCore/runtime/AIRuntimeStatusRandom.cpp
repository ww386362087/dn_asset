#include "AIRuntimeStatusRandom.h"


void AIRuntimeStatusRandom::Init(AITaskData* data)
{
	AIBase::Init(data);
	mAIArgProb = data->vars["mAIArgProb"]->val.get<double>(); 
	
}


AIStatus AIRuntimeStatusRandom::OnTick()
{
	
	return Success;
}