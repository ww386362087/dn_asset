#include "AITree.h"
#include "AIUtil.h"
#include "AIFactory.h"

AITree::AITree(void)
{
	composites = new std::string[3]{
		"Sequence",
		"Selector",
		"Inverter" };

}

AITree::~AITree(void)
{
	delete[] composites;
}

void AITree::EnableBehaviourTree(bool enable)
{
	_enable = enable;
}

bool AITree::SetBehaviourTree(std::string name)
{
	 AIUtil::Load(name,*_tree_data);
	_tree_behaviour = AIFactory::MakeRuntime(&_tree_data->task, this);
	return true;
}


void AITree::SetVariable(std::string name, object value)
{
	if (_enable && _tree_data)
	{
		_tree_data->SetVariable(name, value);
	}
}


void AITree::GetVariable(std::string name,object& obj)
{
	if (_enable)
	{
		uint hash = xhash(name.c_str());
		_tree_data->GetCache();

	
	}
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