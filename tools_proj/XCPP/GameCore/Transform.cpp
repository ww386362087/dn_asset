#include "Transform.h"


Transform::Transform()
{
	position = Vector3::zero;
	rotatiion = Vector3::zero;
	scale = Vector3::one;
}

Transform::Transform(const char* nm)
{
	name = nm;
	Transform();
}

Transform::~Transform()
{
	delete[] name;
	delete parent;
	delete gameObject;
	name = NULL;
}


Transform* Transform::Find(const char* name)
{
	return NULL;
}