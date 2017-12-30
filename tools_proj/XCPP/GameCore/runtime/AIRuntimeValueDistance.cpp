#include "AIRuntimeValueDistance.h"


AIRuntimeValueDistance::~AIRuntimeValueDistance()
{
	delete GameObjectmAIArgTarget;
}

void AIRuntimeValueDistance::Init(AITaskData* data)
{
	AIBase::Init(data);
	floatmAIArgMaxDistance = data->vars["floatmAIArgMaxDistance"]->val.get<double>(); 
	
}


AIStatus AIRuntimeValueDistance::OnTick()
{
	GameObjectmAIArgTarget = _tree->GetGoVariable("target");
	
	return Success;
}