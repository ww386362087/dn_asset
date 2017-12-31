#include "GameObject.h"
#include "Transform.h"
#include "CommandDef.h"


GameObject::GameObject(const char* nm)
{
	name = nm;
	if (transform == NULL)
	{
		transform = new Transform(name);
		uid = xhash(name);
		transform->position = Vector3::zero;
		transform->rotation = Vector3::zero;
		transform->scale = Vector3::one;
		transform->gameObject = this;
	}
}

GameObject::~GameObject()
{
	delete transform;
	delete gameObject;
	//delete[] name;
	//delete[] tag;
	//name = NULL;
	//tag = NULL;
	transform = NULL;
	gameObject = NULL;
}


