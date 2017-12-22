#include "AITreeData.h"


void AITreeData::SetVariable(std::string name, float value)
{
	uint hash = xhash(name.c_str());
	f_cache[hash] = value;
}

void AITreeData::SetVariable(std::string name, int value)
{
	uint hash = xhash(name.c_str());
	i_cache[hash] = value;
}

void AITreeData::SetVariable(std::string name, uint value)
{
	uint hash = xhash(name.c_str());
	u_cache[hash] = value;
}

void AITreeData::SetVariable(std::string name, bool value)
{
	uint hash = xhash(name.c_str());
	b_cache[hash] = value;
}

void AITreeData::SetVariable(std::string name, GameObject* value)
{
	uint hash = xhash(name.c_str());
	g_cache[hash] = value;
}

std::unordered_map<uint, float> AITreeData::GetFloatCache()
{
	return this->f_cache;
}

std::unordered_map<uint, int> AITreeData::GetIntCache()
{
	return this->i_cache;
}

std::unordered_map<uint, uint> AITreeData::GetUintCache()
{
	return this->u_cache;
}

std::unordered_map<uint, bool> AITreeData::GetBoolCache()
{
	return this->b_cache;
}

std::unordered_map<uint, GameObject*> AITreeData::GetGoCache()
{
	return this->g_cache;
}

