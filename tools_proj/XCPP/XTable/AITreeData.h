#pragma once
#include <vector>
#include "Common.h"

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
};

class AISharedVar
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
	template<typename T> void SetVariable(std::string name,T* value);
	
private:
	std::vector<AITreeVar> vars;
	//std::map<uint,int> cache;
	AITaskData task;
};

template<typename T> 
void SetVariable(std::string name,T* value)
{
	uint hash = xhash(name.c_str());
	//cache[hash] = T;
}