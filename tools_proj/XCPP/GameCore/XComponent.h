#ifndef  __XComponent__
#define  __XComponent__

#include "XObject.h"

enum UpdateState
{
	NONE,  //不调用
	TIMER, //每秒一次
	FRAME, //每帧调用
	DOUBLE,//每两帧调用
};

class XComponent:public XObject
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

#endif