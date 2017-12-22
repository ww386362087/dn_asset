#include "XSkillCore.h"
#include "XStateMachine.h"
#include "XEntity.h"

XSkillCore::XSkillCore(XEntity* firer, XSkillData* data)
	:_magic_num(0xFEDCBA98),
	_id(0),
	_current_running_time(1),
	_totally_running_time(1),
	_preserved_strength(0),
	_soul(data),
	_carrier(NULL),
	_firer(firer),
	_is_init_cooling(false),
	_ever_fired(false),
	_is_on_syntonic(false),
	_static_cd(1),
	_init_cd(1),
	_skill_level(0)
{
	
}


XSkillCore::~XSkillCore()
{
}

bool XSkillCore::CanCast(uint token)
{
	if (token > 0 && token == _firer->StateMachine()->ActionToken())
	{
		return true;
	}
	XStateDefine current = _firer->StateMachine()->GetCurrent()->IsFinished() ? _firer->StateMachine()->GetDefault()->SelfState() : _firer->StateMachine()->GetCurrentState();
	return current == XState_Idle || current == XState_Move || current == XState_Charge;
}

bool XSkillCore::Fire(XSkill* carrier)
{
	if (_ever_fired) return false;
	if (carrier != NULL && CooledDown())
	{
		_carrier = carrier;
		OnCdCall(_current_running_time - 1);
		return true;
	}
	else
		return false;
}

void XSkillCore::Execute(XSkill* carrier)
{
	_carrier = carrier;
	ClearHurtTarget();
	ClearWarningPos();

	//if (TryCalcRandomWarningPos())
	//{
	//	if (_firer->NetComponent()) _firer->NetComponent()->BroadcastRandomWarningPos(ID(), WarningRandomItems);
	//}
}

void XSkillCore::ClearHurtTarget()
{
	//std::vector<set<uint> >::iterator it = _hurt_target.begin();
	//while (it != _hurt_target.end())
	//{
	//	(*it).clear();
	//	++it;
	//}
}

void XSkillCore::ClearWarningPos()
{
	//std::vector<std::vector<XSkillWarningPackage> >::iterator it = WarningPosAt.begin();
	//while (it != WarningPosAt.end())
	//{
	//	(*it).clear();
	//	++it;
	//}
}

void XSkillCore::OnCdCall(int left_running_time, bool syntonic)
{

}

float XSkillCore::GetTimeScale() const
{
	return 1;
	//return (float)(XSkillEffectMgr::Instance()->GetAttackSpeedRatio(_firer->GetUnit()->GetAttrPtr()));
}

float XSkillCore::GetElapsedCD()
{
	return 1;
}

float XSkillCore::GetLeftCD()
{
	return 1.0f;
}