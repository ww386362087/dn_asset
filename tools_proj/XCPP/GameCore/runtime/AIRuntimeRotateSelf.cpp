#include "AIRuntimeRotateSelf.h"


AIRuntimeRotateSelf::~AIRuntimeRotateSelf()
{
	
}

void AIRuntimeRotateSelf::Init(AITaskData* data)
{
	AIBase::Init(data);
	SinglemAIArgMax = data->vars["SinglemAIArgMax"]->val.get<double>(); 
	SinglemAIArgMin = data->vars["SinglemAIArgMin"]->val.get<double>(); 
	
}


AIStatus AIRuntimeRotateSelf::OnTick()
{
	
	return Success;
}