#ifndef  __XObject__
#define  __XObject__

#include <unordered_map>
#include "Common.h"
#include "XEventDef.h"
#include "XEventMgr.h"

enum UpdateState
{
	NONE,  //不调用
	TIMER, //每秒一次
	FRAME, //每帧调用
	DOUBLE,//每两帧调用
};


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
	template<class T> T* AttachComponent();
	template<class T> T* GetComponnet();
	template<class T> bool DetachComponent();

protected:
	virtual bool Initilize();
	virtual void Uninitilize();
	virtual void Unload();
	virtual void EventSubscribe();
	virtual void EventUnsubscribe();
	virtual void RegisterEvent(XEventDefine eventID, XEventHandler handler);

	std::unordered_map<uint, XComponent*> components;
	std::unordered_map<uint, EventHandler*> _eventMap;
};


class XComponent :public XObject
{
public:
	XComponent();
	~XComponent();

	virtual void OnUninit();
	virtual void OnInitial(XObject* _obj);
	void Update(float delta);
	virtual void OnUpdate(float delta);
	XObject* xobj;

protected:
	UpdateState state = NONE;

private:
	float _time = 0;
	bool _double = false;
};


template<class T> T* XObject::AttachComponent()
{
	std::string name = typeid(T).name();
	uint uid = xhash(name.c_str());
	std::unordered_map<uint, XComponent*>::iterator  itr = components.find(uid);
	if (itr != components.end())
	{
		return dynamic_cast<T*>(components[uid]);
	}
	else
	{
		T* t = new T();
		t->OnInitial(this);
		components.insert(std::make_pair(uid, t));
		return t;
	}
}

template<class T> T* XObject::GetComponnet()
{
	std::string name = typeid(T).name();
	uint uid = xhash(name.c_str());
	return dynamic_cast<T*>(components[uid]);
}

template<class T> bool XObject::DetachComponent()
{
	std::string name = typeid(T).name();
	uint uid = xhash(name.c_str());
	std::unordered_map<uint, XComponent*>::iterator  itr = components.find(uid);
	if (itr != components.end())
	{
		itr->second->OnUninit();
		components.erase(uid);
		return true;
	}
	return false;
}

#endif