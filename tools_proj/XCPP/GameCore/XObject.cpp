#include "XObject.h"
#include "XComponent.h"

XObject::XObject() {}

XObject::~XObject() {}

bool XObject::Initilize()
{
	return true;
}

void XObject::Uninitilize()
{
}

void XObject::OnCreated()
{
}

void XObject::OnEnterScene()
{
}

void XObject::OnSceneReady()
{
}

void XObject::OnLeaveScene()
{
}

void XObject::Unload()
{
	delete this;
}

void XObject::EventSubscribe()
{

}

void XObject::RegisterEvent(XEventDefine eventID, XEventHandler handler)
{

}

bool XObject::DispatchEvent(XEventArgs* e)
{
	return true;
}


void XObject::DetachAllComponents()
{

}

XComponent* XObject::GetComponent()
{
	uint uid = xhash("typeid");
	return components[uid];
}

bool XObject::DetachComponent()
{
	uint uid = xhash("typeid(T).name()");
	std::unordered_map<uint, XComponent*>::iterator  itr = components.find(uid);
	if (itr != components.end())
	{
		XComponent* cp = itr->second;
		cp->OnUninit();
		components.erase(uid);
		return true;
	}
	return false;
}

XComponent* XObject::AttachComponent()
{
	uint uid = xhash("typeid(T).name()");
	std::unordered_map<uint, XComponent*>::iterator  itr = components.find(uid);
	if (itr != components.end())
	{
		return components[uid];
	}
	else
	{
		XComponent* comp = new XComponent();
		comp->OnInitial(this);
		components.insert(std::make_pair(uid, comp));
		return comp;
	}
}