#include "XScene.h"

XScene::XScene()
{
}


XScene::~XScene()
{
	delete _sceneRow;
}


SceneListRow* XScene::getSceneRow()
{
	return _sceneRow;
}

void XScene::Update(float delta)
{

}

void XScene::Enter(uint sceneid)
{
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

}

void XScene::OnEnterSceneFinally()
{

}