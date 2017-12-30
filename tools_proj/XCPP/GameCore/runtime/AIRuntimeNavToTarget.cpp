#include "AIRuntimeNavToTarget.h"


AIRuntimeNavToTarget::~AIRuntimeNavToTarget()
{
	delete GameObjectmAIArgTarget;delete GameObjectmAIArgNavTarget;
}

void AIRuntimeNavToTarget::Init(AITaskData* data)
{
	AIBase::Init(data);
	Vector3mAIArgNavPos = Obj2Vector(data->vars["Vector3mAIArgNavPos"]->val); 
	
}


AIStatus AIRuntimeNavToTarget::OnTick()
{
	GameObjectmAIArgTarget = _tree->GetGoVariable("target");
	
	return Success;
}