#include "AIRuntimeWait.h"


void AIRuntimeWait::Init(AITaskData* data)
{
	AIBase::Init(data);
}


AIStatus AIRuntimeWait::OnTick()
{
	return Success;
}