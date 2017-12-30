#include "AIRuntimeNavToTarget.h"


void AIRuntimeNavToTarget::Init(AITaskData* data)
{
	AIBase::Init(data);
	mAIArgNavPos = Obj2Vector3(data->vars[2]->val); 
	
}


AIStatus AIRuntimeNavToTarget::OnTick()
{
	mAIArgTarget = _tree->GetGoVariable("target");
	
	return Success;
}