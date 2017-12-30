#include "AIRuntime[*Name*].h"


AIRuntime[*Name*]::~AIRuntime[*Name*]()
{
	[*arg-0*]
}

void AIRuntime[*Name*]::Init(AITaskData* data)
{
	AIBase::Init(data);
	[*arg-1*]
}


AIStatus AIRuntime[*Name*]::OnTick()
{
	[*arg-2*]
	return Success;
}