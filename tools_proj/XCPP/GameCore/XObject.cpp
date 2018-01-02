#include "XObject.h"

XObject::XObject() {}

XObject::~XObject() {}

bool XObject::Initilize()
{
	EventSubscribe();
	return true;
}

void XObject::Uninitilize() 
{
	EventUnsubscribe();
}

void XObject::OnCreated() {}

void XObject::OnEnterScene() {}

void XObject::OnSceneReady() {}

void XObject::OnLeaveScene() {}

void XObject::Unload() {}

void XObject::EventSubscribe() {}

void XObject::EventUnsubscribe() {}

void XObject::RegisterEvent(XEventDefine eventID, XDelegate* handler)
{
	std::unordered_map<uint, XDelegate*>::iterator itr = eventMap.find((uint)eventID);
	if (itr == eventMap.end())
	{
		eventMap.insert(std::make_pair(eventID, handler));
		XEventMgr::Instance()->AddRegist(eventID, this);
	}
}

bool XObject::DispatchEvent(XEventArgs* e)
{
	XEventDefine def = e->getEventDef();
	if (def<0 || def>XEvent_MAX) return false;
	std::unordered_map<uint, XDelegate*>::iterator itr = eventMap.find(def);
	if (itr != eventMap.end())
	{
		XDelegate* eh = itr->second;
		if (eh) (*eh)(e, NULL);
		return true;
	}
	return false;
}




