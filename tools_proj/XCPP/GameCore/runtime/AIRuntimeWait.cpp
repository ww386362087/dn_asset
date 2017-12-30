#include "AIRuntimeWait.h"


void AIRuntimeWait::Init(AITaskData* data)
{
	AIBase::Init(data);
	floatwaitTime = data->vars["floatwaitTime"]->val.get<double>(); 
	boolrandomWait = data->vars["boolrandomWait"]->val.get<bool>(); 
	floatrandomWaitMin = data->vars["floatrandomWaitMin"]->val.get<double>(); 
	floatrandomWaitMax = data->vars["floatrandomWaitMax"]->val.get<double>(); 
	
}


AIStatus AIRuntimeWait::OnTick()
{
	
	return Success;
}