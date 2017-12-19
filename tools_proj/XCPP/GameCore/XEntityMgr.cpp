#include "XEntityMgr.h"
#include "XEntity.h"
#include "XAttributes.h"

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

XEntity* XEntityMgr::PrepareEntity(XAttributes* attr)
{
	XEntity* x = new XEntity();
	GameObject* o = new GameObject();// XResources.LoadInPool("Prefabs/" + attr.Prefab);
	o->name = tostring(attr->getid()).c_str();
	o->transform->position = attr->getAppearPostion();
	o->transform->rotatiion = attr->getAppearQuaternion();
	//x->Initilize();
	uint id = attr->getid();
	if (!x->IsPlayer()) _dic_entities.insert(std::make_pair(id, x));
	return x;
}

XEntity* XEntityMgr::CreateEntity(uint staticid, Vector3* pos, Vector3* rot)
{
	XAttributes* attr = new XAttributes();// InitAttrFromClient((int)staticid);
	attr->setAppearPosition(pos);
	attr->setAppearQuaternion(rot);
	return PrepareEntity(attr);
	//Add(Entity, e);
}

void XEntityMgr::UnloadEntity(uint id)
{
	std::unordered_map<uint, XEntity*>::iterator itr = _dic_entities.find(id);
	if (itr != _dic_entities.end())
	{
		_hash_entities.erase(itr->second);
		itr->second->UnloadEntity();
		_dic_entities.erase(id);
	}
	std::unordered_map<int, std::vector<XEntity*>>::iterator it;
	for (it = _map_entities.begin(); it != _map_entities.end(); it++)
	{
		std::vector<XEntity*> vec = it->second;
		std::vector<XEntity*>::iterator vtr;
		for (vtr = vec.begin(); vtr != vec.end();)
		{
			if ((*vtr)->EntityID == id)
			{
				vec.erase(vtr);
			}
			else
			{
				vtr++;
			}
		}
	}
}

void XEntityMgr::UnloadAll()
{
	std::unordered_set<XEntity*>::iterator itr;
	for (itr = _hash_entities.begin(); itr != _hash_entities.end(); itr++)
	{
		(*itr)->UnloadEntity();
		delete *itr;
	}
	_hash_entities.clear();
	_map_entities.clear();
	_dic_entities.clear();
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



