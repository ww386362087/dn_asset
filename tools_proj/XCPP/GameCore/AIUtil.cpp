#include "AIUtil.h"

std::string  AIUtil::Children = "Children";
std::string AIUtil::Type = "Type";


void AIUtil::Load(std::string name, AITreeData* data)
{
	try
	{
		std::string str = readFile(name.c_str());
		Parse(str, name, data);
	}
	catch (const std::exception& e)
	{
		std::string s = name + " exception:" + tostring(e.what());
		ERR(s);
	}
}

void AIUtil::Parse(std::string json, std::string name, AITreeData* data)
{
	picojson::value v;
	std::string err = picojson::parse(v, json);
	if (err.size())
	{
		ERR(err);
		return;
	}
	picojson::object& o = v.get<picojson::object>();
	data->vars.clear();
	data->task = new AITaskData();
	picojson::array ar = o["Variables"].get<picojson::array>();
	for (picojson::array::iterator it = ar.begin(); it != ar.end(); it++)
	{
		AITreeVar* var = new AITreeVar();
		ParseTreeVar(it->get<picojson::object>(), var);
		data->vars.push_back(var);
	}
	picojson::object root = o["RootTask"].get<picojson::object>();
	ParseTask(root, data->task);
}

void AIUtil::ParseTreeVar(picojson::object& arg, AITreeVar* var)
{
	var->name = arg["Name"].get<std::string>();
	var->type = arg["Type"].get<std::string>();
	var->isShared = arg["IsShared"].get<bool>();
}

Mode AIUtil::Type2Mode(std::string type)
{
	if (type == "Sequence") return Sequence;
	if (type == "Selector") return Selector;
	if (type == "Inverter") return Inverter;
	return Custom;
}

void AIUtil::ParseTask(picojson::object& root, AITaskData* data)
{
	data->children.clear();
	data->vars.clear();
	for (picojson::object::iterator it = root.begin(); it != root.end(); it++)
	{
		bool is_obj = it->second.is<picojson::object>();
		if (it->first == Type)
		{
			data->type = it->second.get<std::string>();
			data->mode = Type2Mode(data->type);
		}
		else if (it->first == Children)
		{
			picojson::array ar = it->second.get<picojson::array>();
			for (picojson::array::iterator iat = ar.begin(); iat != ar.end(); iat++)
			{
				picojson::object obj = iat->get<picojson::object>();
				AITaskData* td = new AITaskData();
				ParseTask(obj, td);
				data->children.push_back(td);
			}
		}
		else if (is_obj)  //sharedvar
		{
			bool isShared = it->second.contains("IsShared");
			AISharedVar* var = new AISharedVar();
			picojson::object ox = it->second.get<picojson::object>();
			ParseSharedVar(it->first, ox, var, isShared);
			data->vars.insert(std::make_pair(it->first, var));
		}
		else
		{
			AIVar* v = new AIVar();
			v->name = it->first;
			v->val = it->second;
			data->vars.insert(std::make_pair(it->first, v));
		}
	}
}

void AIUtil::ParseSharedVar(std::string key, picojson::object& obj, AISharedVar* var,bool shared)
{
	var->name = key;
	var->bindName = obj["Name"].get<std::string>();
	var->isShared = shared ? obj["IsShared"].get<bool>() : false;
	for (picojson::object::iterator it = obj.begin(); it != obj.end(); it++)
	{
		size_t t = it->first.find("Value");
		if (t < 50) //contains value
		{
			var->val = it->second;
		}
	}
}

void AIUtil::ParseCustomVar(std::string key, object val, AIVar* var)
{
	AIVar* v = new AIVar();
	v->name = key;
	v->val = val;
}