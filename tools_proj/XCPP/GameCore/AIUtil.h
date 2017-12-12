#ifndef  __AIUtil__
#define  __AIUtil__

#include <string>
#include <unordered_map>
#include "Common.h"
#include "AITreeData.h"

class AITreeVar;


class AIUtil
{
public:
	static void Load(std::string name,AITreeData& data);
	static void Parse(std::string json, std::string name, AITreeData& data);
	static void ParseTreeVar(std::unordered_map<std::string,object> arg,AITreeVar& var);
	static Mode Type2Mode(std::string type);
	static void ParseTask(std::unordered_map<std::string, object> task, AITaskData& data);
	static void ParseSharedVar(std::string key, std::unordered_map<std::string, object> dic, AISharedVar& var);
	static void ParseCustomVar(std::string key, object val, AIVar& var);
	static void ParseVarValue(AIVar* var, object val);
	static std::string TransfType(object type);
	static void ParseVector(std::string str,object& val);

private:
	static std::string Children; 
	static std::string Type;
};


#endif
