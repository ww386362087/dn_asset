#include "XJAComboSkill.h"
#include "XRole.h"
#include "XEntity.h"
#include "XSkillMgr.h"
#include "XSkillCore.h"
#include "XEventArgs.h"
#include "XRole.h"

XJAComboSkill::XJAComboSkill(XEntity* firer)
	:XArtsSkill(firer)
{
	_during_ja = false;
}

XJAComboSkill::~XJAComboSkill()
{
}

void XJAComboSkill::Start()
{
	_during_ja = false;
	JAAt();
	XArtsSkill::Start();
}

void XJAComboSkill::Stop()
{
	XArtsSkill::Stop();
	_during_ja = false;
}

bool XJAComboSkill::Refire(IArgs* args, void*)
{
	uint id = GetNextJAIdentify();
	if (id != 0)
	{
		XSkillMgr* mgr = _firer->SkillManager();
		XSkillCore* core = mgr->GetSkill(id);
		if (core && core->CanCast(_token))
		{
			XRole* role = dynamic_cast<XRole*>(_firer);
			XAttackArgs attck_arg;
			attck_arg.Slot = _slot_pos;
			attck_arg.Identify = id;
			attck_arg.Target = _target;
			/*if (role) role->ZeroSlotLastTarget();*/
			_firer->DispatchEvent(&attck_arg);
		}
		else
		{
			_during_ja = false;
			if (_data) ERROR("JA Failed!");
		}
	}
	else
	{
		_during_ja = false;
	}
	return true;
}

uint XJAComboSkill::GetNextJAIdentify()
{
	uint id = 0;

	if (ValidJA())
	{
		XJAData jd = _data->Ja[0];
		bool trigger = false;
		XRole* role = dynamic_cast<XRole*>(_firer);
		if (role)
		{
			/*if (role->IsClientAutoFight())
				trigger = true;
			else
			{
				uint swype = role->SlotLastAttackAt(_slot_pos);
				float trigger_at = (swype - _fire_at) / 1000.0f;
				trigger = (trigger_at < jd.End * _time_scale && trigger_at > jd.At * _time_scale);
			}*/
		}
		else
			trigger = true;

		if (trigger)
		{
			//if (role) role->ZeroSlotLastAttack(_slot_pos);
			id = xhash(jd.Name);
		}
		else
			id = xhash(jd.Next_Name);
	}
	return id;
}

void XJAComboSkill::JAAt()
{
	if (_data->Ja.size() == 1)
	{
		XDelegate del = DelCALLBACK(XJAComboSkill, Refire, this);
		AddedTimerToken(XTimerMgr::Instance()->SetTimer((float)(_data->Ja[0].Point * _time_scale), &del));
		_during_ja = true;
	}
}

bool XJAComboSkill::ValidJA()
{
	return true;
}