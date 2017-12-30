#include "AIRuntimeValueHP.h"


void AIRuntimeValueHP::Init(AITaskData* data)
{
	AIBase::Init(data);
	Int32mAIArgMaxHP = data->vars["Int32mAIArgMaxHP"]->val.get<double>(); 
	Int32mAIArgMinHP = data->vars["Int32mAIArgMinHP"]->val.get<double>(); 
	
}


AIStatus AIRuntimeValueHP::OnTick()
{
	
	return Success;
}