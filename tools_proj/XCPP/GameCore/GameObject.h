#ifndef  __GameObject__
#define  __GameObject__

#include "Common.h"

class Transform;

class GameObject
{
public:
	GameObject(const char* nm);
	~GameObject();

	const char* name;
	const char* tag;
	Transform* transform;
	int layer;
	GameObject* gameObject;

private:
	uint uid;

};

#endif