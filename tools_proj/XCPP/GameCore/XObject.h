#ifndef  __XObject__
#define  __XObject__

class XObject
{
public:
	XObject();
	~XObject();


	virtual void OnCreated();
	virtual void OnEnterScene();
	virtual void OnSceneReady();
	virtual void OnLeaveScene();


protected:
	virtual bool Initilize();
	virtual void Uninitilize();


};


#endif