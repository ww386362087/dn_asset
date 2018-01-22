#include "AIBehaviour.h"

void AIBase::Init(AITaskData* data)
{
}

AIBase::~AIBase()
{
	delete _tree;
}

void AIBase::SetTree(AITree* tree)
{
	_tree= tree;
}

Vector3 AIBase::Obj2Vector(object obj)
{
	std::string str = obj.get<std::string>();
	return Vector3::Parse(str);
}

AIStatus AIBase::OnTick(XEntity* entity) { return Success; }

AISequence::~AISequence()
{
	for (size_t i = 0; i < list.size(); i++)
	{
		delete list[i];
	}
	list.clear();
}

void AISequence::Init(AITaskData* data)
{
	if (data->children.size()>0)
	{
		list.clear();
		for (size_t i = 0, max = data->children.size(); i < max; i++)
		{
			AIBase* run = AIFactory::MakeRuntime(data->children[i], _tree);
			if(run) list.push_back(run);
		}
	}
}


AIStatus AISequence::OnTick(XEntity* entity)
{
	if (list.size() > 0)
	{
		for (size_t i = 0, max = list.size(); i < max; i++)
		{
			if (list[i]->OnTick(entity) == Failure)
			{
				return Failure;
			}
		}
	}
	return Success;
}


AISelector::~AISelector()
{
	for (size_t i = 0; i < list.size(); i++)
	{
		delete list[i];
	}
	list.clear();
}

void AISelector::Init(AITaskData* data)
{
	if (data->children.size()>0)
	{
		list.clear();
		for (size_t i = 0, max = data->children.size(); i < max; i++)
		{
			AIBase* run = AIFactory::MakeRuntime(data->children[i], _tree);
			if(run) list.push_back(run);
		}
	}
}


AIStatus AISelector::OnTick(XEntity* entity)
{
	if (list.size() > 0)
	{
		for (size_t i = 0, max = list.size(); i < max; i++)
		{
			if (list[i]->OnTick(entity) == Success)
			{
				return Success;
			}
		}
	}
	return Failure;
}


AIInterval::~AIInterval()
{
	delete node;
}

void AIInterval::Init(AITaskData* data)
{
	node = AIFactory::MakeRuntime(data->children[0], _tree);
}


AIStatus AIInterval::OnTick(XEntity* entity)
{
	if (node)
	{
		AIStatus rst = node->OnTick(entity);
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