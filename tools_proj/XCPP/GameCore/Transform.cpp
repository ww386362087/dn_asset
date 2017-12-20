#include "Transform.h"



Transform::Transform()
{
	Vector3 vz = Vector3::zero;
	Vector3 vo = Vector3::one;
	position = &vz;
	rotatiion = &vz;
	scale = &vo;
}

Transform::Transform(const char* nm)
{
	name = nm;
	Transform();
}

Transform::~Transform()
{
}


Transform* Transform::Find(const char* name)
{
	return NULL;
}