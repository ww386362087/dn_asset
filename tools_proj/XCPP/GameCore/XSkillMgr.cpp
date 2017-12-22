#include "XSkillMgr.h"
#include "XSkillCore.h"
#include "XSkill.h"
#include "SkillReader.h"
#include "XEntity.h"
#include "SkillReader.h"
#include "XArtsSkill.h"
#include "XJAComboSkill.h"

XSkillMgr::XSkillMgr(XEntity* host)
{
}

XSkillMgr::~XSkillMgr()
{
}

XSkill* XSkillMgr::GetCarrier(int id)
{
	std::unordered_map<int, XSkill*>::iterator it = _carriers.find(id);
	if (it == _carriers.end())
	{
		XSkill* pSkill = NULL;
		switch (id)
		{
		case 0: pSkill = new XJAComboSkill(_host); break;
		case 1: pSkill = new XArtsSkill(_host); break;
		}
		if (pSkill) _carriers[id] = pSkill;
		return pSkill;
	}
	else
		return it->second;
}

XSkillCore* XSkillMgr::GetPhysicalSkill()
{
	return 0;
}

void XSkillMgr::AttachSkillByID(uint id)
{
	std::unordered_map<uint, XSkillCore*>::iterator it = _full_skill.find(id);
	if (it != _full_skill.end())
	{
		AttachSkill(it->second->GetSoul());
	}
}

XSkillCore* XSkillMgr::GetSkill(uint id)
{
	std::unordered_map<uint, XSkillCore*>::iterator it = _core.find(id);
	return (it == _core.end()) ? NULL : it->second;
}

bool XSkillMgr::IsCooledDown(XSkillCore* core)
{
	return core->CooledDown();
}

bool XSkillMgr::IsCooledDown(uint id)
{
	return IsCooledDown(GetSkill(id));
}

XSkillCore* XSkillMgr::CreateSkillCore(XSkillData* data)
{
	return new XSkillCore(_host, data);
}

void XSkillMgr::AttachSkill(XSkillData* data, bool attach)
{
	uint id = xhash(data->Name);
	if (_full_skill.find(id) == _full_skill.end())
	{
		XSkillCore* pCore = CreateSkillCore(data);
		_full_skill[id] = pCore;
	}

	//XSkillCore* pSkillCore = _full_skill[id];
	///*
	//* in case PVP version due to data is not as the same as pSkillCore->GetSoul();
	//*/
	//data = pSkillCore->GetSoul();

	//if (pSkillCore->Level() == 0 && !_host->IsTransform() && !data->ForCombinedOnly) return;

	//if (_core.find(id) != _core.end()) return;

	//_core[id] = pSkillCore;

	//if (!data->ForCombinedOnly && attach)
	//{
	//	if (id != _physical)
	//		_skill_order.push_back(pSkillCore);
	//}

	//int carrier_id = data->TypeToken;

	//if (_carriers.find(carrier_id) == _carriers.end())
	//{
	//	XSkill* pSkill = NULL;

	//	switch (carrier_id)
	//	{
	//	case 0: pSkill = new XJAComboSkill(_host); break;
	//	case 1: pSkill = new XArtsSkill(_host); break;
	//	}

	//	if (NULL != pSkill)
	//	{
	//		_carriers[carrier_id] = pSkill;
	//	}
	//}
}



void XSkillMgr::DetachSkill(uint id)
{
	std::unordered_map<uint, XSkillCore*>::iterator it = _core.find(id);
	if (it != _core.end())
	{
		if (it->first == _physical) _physical = 0;
		if (it->first == _ultra) _ultra = 0;
		if (it->first == _appear) _appear = 0;
		if (it->first == _disappear) _disappear = 0;

		_physicals.erase(it->first);

		/*std::vector<XSkillCore*>::iterator vit = _skill_order.begin();
		while (vit != _skill_order.end())
		{
			if ((*vit) == it->second)
				vit = _skill_order.erase(vit);
			else
				++vit;
		}*/

		_core.erase(it);
	}
}