#ifndef  __AIRuntimeLog__
#define  __AIRuntimeLog__

#include "../AIBehaviour.h"
#include "../GameObject.h"
#include "../Vector3.h"


class AIRuntimeLog :public AIBase
{
public:
	~AIRuntimeLog();
	virtual void Init(AITaskData* data);
	virtual AIStatus OnTick();
	

private:
	std::string stringtext;
	bool boollogError;
	
};

#endif