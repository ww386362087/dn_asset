#include "XEntity.h"
#include "XBeHitComponent.h"
#include "XAIComponent.h"
#include "XSkillComponent.h"
#include "Transform.h"
#include "XAttributes.h"
#include "XStateMachine.h"

EntyCallBack eCallback;
CompCallBack cCallback;
EntySyncCallBack sncCallback;

XEntity::~XEntity()
{
#ifdef Client
	eCallback(this->EntityID, 'U', 0);
#endif

	Unload();
	OnUnintial();
	delete _pmachine;
	_attr = NULL;
	_object = NULL;
	_transf = NULL;
	_pmachine = NULL;
}

void XEntity::Update(float delta)
{
	for (std::unordered_map<uint, XComponent*>::iterator it = components.begin(); it != components.end(); it++)
	{
		it->second->Update(delta);
	}
	if (_force_move && _transf)
	{
		_transf->position = _transf->position + _forward * _speed;
#ifdef  Client
		sncCallback(_attr->getid(), 'p', vec2arr(_transf->position));
#endif //  Client
	}
}


void XEntity::AttachToHost()
{
}

void XEntity::DetachFromHost()
{
}

void XEntity::DetachAllComponents()
{
	std::unordered_map<uint, XComponent*>::iterator itr;
	for (itr = components.begin(); itr != components.end(); itr++)
	{
		itr->second->OnUninit();
	}
	components.clear();
}

void XEntity::Initilize(GameObject* go, XAttributes* attr)
{
	XObject::Initilize();
	_object = go;
	_transf = go->transform;
	_attr = attr;

#ifdef Client
	if (GetType() == Player)
	{
		eCallback(attr->getid(), 'P', attr->getPresentID());
	}
	else if (GetType() == Role)
	{
		eCallback(attr->getid(), 'R', attr->getPresentID());
	}
	else if (GetType() == Monster)
	{
		eCallback(attr->getid(), 'M', attr->getPresentID());
	}
	else if (GetType() == Npc)
	{
		eCallback(attr->getid(), 'N', attr->getPresentID());
	}
	else
	{
		eCallback(attr->getid(), 'E', attr->getPresentID());
	}
	sncCallback(attr->getid(), 'p', vec2arr(attr->getAppearPostion()));
	sncCallback(attr->getid(), 'r', vec2arr(attr->getAppearQuaternion()));
	float scale[] = { 3.0f,3.0f,3.0f };
	sncCallback(attr->getid(), 's', scale);
#endif // -- Client
	_transf->position = attr->getAppearPostion();
	_transf->rotation = attr->getAppearQuaternion();
	AttachComponent<XAIComponent>();
	OnInitial();
}

void XEntity::OnInitial() {}

void XEntity::OnUnintial() {}

bool XEntity::IsBoss()
{
	return (GetType() & Boss) != 0;
}

bool XEntity::IsNpc()
{
	return (GetType() & Npc) != 0;
}

bool XEntity::IsRole()
{
	return (GetType() & Role) || (GetType() & Player) != 0;
}

bool XEntity::IsPlayer()
{
	return (GetType() & Player) != 0;
}

void XEntity::MoveForward(Vector3 forward)
{
	_forward = forward;
	_force_move = true;
}

void XEntity::StopMove()
{
	_force_move = false;
	_forward = Vector3::zero;
}

void XEntity::OnDied()
{
	_state = XState_Death;
}

bool XEntity::IsDead()
{
	return _state == XState_Death;
}

EntityType XEntity::GetType()
{
	return Entity;
}

XSkillMgr* XEntity::SkillManager()
{
	XSkillComponent* pskill = GetComponent<XSkillComponent>();
	return pskill ? pskill->SkillManager() : NULL;
}

bool XEntity::Valide(XEntity* e)
{
	return e && e->_attr && !e->IsDead();
}

XEntity* XEntity::ValidEntity(uint id)
{
	if (id == 0) return NULL;
	return XEntityMgr::Instance()->GetEntity(id);
}

Vector3 XEntity::getPostion()
{
	if (_transf)
	{
		return _transf->position;
	}
	else
	{
		return Vector3::zero;
	}
}

GameObject* XEntity::getEntityObject()
{
	return _object;
}

Transform* XEntity::getTransfer()
{
	return _transf;
}

Vector3 XEntity::getForward()
{
	return _transf->forward;
}

XAttributes* XEntity::getAttributes()
{
	return _attr;
}

void XEntity::setAttributes(XAttributes* attr)
{
	_attr = attr;
}

XStateDefine XEntity::getState()
{
	return _state;
}