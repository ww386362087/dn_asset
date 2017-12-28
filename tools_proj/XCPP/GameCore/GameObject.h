#ifndef  __GameObject__
#define  __GameObject__

class Transform;

class GameObject
{
public:
	GameObject(const char* nm);
	~GameObject();
	void Unload();

	const char* name;
	const char* tag;
	Transform* transform;
	int layer;
	GameObject* gameObject;

};

#endif