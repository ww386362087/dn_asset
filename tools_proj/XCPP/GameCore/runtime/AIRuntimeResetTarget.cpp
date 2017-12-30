#include "AIRuntimeResetTarget.h"


void AIRuntimeResetTarget::Init(AITaskData* data)
{
	AIBase::Init(data);
}


AIStatus AIRuntimeResetTarget::OnTick()
{
	return Success;
}