#include "XEventMgr.h"
#include "XObject.h"


void XEventMgr::AddRegist(XEventDefine def, XObject* obj)
{
	int idef = (int)def;
	pool[idef].insert(obj);
}

void XEventMgr::RemoveRegist(XObject* o)
{
	std::unordered_map<int, std::unordered_set<XObject*>>::iterator it;
	for (it = pool.begin(); it != pool.end(); it++)
	{
		delete o;
		it->second.erase(o);
		o = NULL;
	}
}

bool XEventMgr::FireEvent(XEventArgs* arg)
{
	XEventDefine def = arg->GetEventDef();
	int idef = (int)def;
	std::unordered_map<int, std::unordered_set<XObject*>>::iterator itr = pool.find(idef);
	if (itr != pool.end())
	{
		std::unordered_set<XObject*> set = pool[idef];
		std::unordered_set<XObject*>::const_iterator it;
		for (it = set.begin(); it != set.end(); it++)
		{
			(*it)->DispatchEvent(arg);
		}
	}
	return true;
}

void XEventMgr::TimerCallback(ITimerArg* arg)
{
	LOG("CALLBACK");
	XEventArgs* e_arg = (XEventArgs*)arg;
	FireEvent(e_arg);
}

bool XEventMgr::FireEvent(XEventArgs* arg, float delay)
{
	/*ITimerArg* t_arg = dynamic_cast<ITimerArg*>(arg);
	XTimerMgr::Instance()->SetTimer(delay, this, t_arg);*/
	return true;
}
