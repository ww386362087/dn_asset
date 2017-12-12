#ifndef  __AITreeData__
#define  __AITreeData__

#include <vector>
#include "Common.h"
#include <unordered_map>


enum Mode
{
    Selector,
    Sequence,
    Inverter,
    Custom, //自定义的Action | Conditional
};

class AITreeVar
{
public:
	bool isShared;
	std::string type;
	std::string name;
	std::string val;
};


class AIVar
{
public:
	std::string type;
	std::string name;
	object val;
};


class AISharedVar:public AIVar
{
public:
	std::string bindName;
	bool isShared;
};


class AITaskData
{
public:
	Mode mode;
	std::string type;
	std::vector<AIVar> vars;
	std::vector<AITaskData> children;
};

class AITreeData 
{
public :
	std::vector<AITreeVar> vars;
	AITaskData task;

	void SetVariable(std::string name,object value);
	std::unordered_map<uint,object> GetCache();
	
private:
	std::unordered_map<uint,object> cache;
	
};



#endif