#include "Transform.h"



Transform::Transform(const char* nm)
{
	name = nm;
	position = Vector3::zero;
	rotatiion = Vector3::zero;
	scale = Vector3::one;
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