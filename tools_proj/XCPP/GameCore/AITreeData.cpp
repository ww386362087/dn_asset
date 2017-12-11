#include "AITreeData.h"


void AITreeData::SetVariable(std::string name, object value)
{
	uint hash = xhash(name.c_str());
	cache[hash] = value;
}

std::unordered_map<uint, object> AITreeData::GetCache()
{
	return this->cache;
}