#include "XEntityMgr.h"
#include "XEntity.h"
#include "XAttributes.h"
#include "XPlayer.h"

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

XAttributes* XEntityMgr::InitAttr(int staticid)
{
	/*XEntityStatisticsRow* srow;
	iGetXEntityStatisticsRow(staticid, srow);
	XEntityPresentationRow* prow;
	iGetXEntityPresentationRow(srow->presentid, prow);*/
	XAttributes* attr = new XAttributes();
	attr->setid(new_id());
	//attr->setPresentID(srow->presentid);
	//attr->setPrefab(prow->prefab);
	//attr->setName(prow->name);
	return attr;
}

XEntity* XEntityMgr::PrepareEntity(XAttributes* attr)
{
	XEntity* x = new XEntity();
	GameObject* o = new GameObject();// XResources.LoadInPool("Prefabs/" + attr.Prefab);
	o->name = tostring(attr->getid()).c_str();
	o->transform->position = attr->getAppearPostion();
	o->transform->rotatiion = attr->getAppearQuaternion();
	x->Initilize(o, attr);
	uint id = attr->getid();
	if (!x->IsPlayer()) _dic_entities.insert(std::make_pair(id, x));
	return x;
}

XEntity* XEntityMgr::CreateEntity(uint staticid, Vector3 pos, Vector3 rot)
{
	XAttributes* attr = InitAttr((int)staticid);
	attr->setAppearPosition(pos);
	attr->setAppearQuaternion(rot);
	XEntity* ent = PrepareEntity(attr);
	Add(Entity, ent);
	return ent;
}

bool XEntityMgr::Add(EntityType type, XEntity* e)
{
	std::vector<XEntity*> vec = _map_entities[(int)type];
	bool exit = false;
	for (size_t i = 0; i < vec.size(); i++)
	{
		if (vec[i] == e) exit = true;
	}
	if (exit) vec.push_back(e);
	return !exit;
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
	if (Player && id == Player->EntityID)
	{
		return Player;
	}
	else
	{
		return _dic_entities[id];
	}
}


XPlayer* XEntityMgr::CreatePlayer()
{
	int staticid = 2;
	XAttributes* attr = InitAttr(staticid);
	attr->setAppearPosition(Vector3::zero);
	attr->setAppearQuaternion(Vector3::zero);
	return Player = dynamic_cast<XPlayer*>(PrepareEntity(attr));
}

