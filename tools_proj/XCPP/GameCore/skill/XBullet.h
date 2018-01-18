#ifndef  __XBullet__
#define  __XBullet__

#include "../Vector3.h"
#include "XBulletCore.h"


struct XBulletTarget
{
public:
	XBulletTarget()
		:ID(0),
		TimerToken(0),
		Hurtable(true),
		HurtCount(0)
	{}

	uint ID;
	uint TimerToken;
	bool Hurtable;
	int HurtCount;
};


class XBullet
{
public:
	XBullet();
	~XBullet();

private:
	uint _magic_num;

	uint _id;
	uint _extra_id;

	bool _client;
	bool _active;
	bool _pingpong;
	bool _hit_triggered;

	int _layer_mask;
	float _elapsed;

	XBulletCore* _data;

	Vector3 _origin;
	Vector3 _position;

	int _tail_results;
	uint _tail_results_token;

	std::map<uint, XBulletTarget> _hurt_target;
};

#endif