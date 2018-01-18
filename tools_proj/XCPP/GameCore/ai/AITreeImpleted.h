#ifndef  __AITreeImpleted__
#define  __AITreeImpleted__

#include "../Vector3.h"
#include "AIBehaviour.h"
#include "../Common.h"
#include "../XAttributeDefine.h"
#include <string>

class  XEntity;
class Transform;
class GameObject;

class AITreeImpleted
{
public:

	static XEntity* Transform2Entity(Transform* t);
	static AIStatus NavToTargetUpdate(XEntity* entity, GameObject* mAIArgTarget, GameObject* mAIArgNavTarget, Vector3 mAIArgNavPos);
	static AIStatus StopNavMoveUpdate(XEntity* entity);
	static AIStatus RotateToTargetUpdate(XEntity* entity, GameObject* mAIArgTarget, float ang);
	static AIStatus RotateSelfUpdate(XEntity* entity, float max, float min);
	static AIStatus MoveForwardUpdate(XEntity* entity);
	static AIStatus DetectEnemyInSightUpdate(XEntity* entity);
	static AIStatus FindTargetByDistanceUpdate(XEntity* entity, float mAIArgDistance, float mAIArgAngle);
	static AIStatus ResetTargetUpdate(XEntity* entity);
	static AIStatus DoSelectNearestUpdate(XEntity* entity);
	static AIStatus DoSelectFarthestUpdate(XEntity* entity);
	static AIStatus DoSelectRandomTargetUpdate(XEntity* entity);
	static AIStatus CalDistanceUpdate(XEntity* entity, Transform* mAIArgObject, float mAIArgDistance, Vector3 mAIArgDestPoint);
	static AIStatus SelectMoveTargetByIdUpdate(XEntity* entity, Transform* mAIArgMoveTarget, int mAIArgObjectId);
	static AIStatus ValueHPUpdate(XEntity* entity, int mAIArgMaxHP, int mAIArgMinHP);
	static AIStatus ValueMPUpdate(XEntity* entity, int mAIArgMaxMP, int mAIArgMinMP);
	static AIStatus ValueTargetUpdate(XEntity* entity, Transform* mAIArgTarget);
	static AIStatus ValueDistanceUpdate(XEntity* entity, GameObject* mAIArgTarget, float mAIArgMaxDistance);
	static AIStatus StatusRandomUpdate(XEntity* e, int prob);
	static AIStatus IsOppoCastingSkillUpdate(XEntity* entity);
	static AIStatus IsHurtOppoUpdate(XEntity* entity);
	static AIStatus IsFixedInCdUpdate(XEntity* entity);
	static AIStatus IsCastingSkillUpdate(XEntity* entity);
	static AIStatus StatusIdleUpdate(XEntity* entity);
	static AIStatus StatusMoveUpdate(XEntity* entity);
	static AIStatus StatusBehitUpdate(XEntity* entity);
	static AIStatus StatusDeathUpdate(XEntity* entity);
	static AIStatus StatusFreezeUpdate(XEntity* entity);
	static AIStatus StatusSkillUpdate(XEntity* entity);
	static AIStatus LogUpdate(XEntity* entity, std::string str, bool b);
	static AIStatus WaitUpdate(XEntity* entity, float wait, bool rand, float min, float max);
	static AIStatus XAIActionSkillUpdate(XEntity* entity, std::string scr, GameObject* target);
	static AIStatus RandomCompareUpdate(XEntity* entity, int prob);
	static Transform* SelectMoveTargetById(XEntity* entity, int objectid);
	static bool ActionNav(XEntity* entity, Vector3 dest);
	static bool NavToTarget(XEntity* entity, GameObject* target);
	static bool StopNavMove(XEntity* entity);
	static bool RotateToTarget(Transform* src, GameObject* target, float ang);
	static bool DetectEnemyInSight(XEntity* e);


};

#endif