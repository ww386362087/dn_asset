#include "AIRuntimeWait.h"


void AIRuntimeWait::Init(AITaskData* data)
{
	AIBase::Init(data);
	floatwaitTime = data->vars[0]->val.get<double>(); 
	boolrandomWait = data->vars[1]->val.get<bool>(); 
	floatrandomWaitMin = data->vars[2]->val.get<double>(); 
	floatrandomWaitMax = data->vars[3]->val.get<double>(); 
	
}


AIStatus AIRuntimeWait::OnTick()
{
	
	return Success;
}