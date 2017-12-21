#ifndef  __XEventMgr__
#define  __XEventMgr__

#include "Singleton.h"
#include <unordered_map>
#include <unordered_set>
#include "Common.h"
#include "Log.h"
#include "XTimerMgr.h"
#include "ITimerCallback.h"
#include "XEventDef.h"

class  XObject;
class  XEventArgs;

class XEventMgr:public Singleton<XEventMgr>,ITimerCallback
{
public:
	void AddRegist(XEventDefine def, XObject* obj);
	void RemoveRegist(XObject* o);
	bool FireEvent(XEventArgs* arg);
	virtual void TimerCallback(ITimerArg* arg);
	bool FireEvent(XEventArgs* arg, float delay);
	
	
private:
	std::unordered_map<int, std::unordered_set<XObject*>> pool;
};


#endif