#include "AIBehaviour.h"

void AIBase::Init(AITaskData* data)
{
}

void AIBase::SetTree(AITree* tree)
{
	_tree= tree;
}

Vector3 AIBase::Obj2Vector3(object obj)
{
	std::string str = obj.get<std::string>();
	return Vector3::Parse(str);
}

AIStatus AIBase::OnTick() { return Success; }


void AISequence::Init(AITaskData* data)
{
	if (data->children.size()>0)
	{
		list.clear();
		for (size_t i = 0, max = data->children.size(); i < max; i++)
		{
			AIBase* run = AIFactory::MakeRuntime(data->children[i], _tree);
			list.push_back(run);
		}
	}
}


AIStatus AISequence::OnTick()
{
	if (list.size() > 0)
	{
		for (size_t i = 0, max = list.size(); i < max; i++)
		{
			if (list[i]->OnTick() == Failure)
			{
				return Failure;
			}
		}
	}
	return Success;
}


void AISelector::Init(AITaskData* data)
{
	if (data->children.size()>0)
	{
		list.clear();
		for (size_t i = 0, max = data->children.size(); i < max; i++)
		{
			AIBase* run = AIFactory::MakeRuntime(data->children[i], _tree);
			list.push_back(run);
		}
	}
}


AIStatus AISelector::OnTick()
{
	if (list.size() > 0)
	{
		for (size_t i = 0, max = list.size(); i < max; i++)
		{
			if (list[i]->OnTick() == Success)
			{
				return Success;
			}
		}
	}
	return Failure;
}



void AIInterval::Init(AITaskData* data)
{
	node = AIFactory::MakeRuntime(data->children[0], _tree);
}


AIStatus AIInterval::OnTick()
{
	if (node)
	{
		AIStatus rst = node->OnTick();
		if (rst == Failure)
		{
			return Success;
		}
		else if (rst == Success)
		{
			return Failure;
		}
		return rst;
	}
	return Success;
}