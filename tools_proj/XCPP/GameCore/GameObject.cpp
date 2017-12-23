#include "GameObject.h"
#include "Transform.h"
#include "NativeInterface.h"
#include "CommandDef.h"

extern SharpCALLBACK callback;

GameObject::GameObject(const char* nm)
{
	name = nm;
	if (transform == NULL)
	{
		transform = new Transform(name);
		transform->gameObject = this;
		callback(CMLoadGo, name);
	}
}

GameObject::~GameObject()
{
}


void GameObject::Unload()
{
	if (name)
	{
		callback(CMUnloadGo, name);
	}
}