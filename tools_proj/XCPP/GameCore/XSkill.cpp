#include "XSkill.h"
#include "XEntity.h"
#include "XRole.h"
#include "SkillReader.h"

const int XSkill::XJAComboSkillHash = 0;
const int XSkill::XArtsSkillHash = 1;
const int XSkill::XCombinedSkillHash = 3;

XSkill::XSkill(XEntity* ent)
	:_firer(ent),
	_token(0),
	_slot_pos(-1),
	_core(NULL),
	_data(NULL),
	_target(0),
	_time_scale(1.0f),
	_fire_at(0),
	_casting(false),
	_execute(false),
	_skill_forward(Vector3::forward)
{
}


XSkill::~XSkill()
{
}

bool XSkill::Update(float fDeltaT)
{
	return true;
}

bool XSkill::Fire(uint target, XSkillCore* pCore, XAttackArgs* pargs)
{
	return true;
}

void XSkill::Cease()
{

}

void XSkill::Execute()
{

}

bool XSkill::OnResult(IArgs* args, void* param)
{
	bool result = true;
	if (result)
	{
		if (_firer->IsRole())
		{
			XRole* role = dynamic_cast<XRole*>(_firer);
			//role->Statistics().AppendTime();
		}

		Result(reinterpret_cast<XResultData*>(param));
	}

	return true;
}