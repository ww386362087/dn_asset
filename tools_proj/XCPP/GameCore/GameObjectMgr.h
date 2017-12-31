#ifndef  __GameObjectPool__
#define  __GameObjectPool__

#include <unordered_map>
#include "Common.h"
#include "Singleton.h"

class GameObject;

class GameObjectMgr:public Singleton<GameObjectMgr>
{
public:
	~GameObjectMgr();

	GameObject* Create(const char* name);
	bool Remv(GameObject*);
	bool Remv(const char* name);
	bool Remv(uint id);
	GameObject* Get(uint id);
	size_t Count();

private:
	bool Add(GameObject*);
	std::unordered_map<uint, GameObject*> pool;

};


extern "C"
{
	ENGINE_INTERFACE_EXPORT void iGoInfo(const char*,unsigned char, float arr[]);
}


#endif