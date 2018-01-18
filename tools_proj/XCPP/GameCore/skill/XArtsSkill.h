#ifndef __XARTSSKILL_H__
#define __XARTSSKILL_H__

#include "XSkill.h"
#include "SkillReader.h"


class XEntity;
class XBullet;
class IArgs;

class XArtsSkill:public XSkill
{
public:
	XArtsSkill(XEntity* firer);
	~XArtsSkill();

	bool GroupResults(IArgs* args, void*) { return true; }
	bool LoopResults(IArgs* args, void*) { return true; }

	//skill do self starting here
	virtual void Start() {}
	//skill do self stoppage here
	virtual void Stop();
	//update effects per frame
	virtual bool Present(float fDeltaT) { return true; }
	//set skill result point here
	virtual void Result(XResultData* data) {}

	XBullet* GenerateBullet(XResultData* data, XEntity* target, int additional, int loop, int group, int wid = -1, bool extrainfo = false);

	bool ChargeTo(IArgs* args, void*);
	bool Manipulate(IArgs* args, void*);
	bool KillManipulate(IArgs* args, void*);
	bool Warning(IArgs*, void* param);
	bool Mob(IArgs*, void* param);


protected:
	virtual void FireEvents();

private:
	std::vector<uint> _mob_unit;
};

#endif