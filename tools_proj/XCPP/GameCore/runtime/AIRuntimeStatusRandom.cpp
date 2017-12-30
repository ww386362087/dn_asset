#include "AIRuntimeStatusRandom.h"


void AIRuntimeStatusRandom::Init(AITaskData* data)
{
	AIBase::Init(data);
	Int32mAIArgProb = data->vars["Int32mAIArgProb"]->val.get<double>(); 
	
}


AIStatus AIRuntimeStatusRandom::OnTick()
{
	
	return Success;
}