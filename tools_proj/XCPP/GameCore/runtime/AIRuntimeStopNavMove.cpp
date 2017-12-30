#include "AIRuntimeStopNavMove.h"


void AIRuntimeStopNavMove::Init(AITaskData* data)
{
	AIBase::Init(data);
}


AIStatus AIRuntimeStopNavMove::OnTick()
{
	return Success;
}