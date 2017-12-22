#ifndef  __XSkill__
#define  __XSkill__

#include "Common.h"
#include "XEventArgs.h"
#include "Vector3.h"

class XEntity;
class XSkillCore;
class XSkillData;

class XSkill
{
public:
	XSkill(XEntity* ent);
	~XSkill();

	bool Update(float fDeltaT);
	bool Fire(uint target, XSkillCore* pCore, XAttackArgs* pargs);
	void Cease();
	void Execute();
	bool OnResult(IArgs*, void* param);
	void AddedTimerToken(uint token) { _tokens.push_back(token); }

	static const int XJAComboSkillHash;
	static const int XArtsSkillHash;
	static const int XCombinedSkillHash;

protected:
	virtual void Result(XResultData* data) = 0;

protected:
	XEntity* _firer;
	uint _token;
	int _slot_pos;
	XSkillCore* _core;
	XSkillData* _data;
	uint _target;
	float _time_scale;
	uint _fire_at;

private:
	bool _casting;
	bool _execute;
	Vector3 _skill_forward;
	std::vector<uint> _tokens;
};


#endif