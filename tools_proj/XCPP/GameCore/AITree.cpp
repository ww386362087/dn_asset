#include "AITree.h"
#include "AIUtil.h"
#include "AIFactory.h"

AITree::AITree(void)
{
	composites = new const char*[3]{
		"Sequence",
		"Selector",
		"Inverter" };
}

AITree::~AITree(void)
{
	delete[] composites;
}

void AITree::Initial(XEntity* e)
{
	_entity = e;
}

void AITree::EnableBehaviourTree(bool enable)
{
	_enable = enable;
}

bool AITree::SetBehaviourTree(const char* name)
{
	 AIUtil::Load(name,*_tree_data);
	_tree_behaviour = AIFactory::MakeRuntime(&_tree_data->task, this);
	return true;
}


void AITree::SetVariable(const char* name, float value)
{
	if (_enable && _tree_data)
	{
		_tree_data->SetVariable(name, value);
	}
}

void AITree::SetVariable(const char* name, int value)
{
	if (_enable && _tree_data)
	{
		_tree_data->SetVariable(name, value);
	}
}

void AITree::SetVariable(const char* name, bool value)
{
	if (_enable && _tree_data)
	{
		_tree_data->SetVariable(name, value);
	}
}

void AITree::SetVariable(const char* name, GameObject* value)
{
	if (_enable && _tree_data)
	{
		_tree_data->SetVariable(name, value);
	}
}


float AITree::GetFloatVariable(const char* name)
{
	uint hash = xhash(name);
	std::unordered_map<uint, float> map = _tree_data->GetFloatCache();
	return map[hash];
}

int AITree::GetIntVariable(const char* name)
{
	uint hash = xhash(name);
	std::unordered_map<uint, int> map = _tree_data->GetIntCache();
	return map[hash];
}

bool AITree::GetBoolVariable(const char* name)
{
	uint hash = xhash(name);
	std::unordered_map<uint, bool> map = _tree_data->GetBoolCache();
	return map[hash];
}

GameObject* AITree::GetGoVariable(const char* name)
{
	uint hash = xhash(name);
	std::unordered_map<uint, GameObject*> map = _tree_data->GetGoCache();
	return map[hash];
}


void AITree::SetManual(bool enable)
{
	//do nothing here 
}


void AITree::TickBehaviorTree()
{
	if (_enable && _tree_behaviour)
	{
		_tree_behaviour->OnTick();
	}
}