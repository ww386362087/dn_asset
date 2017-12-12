#include "Vector3.h"

const Vector3 Vector3::down(0, -1, 0);
const Vector3 Vector3::back(0, 0, -1);
const Vector3 Vector3::forward(0, 0, 1);
const Vector3 Vector3::up(0, 1, 0);
const Vector3 Vector3::left(-1, 0, 0);
const Vector3 Vector3::right(1, 0, 0);
const Vector3 Vector3::one(1, 1, 1);
const Vector3 Vector3::zero(0, 0, 0);

std::ostream& operator<<(std::ostream &stream, const Vector3 &pos)
{
	stream << "(" << pos.x << "," << pos.y << "," << pos.z << ")";
	return stream;
}

float Vector3::Angle(const Vector3& v, const Vector3& u)
{
	if (v.IsZero() || u.IsZero())
	{
		ERROR("Error Vector3 to calc Angle.");
		return 0;
	}

	float f = Dot(v, u) / sqrt(sqrtMagnitude(v) * sqrtMagnitude(u));
	/*
	* in case f == (+-)1.00000001
	*/
	if ((double)f > 1.0) f = 1.0f;
	else if ((double)f < -1.0) f = -1.0f;

	return XRad2Deg(acosf(f));
}

Vector3 Vector3::Normalized(const Vector3& v)
{
	if (v.IsZero()) return Vector3::zero;
	float len = Magnitude(v);
	return Vector3(v.x / len, v.y / len, v.z / len);
}

Vector3 Vector3::Cross(const Vector3& v, const Vector3& u)
{
	return Vector3(v.y * u.z - v.z * u.y,
		v.z * u.x - v.x * u.z,
		v.x * u.y - v.y * u.x);
}

float Vector3::Distance(const Vector3& v, const Vector3& u)
{
	return Magnitude(v - u);
}

float Vector3::Dot(const Vector3& v, const Vector3& u)
{
	return v.x * u.x + v.y * u.y + v.z * u.z;
}

float Vector3::Magnitude(const Vector3& v)
{
	return v.IsZero() ? 0 : sqrt(Dot(v, v));
}

float Vector3::sqrtMagnitude(const Vector3& v)
{
	return Dot(v, v);
}

