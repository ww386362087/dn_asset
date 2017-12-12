#include "AIUtil.h"

std::string  AIUtil::Children = "Children";
std::string AIUtil::Type = "Type";


void AIUtil::Load(std::string name, AITreeData& data)
{

}


void AIUtil::Parse(std::string json, std::string name, AITreeData& data)
{

}

void AIUtil::ParseTreeVar(std::unordered_map<std::string, object> arg, AITreeVar& var)
{

}

Mode AIUtil::Type2Mode(std::string type)
{
	if (type == "Sequence") return Sequence;
	if (type == "Selector") return Selector;
	if (type == "Inverter") return Inverter;
	return Custom;
}

void AIUtil::ParseTask(std::unordered_map<std::string, object> task, AITaskData& data)
{

}

void AIUtil::ParseSharedVar(std::string key, std::unordered_map<std::string, object> dic, AISharedVar& var)
{

}

void AIUtil::ParseCustomVar(std::string key, object val, AIVar& var)
{

}

void AIUtil::ParseVarValue(AIVar* var, object val)
{
	
	/*if(var->type == "System.Boolean")
		var->val = val;
	else if(var->type== "System.String")
		var->val = val.get<std::string>().Replace("\n", "").Replace("\t", "").Replace("\r", "");
	else if (var->type == "System.Single")
		var->val = float.Parse(val.ToString());
	else if (var->type == "System.Int32":
		var->val = int.Parse(val.ToString());
	else if (var->type == "Vector3"||var->type == "Vector2"||var->type == "Vector4")
		var->val = ParseVector(val.ToString());
	else*/
		var->val = val;
}

std::string AIUtil::TransfType(object type)
{
	std::string* arr = new std::string[4]{ 
		"float", 
		"int", 
		"bool", 
		"string" };

	std::string* arr2 = new std::string[4]{ 
		"System.Single", 
		"System.Int32", 
		"System.Boolean", 
		"System.String" };

	for (int i = 0, max = 4; i < max; i++)
	{
	/*	if (tostring(type) == arr[i])
			return arr2[i];*/
	}
	//return tostring(type);
	return "";
}

void AIUtil::ParseVector(std::string str,object& val)
{

}