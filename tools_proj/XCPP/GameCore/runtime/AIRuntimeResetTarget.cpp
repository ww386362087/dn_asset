#include "AIRuntimeResetTarget.h"


AIRuntimeResetTarget::~AIRuntimeResetTarget()
{
	
}

void AIRuntimeResetTarget::Init(AITaskData* data)
{
	AIBase::Init(data);
	
}


AIStatus AIRuntimeResetTarget::OnTick()
{
	
	return Success;
}