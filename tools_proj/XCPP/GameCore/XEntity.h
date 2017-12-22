#ifndef  __XEntity__
#define  __XEntity__

#include "XObject.h"
#include "XEntityMgr.h"
#include "GameObject.h"
#include "Transform.h"
#include "Vector3.h"
#include "XStateDefine.h"

class  XAttributes;

class XEntity:public XObject
{
public:
	virtual void Update(float delta);
	virtual void LateUpdate();
	virtual void AttachToHost();
	virtual void DetachFromHost();
	virtual void OnInitial();
	virtual void OnUnintial();

	void Initilize(GameObject* go, XAttributes* attr);
	bool IsPlayer();
	bool IsRole();
	bool IsBoss();
	bool IsNpc();
	bool IsDead();
	EntityType GetType();

	void MoveForward(Vector3 forward);
	void StopMove();
	void OnDied();
	void UnloadEntity();
	uint EntityID;
	static bool Valide(XEntity* e);
	static XEntity* ValidEntity(uint id);

public:
	Vector3* getPostion();
	GameObject* getEntityObject();
	XAttributes* getAttributes();

protected:
	EntityType _eEntity_Type = Entity;
	GameObject* _object;
	Transform* _transf;
	int _layer = 0;
	float _speed = 0.03f;
	bool _force_move = false;
	bool _freeze = false;
	Vector3 _forward = Vector3::zero;
	XAttributes* _attr;
	XStateDefine _state = XState_Idle;

};

#endif