#ifndef  __AIRuntime[*Name*]__
#define  __AIRuntime[*Name*]__

#include "../AIBehaviour.h"
#include "../GameObject.h"
#include "../Vector3.h"


class AIRuntime[*Name*] :public AIBase
{
public:
	~AIRuntime[*Name*]();
	virtual void Init(AITaskData* data);
	virtual AIStatus OnTick();
	

private:
	[*Arg*]
};

#endif