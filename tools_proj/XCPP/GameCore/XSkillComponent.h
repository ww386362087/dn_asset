#ifndef  __XSkillComponent__
#define  __XSkillComponent__

#include "XObject.h"
#include "skill/XSkill.h"
class XSkillMgr;

class XSkillComponent :public XComponent
{
public:
	XSkillComponent();
	~XSkillComponent();
	XSkillMgr* SkillManager() { return _skillMgr; }
	XSkill* CurrentSkill() { return _skill; }
	bool IsCasting() { return _skill != NULL && _skill->Casting(); }

private:
	XSkill* _skill;
	XSkillMgr* _skillMgr;
};

#endif