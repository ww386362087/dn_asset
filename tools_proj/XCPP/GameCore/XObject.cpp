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

void XObject::RegisterEvent(XEventDefine eventID, XEventHandler handler)
{
	std::unordered_map<uint, EventHandler*>::iterator itr = _eventMap.find((uint)eventID);
	if (itr == _eventMap.end())
	{
		EventHandler* eh = new EventHandler();
		eh->eventDefine = eventID;
		eh->handler = handler;
		_eventMap.insert(std::make_pair(eventID, eh));
		XEventMgr::Instance()->AddRegist(eventID, this);
	}
}

bool XObject::DispatchEvent(XEventArgs* e)
{
	XEventDefine def = e->GetEventDef();
	std::unordered_map<uint, EventHandler*>::iterator itr = _eventMap.find((uint)def);
	if (itr != _eventMap.end())
	{
		EventHandler* eh = itr->second;
		if (eh) eh->handler(e);
		return true;
	}
	return false;
}


void XObject::DetachAllComponents()
{
	std::unordered_map<uint, XComponent*>::iterator itr;
	for (itr = components.begin(); itr != components.end(); itr++)
	{
		itr->second->OnUninit();
	}
	components.clear();
}

