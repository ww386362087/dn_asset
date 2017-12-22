#include "XSkillCore.h"



XSkillCore::XSkillCore()
{
}


XSkillCore::~XSkillCore()
{
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