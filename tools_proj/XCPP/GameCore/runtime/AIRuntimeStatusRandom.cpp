#include "AIRuntimeStatusRandom.h"


void AIRuntimeStatusRandom::Init(AITaskData* data)
{
	AIBase::Init(data);
}


AIStatus AIRuntimeStatusRandom::OnTick()
{
	return Success;
}