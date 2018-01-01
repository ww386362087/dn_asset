#include "GameObjectMgr.h"
#include "GameObject.h"
#include "Transform.h"
#include "Common.h"
#include "CommandDef.h"
#include "NativeInterface.h"

extern SharpCALLBACK callback;


void GameObjectMgr::Clear()
{
	for (std::map<uint, GameObject*>::iterator it = pool.begin(); it != pool.end(); it++)
	{
		delete it->second;
	}
	pool.clear();
}

GameObject* GameObjectMgr::Create(const char* name)
{
	uint hash = xhash(name);
	if (pool.find(hash) == pool.end())
	{
		GameObject* go = new GameObject(name);
		callback(CMLoadGo, name);
		Add(go);
		return go;
	}
	return pool[hash];
}

bool GameObjectMgr::Add(GameObject* go)
{
	const char* name = go->name;
	uint hash = xhash(name);
	if (pool.find(hash) == pool.end())
	{
		pool.insert(std::make_pair(hash, go));
		return true;
	}
	return false;
}

bool GameObjectMgr::Remv(GameObject* go)
{
	const char* name = go->name;
	return  Remv(name);
}


bool GameObjectMgr::Remv(const char* name)
{
	uint hash = xhash(name);
	return Remv(hash);
}

bool GameObjectMgr::Remv(uint hash)
{
	if (pool.find(hash) != pool.end())
	{
		callback(CMUnloadGo, pool[hash]->name);
		delete pool[hash];
		pool.erase(hash);
		return true;
	}
	return false;
}

GameObject* GameObjectMgr::Get(uint id)
{
	if (pool.find(id) != pool.end())
	{
		return pool[id];
	}
	return NULL;
}

size_t GameObjectMgr::Count()
{
	return pool.size();
}

extern "C"
{
	void iGoInfo(const char* name, unsigned char prop, float* arr)
	{
		uint hash = xhash(name);
		GameObject* go = GameObjectMgr::Instance()->Get(hash);
		if (go == NULL) return;
		switch (prop)
		{
		case 'p':
		case 'P':
			arr = vec2arr(go->transform->position);
			break;
		case 'r':
		case 'R':
			arr = vec2arr(go->transform->rotation);
			break;
		case 's':
		case 'S':
			arr = vec2arr(go->transform->scale);
			break;
		case 'f':
		case 'F':
			arr = vec2arr(go->transform->forward);
			break;
		default:
			break;
		}
	}
}