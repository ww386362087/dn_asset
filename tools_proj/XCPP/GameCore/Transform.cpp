#include "Transform.h"
#include "GameObject.h"


Transform::Transform(const char* nm)
{
	name = nm;
	position = Vector3::zero;
	rotation = Vector3::zero;
	scale = Vector3::one;
}

Transform::~Transform()
{
	//delete[] name;
}


Transform* Transform::Find(const char* name)
{
	return NULL;
}