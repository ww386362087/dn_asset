#include "GameObject.h"
#include "Transform.h"


GameObject::GameObject()
{
	if (transform == NULL)
	{
		transform = new Transform(name);
		transform->gameObject = this;
	}
}

GameObject::GameObject(const char* nm)
{
	name = nm;
	GameObject();
}

GameObject::~GameObject()
{
}
