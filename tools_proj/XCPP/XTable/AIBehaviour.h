#pragma once
#include "AITree.h"
#include "AITreeData.h"
#include <vector>

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
	virtual void Init(AITaskData data);
	virtual void OnTick();
	void SetTree(AITree tree);

protected:
	AITree _tree;
};


class AISequence:public AIBase
{
public:
	virtual void Init(AITaskData data);
	virtual void OnTick();
private:
	std::vector<AIBase> list;
};


class AISelector:public AIBase
{
public :
	virtual void Init(AITaskData data);
	virtual void OnTick();

private:
	std::vector<AIBase> list;
};


class AIInterval:public AIBase
{
public:
	virtual void Init(AITaskData data);
	virtual void OnTick();

private:
	AIBase node;
};