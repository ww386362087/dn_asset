/*
* <auto-generated>
*	 The code is generated by tools
*	 Edit manually this code will lead to incorrect behavior
* </auto-generated>
*/

#include "AIRuntimeWait.h"


AIRuntimeWait::~AIRuntimeWait()
{
	
}

void AIRuntimeWait::Init(AITaskData* data)
{
	AIBase::Init(data);
	floatwaitTime = (float)data->vars["floatwaitTime"]->val.get<double>(); 
	boolrandomWait = data->vars["boolrandomWait"]->val.get<bool>(); 
	floatrandomWaitMin = (float)data->vars["floatrandomWaitMin"]->val.get<double>(); 
	floatrandomWaitMax = (float)data->vars["floatrandomWaitMax"]->val.get<double>(); 
	
}


AIStatus AIRuntimeWait::OnTick(XEntity* entity)
{
	
	return AITreeImpleted::WaitUpdate(entity,floatwaitTime,boolrandomWait,floatrandomWaitMin,floatrandomWaitMax);
}