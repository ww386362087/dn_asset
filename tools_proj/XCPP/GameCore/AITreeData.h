#ifndef  __AITreeData__
#define  __AITreeData__

#include <vector>
#include "Common.h"
#include <unordered_map>

class  GameObject;

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
	//std::string type; // var's type is not need to use
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
	std::unordered_map<std::string,AIVar*> vars;
	std::vector<AITaskData*> children;
};

class AITreeData
{
public:
	std::vector<AITreeVar*> vars;
	AITaskData* task;

	void SetVariable(std::string name, float value);
	void SetVariable(std::string name, int value);
	void SetVariable(std::string name, uint value);
	void SetVariable(std::string name, bool value);
	void SetVariable(std::string name, GameObject* value);
	std::unordered_map<uint, float> GetFloatCache();
	std::unordered_map<uint, int> GetIntCache();
	std::unordered_map<uint, uint> GetUintCache();
	std::unordered_map<uint, bool> GetBoolCache();
	std::unordered_map<uint, GameObject*> GetGoCache();

private:
	std::unordered_map<uint, float> f_cache;
	std::unordered_map<uint, int> i_cache;
	std::unordered_map<uint, uint> u_cache;
	std::unordered_map<uint, bool> b_cache;
	std::unordered_map<uint, GameObject*> g_cache;
};



#endif