#ifndef  __XObject__
#define  __XObject__

#include <unordered_map>
#include "Common.h"
#include "XEventDef.h"
#include "XEventMgr.h"

class XComponent;

class XObject
{
public:
	XObject();
	~XObject();

	virtual void OnCreated();
	virtual void OnEnterScene();
	virtual void OnSceneReady();
	virtual void OnLeaveScene();
    virtual bool DispatchEvent(XEventArgs* e);

	template<typename T> T* GetComponent();
	template<typename T> bool DetachComponent();
	template<typename T> T* AttachComponent();
	void DetachAllComponents();


protected:
	virtual bool Initilize();
	virtual void Uninitilize();
	virtual void Unload();
	virtual void EventSubscribe();
	virtual void RegisterEvent(XEventDefine eventID, XEventHandler handler);

	std::unordered_map<uint, XComponent*> components;
	std::unordered_map<uint, EventHandler*> _eventMap;
};


template<typename T> T* XObject::GetComponent()
{
	uint uid = xhash(typeid(T).Name);
	return components[uid];
}

template<typename T> bool XObject::DetachComponent()
{
	uint uid = xhash(typeid(T).Name);
	std::unordered_map<uint, XComponent*>::iterator  itr = components.find(uid);
	if (itr!= components.end())
	{
		itr->second->OnUninit();
		components.erase(uid);
		return true;
	}
	return false;
}

template<typename T> T* XObject::AttachComponent()
{
	uint uid = xhash(typeid(T).Name);
	std::unordered_map<uint, XComponent*>::iterator  itr = components.find(uid);
	if (itr != components.end())
	{
		return components[uid];
	}
	else
	{
		T* t = new T();
		XComponent* comp = dynamic_cast<XComponent*>(t);
		comp->OnInitial(this);
		components.insert(uid, comp);
		return t;
	}
}

#endif