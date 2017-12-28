#include "AIUtil.h"

std::string  AIUtil::Children = "Children";
std::string AIUtil::Type = "Type";


void AIUtil::Load(std::string name, AITreeData& data)
{
	std::string str = readFile(name.c_str());
	Parse(str, name, data);
}

void AIUtil::Parse(std::string json, std::string name, AITreeData& tree)
{
	picojson::value v;
	std::string err = picojson::parse(v, json);
	if (err.size())
	{
		ERR(err);
		return;
	}
	picojson::object& o = v.get<picojson::object>();
	tree.vars.clear();
	picojson::array ar = o["Variables"].get<picojson::array>();
	for (picojson::array::iterator it = ar.begin(); it != ar.end(); it++)
	{
		AITreeVar* var = new AITreeVar();
		ParseTreeVar(it->get<picojson::object>(), *var);
		tree.vars.push_back(*var);
	}
	picojson::object root = o["RootTask"].get<picojson::object>();
	ParseTask(root, tree.task);
}

void AIUtil::ParseTreeVar(picojson::object& arg, AITreeVar& var)
{
	var.name = arg["Name"].get<std::string>();
	var.type = arg["Type"].get<std::string>();
	var.isShared = arg["IsShared"].get<bool>();
}

Mode AIUtil::Type2Mode(std::string type)
{
	if (type == "Sequence") return Sequence;
	if (type == "Selector") return Selector;
	if (type == "Inverter") return Inverter;
	return Custom;
}

void AIUtil::ParseTask(picojson::object& root, AITaskData& data)
{
	data.children.clear();
	data.vars.clear();
	for (picojson::object::iterator it = root.begin(); it != root.end(); it++)
	{
		if (it->first == Type)
		{
			data.type = it->second.get<std::string>();
			data.mode = Type2Mode(data.type);
		}
		else if(it->first == Children)
		{
			picojson::array ar = it->second.get<picojson::array>();
			for (picojson::array::iterator iat = ar.begin(); iat != ar.end(); iat++)
			{
				picojson::object obj = iat->get<picojson::object>();
				AITaskData* td = new AITaskData();
				ParseTask(obj, *td);
				data.children.push_back(*td);
			}
		}
		else if (it->second.contains("IsShared"))
		{
			AISharedVar* var = new AISharedVar();
			picojson::object ox = it->second.get<picojson::object>();
			ParseSharedVar(it->first, ox, *var);
			data.vars.push_back(*var);
		}
		else
		{
			AIVar* var = new AIVar();
			ParseCustomVar(it->first, it->second, *var);
			data.vars.push_back(*var);
		}
	}
}

void AIUtil::ParseSharedVar(std::string key, picojson::object& obj, AISharedVar& var)
{
	var.name = key;
	var.bindName = obj["Name"].get<std::string>();
	var.isShared = obj["IsShared"].get<bool>();
	var.type = TransfType(obj["Type"]);
	for (picojson::object::iterator it = obj.begin(); it != obj.end(); it++)
	{
		if (it->first.find("Value"))
		{
			var.val = it->second;
		}
	}
}

void AIUtil::ParseCustomVar(std::string key, object val, AIVar& var)
{
	std::string arr[] = { "float", "int", "bool", "string", "Vector3", "Vector2", "Vector4", "GameObject", "Transform" };
	for (int i = 0; i < 9; i++)
	{
		if (key.find(arr[i])) // startwith
		{
			AIVar* v = new AIVar();
			v->type = arr[i];
			v->name = key;
			v->val = val;
		}
	}
}

std::string AIUtil::TransfType(object type)
{
	return type.get<std::string>();
}

