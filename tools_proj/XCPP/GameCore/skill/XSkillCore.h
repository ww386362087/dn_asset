#ifndef  __XSkillCore__
#define  __XSkillCore__

#include "../Common.h"
#include "../XStateDefine.h"
#include <set>

class XEntity;
class XSkill;
class XSkillData;
class Enemy;

class XSkillCore
{
public:
	XSkillCore(XEntity* firer, XSkillData* data);
	~XSkillCore();

	bool CooledDown() const { return !_is_init_cooling && _current_running_time > 0; }
	bool HasInitCD() const { return _init_cd > 0; }
	void CheckRunningTime()
	{
		if (_current_running_time < 0) _current_running_time = 0;
		else if (_current_running_time > _totally_running_time) _current_running_time = _totally_running_time;
	}
	int CarrierID() const { return _carrier_id; }
	XSkill* Carrier() const { return _carrier; }
	uint ID() const { return _id; }
	uint Level() const { return _skill_level; }
	void Halt() { _carrier = NULL; }
	XSkillData* GetSoul() const { return _soul; }
	bool CanCast(uint token);

	bool Fire(XSkill* carrier);
	void Execute(XSkill* carrier);
	void OnCdCall(int left_running_time, bool syntonic = false);
	float GetTimeScale() const;
	float GetElapsedCD();
	float GetLeftCD();
	void ClearHurtTarget();
	void ClearWarningPos();

private:
	uint _magic_num;
	uint _id;
	int _carrier_id;
	int _current_running_time;
	int _totally_running_time;
	int _preserved_strength;

	XSkillData* _soul;
	XSkill* _carrier;
	XEntity* _firer;

	bool  _is_init_cooling;
	bool  _ever_fired;
	bool  _is_on_syntonic;
	float _static_cd;
	float _init_cd;
	uint _skill_level;


	std::vector<std::set<uint> > _hurt_target;

	float _semi_dynamic_cd_ratio;
	float _last_dynamic_cd;
	float _dynamic_cd_ratio;
	float _dynamic_cd_delta;

	uint _sync_at_sequence;
};

#endif