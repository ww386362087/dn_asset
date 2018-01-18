#ifndef __XBULLETCORE_H__
#define __XBULLETCORE_H__

#include "../Vector3.h"
#include "XSkillCore.h"
#include "SkillReader.h"
#include "../XEntity.h"

class XBulletCore
{
public:
	XBulletCore();
	~XBulletCore();
	uint Token() { return _token; }
	uint GetFirerID() { return _firer; }
	uint GetTargetID() { return _target; }
	XEntity* GetFirer() { return XEntity::ValidEntity(_firer); }
	XEntity* GetTarget() { return XEntity::ValidEntity(_target); }
	XSkillCore* GetSkillCore() { return _core; }
	int GetSequnce() { return _sequnce; }
	int ResultTime() { return _result_time; }
	bool IsWarning() { return _warning; }
	uint ResultID() { return _result_id; }
	bool HasTarget() { return GetTarget() != NULL; }
	bool FlyWithTerrain() { return _with_terrain; }
	float GetVelocity() { return _velocity; }
	inline float Life() { return _life; }
	inline float Running() { return _running; }
	inline float InitH() { return _init_h; }
	inline float Radius() { return _radius; }
	inline const Vector3& WarningPos() { return _warning_pos; }
	inline const Vector3& Begin() { return _begin; }
	inline const Vector3& FlyTo() { return _fly_to; }
	//const XResultData* GetResultData() const;


private:
	uint _token;
	uint _firer;
	uint _target;

	XSkillCore* _core;
	XResultData* _result;

	int _sequnce;
	uint _result_id;
	int _result_time;

	float _life;
	float _running;
	float _radius;
	float _velocity;
	float _init_h;

	bool _warning;
	bool _with_terrain;

	Vector3 _warning_pos;

	Vector3 _begin;
	Vector3 _fly_to;
};


#endif