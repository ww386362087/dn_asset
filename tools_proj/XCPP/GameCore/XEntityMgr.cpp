#include "XEntityMgr.h"
#include "XEntity.h"
#include "XAttributes.h"
#include "XPlayer.h"
#include "XNpc.h"
#include "XEntityStatistics.h"
#include "XEntityPresentation.h"
#include "XScene.h"

XEntityMgr::~XEntityMgr()
{
}

void XEntityMgr::Update(float delta)
{
	for (std::unordered_set<XEntity*>::iterator itr = _hash_entities.begin(); itr != _hash_entities.end(); itr++)
	{
		(*itr)->Update(delta);
	}
	if (Player)
	{
		Player->Update(delta);
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

 void XEntityMgr::InitAttr(int staticid, XAttributes* attr)
{
	XEntityStatisticsRow srow;
	iGetXEntityStatisticsRowByID(staticid, &srow);
	XEntityPresentationRow prow;
	iGetXEntityPresentationRowByID((uint)srow.presentid, &prow);
	attr->setid(new_id());
	attr->setPresentID(srow.presentid);
	attr->setPrefab(prow.prefab);
	attr->setName(prow.name);
	attr->setAIBehaviour(srow.aibehavior);
}


 std::vector<XEntity*> XEntityMgr::GetAllEnemy(XEntity* e)
 {
	 EntityType type = (EntityType)(1 << (e->getAttributes()->FightGroup + Ship_Start));
	 if (type == Ally) return _map_entities[Enemy];
	 else if (type == Enemy) return _map_entities[Ally];
	 return _empty;
 }


 std::vector<XEntity*> XEntityMgr::GetAllAlly(XEntity* e)
 {
	 EntityType type = (EntityType)(1 << (e->getAttributes()->FightGroup + Ship_Start));
	 if (type == Ally) return _map_entities[Enemy];
	 else if (type == Enemy) return _map_entities[Ally];
	 return _empty;
 }

 std::vector<XEntity*> XEntityMgr::GetAllNPC()
 {
	 return _map_entities[Npc];
 }

void XEntityMgr::PrepareEntity(XAttributes* attr, XEntity* ent)
{
	ent->setAttributes(attr);
	std::string str = "Prefabs/" + tostring(attr->getPrefab());
	GameObject* o = GameObjectMgr::Instance()->Create(str.c_str());
	o->name = tostring(attr->getid()).c_str();
	o->transform->position = attr->getAppearPostion();
	Vector3 rot = attr->getAppearQuaternion();
	o->transform->rotation = rot;

	ent->Initilize(o, attr);
	ent->EntityID = attr->getid();
	if (!ent->IsPlayer())
		_dic_entities.insert(std::make_pair(id, ent));
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
	_hash_entities.insert(e);
	return !exit;
}

void XEntityMgr::UnloadEntity(uint id)
{
	std::unordered_map<uint, XEntity*>::iterator itr = _dic_entities.find(id);
	if (itr != _dic_entities.end())
	{
		_dic_entities.erase(id);
		_hash_entities.erase(itr->second);
		delete itr->second->getAttributes();
		delete itr->second;
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
		delete (*itr)->getAttributes();
		delete *itr;
	}
	_hash_entities.clear();
	_map_entities.clear();
	_dic_entities.clear();

	if (Player)
	{
		delete Player->getAttributes();
		delete Player;
	} 
	Player = NULL;
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
	XAttributes* attr = new XAttributes();
	InitAttr(staticid, attr);
	SceneListRow* row = XScene::Instance()->getSceneRow();
	if (row)
	{
		std::vector<std::string> s = split(row->startpos, '=');
		float x = (float)atof(s[0].c_str());
		float y = (float)atof(s[1].c_str());
		float z = (float)atof(s[2].c_str());
		attr->setAppearPosition(Vector3(x, y, z));
		attr->setAppearQuaternion(Vector3(row->startrot[0], row->startrot[1], row->startrot[2]));
	}
	Player = new XPlayer();
	PrepareEntity(attr, Player);
	return Player;
}


XEntity* XEntityMgr::CreateNPC(XNpcListRow& row, Vector3 pos, Vector3 rot)
{
	XAttributes* attr = InitAttrByPresent(row.presentid);
	attr->setAppearPosition(pos);
	attr->setAppearQuaternion(rot);
	XEntity* enty = new XNpc();
	PrepareEntity(attr, enty);
	Add(Npc, enty);
	return enty;
}

XAttributes* XEntityMgr::InitAttrByPresent(uint presentID)
{
	XAttributes* attr = new XAttributes();
	XEntityPresentationRow row;
	iGetXEntityPresentationRowByID(presentID, &row);
	attr->setPresentID(presentID);
	attr->setPrefab(row.prefab);
	attr->setid(new_id());
	attr->setName(row.name);
	return attr;
}