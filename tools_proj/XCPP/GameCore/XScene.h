#ifndef __XScene__
#define __XScene__

#include "SceneList.h"
#include "Common.h"
#include "XEntityMgr.h"
#include "Singleton.h"

class XScene : public Singleton<XScene>
{
public:
	XScene();
	~XScene();
	SceneListRow* getSceneRow();
	void Update(float delta);
	void Enter(uint sceneid);
	void DoLoad();
	void OnEnterScene(uint sceneid);
	void OnEnterSceneFinally();


private:
	SceneListRow* _sceneRow;
};

#endif