#include "AIRuntimeNavToTarget.h"


void AIRuntimeNavToTarget::Init(AITaskData* data)
{
	AIBase::Init(data);
	mAIArgNavPos = Obj2Vector(data->vars["mAIArgNavPos"]->val); 
	
}


AIStatus AIRuntimeNavToTarget::OnTick()
{
	mAIArgTarget = _tree->GetGoVariable("target");
	
	return Success;
}