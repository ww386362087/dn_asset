#ifndef  __XAttributes__
#define  __XAttributes__

#include "Common.h"
class Vector3;

class XAttributes
{
public:
	uint getid();
	void setid(uint val);
	Vector3* getAppearPostion();
	void setAppearPosition(Vector3* v);
	Vector3* getAppearQuaternion();
	void setAppearQuaternion(Vector3* rot);
	uint getPresentID();
	void setPresentID(uint val);
	bool getDead();
	void setDead(bool val);
	int getFightGroup();
	void setFightGroup(int val);
	bool getBlocked();
	void setBlocked(bool val);

	int AiHit;

private:
	uint _id = 0;
	Vector3* _pos;
	Vector3* _rot;
	bool _dead;
	int _group;
	bool _block;
	const char* _prefab_name ;
	const char* _name ;
	Vector3* _appear_pos;
	uint _presentID = 2;
	bool _is_dead = false;
	uint _type_id;
};

#endif