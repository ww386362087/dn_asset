#include "AIRuntimeLog.h"


void AIRuntimeLog::Init(AITaskData* data)
{
	AIBase::Init(data);
	stringtext = data->vars[0]->val.get<std::string>(); 
	boollogError = data->vars[1]->val.get<bool>(); 
	
}


AIStatus AIRuntimeLog::OnTick()
{
	
	return Success;
}