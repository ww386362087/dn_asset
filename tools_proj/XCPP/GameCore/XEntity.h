#ifndef  __XEntity__
#define  __XEntity__

#include "XObject.h"
#include "XEntityMgr.h"
#include "GameObject.h"
#include "Transform.h"
#include "Vector3.h"
#include "XStateDefine.h"

class XAttributes;
class XSkillMgr;
class XStateMachine;

class XEntity:public XObject
{
public:
	~XEntity();
	virtual void Update(float delta);
	virtual void AttachToHost();
	virtual void DetachFromHost();
	virtual void OnInitial();
	virtual void OnUnintial();

	void DetachAllComponents();
	template<class T> T* AttachComponent();
	template<class T> T* GetComponent();
	template<class T> bool DetachComponent();

	void Initilize(GameObject* go, XAttributes* attr);
	bool IsPlayer();
	bool IsRole();
	bool IsBoss();
	bool IsNpc();
	bool IsDead();
	virtual EntityType GetType();
	XSkillMgr* SkillManager();

	inline XStateMachine* StateMachine() const { return _pmachine; }

	void MoveForward(Vector3 forward);
	void StopMove();
	void OnDied();
	uint EntityID;
	static bool Valide(XEntity* e);
	static XEntity* ValidEntity(uint id);

public:
	Vector3 getPostion();
	GameObject* getEntityObject();
	Transform* getTransfer();
	Vector3 getForward();
	XAttributes* getAttributes();
	void setAttributes(XAttributes*);
	XStateDefine getState();
	int Wave;
	float CreateTime;

protected:
	XStateMachine* _pmachine;
	GameObject* _object;
	Transform* _transf;
	int _layer = 0;
	float _speed = 0.03f;
	bool _force_move = false;
	bool _freeze = false;
	Vector3 _forward = Vector3::zero;
	XAttributes* _attr;
	XStateDefine _state = XState_Idle;

	std::unordered_map<uint, XComponent*> components;

};


template<class T> T* XEntity::AttachComponent()
{
	std::string name = typeid(T).name();
	uint uid = xhash(name.c_str());
	std::unordered_map<uint, XComponent*>::iterator  itr = components.find(uid);
	if (itr != components.end())
	{
		return dynamic_cast<T*>(components[uid]);
	}
	else
	{
		T* t = new T();
		t->OnInitial(this);
		components.insert(std::make_pair(uid, t));
		return t;
	}
}

template<class T> T* XEntity::GetComponent()
{
	std::string name = typeid(T).name();
	uint uid = xhash(name.c_str());
	return dynamic_cast<T*>(components[uid]);
}

template<class T> bool XEntity::DetachComponent()
{
	std::string name = typeid(T).name();
	uint uid = xhash(name.c_str());
	std::unordered_map<uint, XComponent*>::iterator  itr = components.find(uid);
	if (itr != components.end())
	{
		itr->second->OnUninit();
		components.erase(uid);
		return true;
	}
	return false;
}


#endif