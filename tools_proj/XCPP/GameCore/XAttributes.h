#ifndef  __XAttributes__
#define  __XAttributes__

#include "Common.h"
#include "XAttributeDefine.h"
#include "Vector3.h"

class XAttributes
{
public:
	XAttributes();
	~XAttributes();
	uint getid();
	void setid(uint val);
	Vector3 getAppearPostion();
	void setAppearPosition(Vector3 v);
	Vector3 getAppearQuaternion();
	void setAppearQuaternion(Vector3 rot);
	uint getPresentID();
	void setPresentID(uint val);
	bool getDead();
	void setDead(bool val);
	const char* getName();
	void setName(const char* val);
	const char* getPrefab();
	void setPrefab(const char* val);
	int getFightGroup();
	void setFightGroup(int val);
	bool getBlocked();
	void setBlocked(bool val);
	const char* getAiBehaviour();
	void setAIBehaviour(const char* ai);
	double GetAttr(XAttributeDefine def);
	uint getTypeID();

	int AiHit;
	int FightGroup;
	bool Blocked;
	bool IsFixedInCD;
	bool Outline;
	int SummonGroup;
	bool EndShow;
	bool GeneralCutScene;
	float NormalAttackProb;
	float EnterFightRange;
	float FloatingMax;
	float FloatingMin;
	float AIStartTime;
	float AIActionGap;
	const char* AiBehavior;
	float FightTogetherDis;

private:
	uint _id = 0;
	Vector3 _pos;
	Vector3 _rot;
	bool _dead;
	int _group;
	bool _block;
	const char* _prefab_name ;
	const char* _name ;
	Vector3 _appear_pos;
	uint _presentID = 2;
	bool _is_dead = false;
	uint _type_id;
};

#endif