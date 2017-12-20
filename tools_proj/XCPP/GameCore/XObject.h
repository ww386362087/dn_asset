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

	void DetachAllComponents();
	XComponent* GetComponent();
	bool DetachComponent();
	XComponent* AttachComponent();
	template<class T> T* TAttachComponent();
	template<class T> T* TGetComponnet();


protected:
	virtual bool Initilize();
	virtual void Uninitilize();
	virtual void Unload();
	virtual void EventSubscribe();
	virtual void RegisterEvent(XEventDefine eventID, XEventHandler handler);

	std::unordered_map<uint, XComponent*> components;
	std::unordered_map<uint, EventHandler*> _eventMap;
};



template<class T> T* XObject::TAttachComponent()
{
	T* t = new T();
	std::string name= tostring(t);
	t->OnInitial(this);
	return t;
}


template<class T> T* XObject::TGetComponnet()
{
	return 0;
}


#endif