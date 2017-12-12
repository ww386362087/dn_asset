#ifndef __XVECTOR3_H__
#define __XVECTOR3_H__

#include <iosfwd>
#include "XMath.h"
#include "Log.h"

class Vector3;
std::ostream &operator<<(std::ostream &stream, const Vector3 &pos);

class Vector3
{
public:
	float x;
	float y;
	float z;

	Vector3()
		:x(0), y(0), z(0) {};
	Vector3(float x1, float y1, float z1)
		:x(x1), y(y1), z(z1) { }
	Vector3(const Vector3& v)
		:x(v.x), y(v.y), z(v.z) { }

	Vector3 operator -() const { return Vector3(-x, -y, -z); }

	void operator =(const Vector3& v) { x = v.x; y = v.y; z = v.z; }

	Vector3 operator -(const Vector3& v) const { return Vector3(x - v.x, y - v.y, z - v.z); }
	
	Vector3 operator +(const Vector3& v) const { return Vector3(x + v.x, y + v.y, z + v.z); }

	Vector3 operator *(const Vector3& v) const { return Vector3(v.x * x, v.y * y, v.z * z); }

	bool operator ==(const Vector3& v) const { return !((v.x != x) || (v.y != y) || (v.z != z)); }

	bool operator !=(const Vector3& v) const { return (v.x != x) || (v.y != y) || (v.z != z); }

	void Normalize();

	void Zero() { x = 0; y = 0; z = 0; }

	bool IsZero() const { return x == 0 && y == 0 && z == 0; }

	static float Dot(float v1, float u1, float v2, float u2)
	{
		return v1 * u2 - v2 * u1;
	}

	static float Angle(const Vector3& v, const Vector3& u);

	static Vector3 Normalized(const Vector3& v);

	static Vector3 Cross(const Vector3& v, const Vector3& u);

	static float Distance(const Vector3& v, const Vector3& u);

	static float Dot(const Vector3& v, const Vector3& u);

	static float Magnitude(const Vector3& v);

	static float sqrtMagnitude(const Vector3& v);

	inline void Horizontal()
	{
		y = 0;
		Normalize();
	}

	static const Vector3 down;
	static const Vector3 back;
	static const Vector3 forward;
	static const Vector3 up;
	static const Vector3 left;
	static const Vector3 right;
	static const Vector3 one;
	static const Vector3 zero;
};

#endif