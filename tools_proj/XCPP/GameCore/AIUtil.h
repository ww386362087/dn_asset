#ifndef  __AIUtil__
#define  __AIUtil__

#include <string>
#include "Common.h"
#include "AITreeData.h"
#include "Log.h"

class AITreeVar;


class AIUtil
{
public:
	static void Load(std::string name,AITreeData* data);
	static void Parse(std::string json, std::string name, AITreeData* data);
	static void ParseTreeVar(picojson::object& arg,AITreeVar* var);
	static Mode Type2Mode(std::string type);
	static void ParseTask(picojson::object& root, AITaskData* data);
	static void ParseSharedVar(std::string key, picojson::object& obj, AISharedVar* var,bool shared);
	static void ParseCustomVar(std::string key, object val, AIVar* var);

private:
	static std::string Children; 
	static std::string Type;
};


#endif
