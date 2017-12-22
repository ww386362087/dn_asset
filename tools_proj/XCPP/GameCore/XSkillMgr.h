#ifndef  __XSkillMgr__
#define  __XSkillMgr__


#include "Common.h"
#include "Vector3.h"
#include <set>

class XEntity;
class XSkill;
class XSkillCore;
class XSkillData;

class XSkillMgr
{
public:
	XSkillMgr(XEntity* host);
	~XSkillMgr();

	XSkill* GetCarrier(int id);
	XSkillCore* GetPhysicalSkill();
	XSkillCore* GetSkill(uint id);

	bool IsCooledDown(XSkillCore* core);
	bool IsCooledDown(uint id);
	void AttachSkill(XSkillData* data, bool attach = true);
	void AttachSkillByID(uint id);
	void DetachSkill(uint id);

private:
	XSkillCore* CreateSkillCore(XSkillData* data);

private:
	XEntity* _host;
	std::unordered_map<int, XSkill*> _carriers;
	std::unordered_map<uint, XSkillCore*> _full_skill;
	std::unordered_map<uint, XSkillCore*> _core;
	std::unordered_map<uint, std::vector<uint> > _qte;
	std::set<uint> _physicals;

	uint _physical;
	uint _ultra;
	uint _appear;
	uint _disappear;
	uint _dash;
	uint _recovery;
	uint _broken;
};

#endif