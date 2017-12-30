#include "AIRuntimeLog.h"


void AIRuntimeLog::Init(AITaskData* data)
{
	AIBase::Init(data);
	stringtext = data->vars["stringtext"]->val.get<std::string>(); 
	boollogError = data->vars["boollogError"]->val.get<bool>(); 
	
}


AIStatus AIRuntimeLog::OnTick()
{
	
	return Success;
}