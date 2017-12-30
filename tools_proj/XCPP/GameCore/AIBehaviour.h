#ifndef  __AIBehaviour__
#define  __AIBehaviour__


#include "AITree.h"
#include "AIFactory.h"
#include "Vector3.h"
#include <vector>
#include <string>

enum AIStatus
{
	Inactive = 0,
    Failure = 1,
    Success = 2,
    Running = 3
};


class AIBase
{
public:
	virtual void Init(AITaskData* data);
	virtual AIStatus OnTick();
	void SetTree(AITree* tree);
	Vector3 Obj2Vector3(object obj);
protected:
	AITree* _tree;
};


class AISequence:public AIBase
{
public:
	virtual void Init(AITaskData* data);
	virtual AIStatus OnTick();
private:
	std::vector<AIBase*> list;
};


class AISelector:public AIBase
{
public :
	virtual void Init(AITaskData* data);
	virtual AIStatus OnTick();

private:
	std::vector<AIBase*> list;
};


class AIInterval:public AIBase
{
public:
	virtual void Init(AITaskData* data);
	virtual AIStatus OnTick();

private:
	AIBase* node;
};

#endif