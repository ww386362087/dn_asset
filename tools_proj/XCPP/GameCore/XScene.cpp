#include "XScene.h"
#include "XLevelSpawnMgr.h"


SceneListRow* XScene::getSceneRow()
{
	return _sceneRow;
}

void XScene::Update(float delta)
{
	XLevelSpawnMgr::Instance()->Update(delta);
}

void XScene::Enter(uint sceneid)
{
	_sceneRow = new SceneListRow();
	//delete _sceneRow;
	iGetSceneListRowByID(sceneid, _sceneRow);
	OnEnterScene(sceneid);
	DoLoad();
}

void XScene::DoLoad()
{
	XEntityMgr::Instance()->CreatePlayer();
}

void XScene::OnEnterScene(uint sceneid)
{
	XLevelSpawnMgr::Instance()->OnEnterScene(sceneid);
}

void XScene::OnEnterSceneFinally()
{

}