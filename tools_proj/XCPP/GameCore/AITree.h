#ifndef  __AITree__
#define  __AITree__

#include "AITreeData.h"
#include <unordered_map>


class AIBase;

class AITree
{
public:
	AITree(void);
	~AITree(void);
	void EnableBehaviourTree(bool enable);
	bool SetBehaviourTree(std::string name);
	void SetVariable(std::string name, object value);
	void GetVariable(std::string name,object& obj);
	void SetManual(bool enable);
	void TickBehaviorTree();


private:
	std::string *composites;
	AITreeData* _tree_data;
	AIBase* _tree_behaviour;
	bool _enable;
};


#endif