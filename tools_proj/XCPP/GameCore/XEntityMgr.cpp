#include "XEntityMgr.h"
#include "XEntity.h"

XEntityMgr::XEntityMgr()
{

}

XEntityMgr::~XEntityMgr()
{

}

void XEntityMgr::Update(float delta)
{
	for (std::unordered_set<XEntity*>::iterator itr = _hash_entities.begin(); itr != _hash_entities.end(); itr++)
	{
		(*itr)->Update(delta);
	}
}

void XEntityMgr::LateUpdate()
{
	for (std::unordered_set<XEntity*>::iterator itr = _hash_entities.begin(); itr != _hash_entities.end(); itr++)
	{
		(*itr)->LateUpdate();
	}
}

void XEntityMgr::AttachToHost()
{
	for (std::unordered_set<XEntity*>::iterator itr = _hash_entities.begin(); itr != _hash_entities.end(); itr++)
	{
		(*itr)->AttachToHost();
	}
}

void XEntityMgr::DetachFromHost()
{
	for (std::unordered_set<XEntity*>::iterator itr = _hash_entities.begin(); itr != _hash_entities.end(); itr++)
	{
		(*itr)->DetachFromHost();
	}
}

XEntity* XEntityMgr::GetEntity(uint id)
{
	return NULL;
}

XRole* XEntityMgr::CreateTestRole()
{
	return NULL;
}

XPlayer* XEntityMgr::CreatePlayer()
{
	return NULL;
}