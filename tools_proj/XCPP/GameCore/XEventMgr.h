#ifndef  __XEventMgr__
#define  __XEventMgr__

#include "Singleton.h"
#include <unordered_map>
#include <unordered_set>
#include "Common.h"
#include "Log.h"
#include "XTimerMgr.h"
#include "XEventDef.h"

class  XObject;
class  XEventArgs;

class XEventMgr:public Singleton<XEventMgr>
{
public:
	void AddRegist(XEventDefine def, XObject* obj);
	void RemoveRegist(XObject* o);
	bool FireEvent(XEventArgs* arg);
	bool FireEvent(XEventArgs* arg, float delay);
	bool OnDelay(IArgs* arg, void *);
	
private:
	std::unordered_map<int, std::unordered_set<XObject*>> pool;
};


#endif