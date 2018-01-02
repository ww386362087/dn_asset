#ifndef  __GameObjectPool__
#define  __GameObjectPool__

#include <map>
#include "Common.h"
#include "Singleton.h"

class GameObject;

class GameObjectMgr:public Singleton<GameObjectMgr>
{
public:
	void Clear();
	GameObject* Create(const char* name);
	bool Remv(GameObject*);
	bool Remv(const char* name);
	bool Remv(uint id);
	GameObject* Get(uint id);
	size_t Count();

private:
	bool Add(GameObject*);
	std::map<uint, GameObject*> pool;

};


#endif