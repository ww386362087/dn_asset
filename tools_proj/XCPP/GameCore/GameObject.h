#ifndef  __GameObject__
#define  __GameObject__

class Transform;

class GameObject
{
public:
	GameObject();
	~GameObject();

	const char* tag;
	Transform* transform;
	int layer;
	GameObject* gameObject;

};

#endif