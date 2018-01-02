#ifndef  __XObject__
#define  __XObject__

#include <unordered_map>
#include "Common.h"
#include "XEventDef.h"
#include "XEventMgr.h"
#include "XComponentDef.h"
#include "XEventArgs.h"
#include "XDelegate.h"

enum UpdateState
{
	NONE,  //不调用
	TIMER, //每秒一次
	FRAME, //每帧调用
	DOUBLE,//每两帧调用
};

class XComponent;
class XEntity;

class XObject
{
public:
	XObject();
	~XObject();

	virtual void OnCreated();
	virtual void OnEnterScene();
	virtual void OnSceneReady();
	virtual void OnLeaveScene();
	virtual void RegisterEvent(XEventDefine eventID, XDelegate* handler);
    virtual bool DispatchEvent(XEventArgs* e);

	
protected:
	virtual bool Initilize();
	virtual void Uninitilize();
	virtual void Unload();
	virtual void EventSubscribe();
	virtual void EventUnsubscribe();

	std::unordered_map<uint, XDelegate*> eventMap;
};


class XComponent :public XObject
{
public:
	XComponent();
	~XComponent();

	virtual void OnUninit();
	virtual void OnInitial(XEntity* _obj);
	void Update(float delta);
	virtual void OnUpdate(float delta);
	inline XEntity* GetHost() { return xenty; }
	XEntity* xenty;

protected:
	UpdateState state = NONE;
	

private:
	float _time = 0;
	bool _double = false;
};



#endif