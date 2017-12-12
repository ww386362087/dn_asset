#ifndef  __AIFactory__
#define  __AIFactory__


#include "AIBehaviour.h"
#include "AITreeData.h"

class AIFactory
{
public:
	AIFactory(void);
	~AIFactory(void);

	static AIBase* MakeRuntime(AITaskData* data, AITree* tree);
};

#endif