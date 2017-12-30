#include "AIRuntimeLog.h"


void AIRuntimeLog::Init(AITaskData* data)
{
	AIBase::Init(data);
}


AIStatus AIRuntimeLog::OnTick()
{
	return Success;
}