#include "AIFactory.h"


AIFactory::AIFactory(void)
{
}


AIFactory::~AIFactory(void)
{
}


AIBase* AIFactory::MakeRuntime(AITaskData* data, AITree* tree)
{
	AIBase* rst = NULL;
	if (data->type == "Sequence")
	{
		rst = new AISequence();
	}
	else if (data->type == "Selector")
	{
		rst = new AISelector();
	}
	else if (data->type == "Inverter")
	{
		rst = new AIInterval();
	}
	if (rst != NULL) 
	{
		rst->SetTree(tree);
		rst->Init(data);
	}
	return rst;
}