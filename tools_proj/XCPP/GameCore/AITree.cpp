#include "AITree.h"


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

	return true;
}


void AITree::SetVariable(std::string name, object value)
{

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

}


void AITree::TickBehaviorTree()
{

}