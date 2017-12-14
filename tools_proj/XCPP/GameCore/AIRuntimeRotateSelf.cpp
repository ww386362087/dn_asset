#include "AIRuntimeRotateSelf.h"


AIRuntimeRotateSelf::AIRuntimeRotateSelf()
{
}


AIRuntimeRotateSelf::~AIRuntimeRotateSelf()
{
}


void AIRuntimeRotateSelf::Init(AITaskData* data)
{
	//mAIArgMax = data->vars[0].val.get<float>();
	//mAIArgMin = data->vars[1].val.get<float>();
}


AIStatus AIRuntimeRotateSelf::OnTick()
{
	return Success;
}