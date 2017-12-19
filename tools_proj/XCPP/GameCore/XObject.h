#ifndef  __XObject__
#define  __XObject__

#include <unordered_map>
#include "Common.h"
#include "XComponent.h"
#include "XEventDef.h"
#include "XEventMgr.h"

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

protected:
	virtual bool Initilize();
	virtual void Uninitilize();
	virtual void Unload();
	virtual void EventSubscribe();
	virtual void RegisterEvent(XEventDefine eventID, XEventHandler handler);

	std::unordered_map<uint, XComponent*> components;
	std::unordered_map<uint, EventHandler*> _eventMap;
};


#endif