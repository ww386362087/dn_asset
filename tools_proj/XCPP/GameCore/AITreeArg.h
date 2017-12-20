#ifndef  __AITreeArg__
#define  __AITreeArg__

#include<string>


class AITreeArg
{
public:
	static const char* HeartRate;
	static const char* NavTarget;
	static const char* TARGET;
	static const char* MASTER;
	static const char* IsOppoCastingSkill;
	static const char* IsHurtOppo;
	static const char* TargetDistance;
	static const char* MasterDistance;
	static const char* IsFixedInCD;
	static const char* NormalAttackProb;
	static const char* EnterFightRange;
	static const char* FightTogetherDis;
	static const char* MaxHP;
	static const char* CurrHP;
	static const char* MaxSupperArmor;
	static const char* CurrSuperArmor;
	static const char* EntityType;
	static const char* TargetRot;
	static const char* AttackRange;
	static const char* MinKeepRange;
	static const char* IsCastingSkill;
	static const char* IsFighting;
	static const char* IsQteState;
	static const char* BornPos;
	static const char* SkillID;
};


const char* AITreeArg::HeartRate = "heartrate";
const char* AITreeArg::NavTarget = "navtarget";
const char* AITreeArg::TARGET = "target";
const char* AITreeArg::MASTER = "master";
const char* AITreeArg::IsOppoCastingSkill = "is_oppo_casting_skill";
const char* AITreeArg::IsHurtOppo = "is_hurt_oppo";
const char* AITreeArg::TargetDistance = "target_distance";
const char* AITreeArg::MasterDistance = "master_distance";
const char* AITreeArg::IsFixedInCD = "is_fixed_in_cd";
const char* AITreeArg::NormalAttackProb = "normal_attack_prob";
const char* AITreeArg::EnterFightRange = "enter_fight_range";
const char* AITreeArg::FightTogetherDis = "fight_together_dis";
const char* AITreeArg::MaxHP = "max_hp";
const char* AITreeArg::CurrHP = "current_hp";
const char* AITreeArg::MaxSupperArmor = "max_super_armor";
const char* AITreeArg::CurrSuperArmor = "current_super_armor";
const char* AITreeArg::EntityType = "entity_type";
const char* AITreeArg::TargetRot = "target_rotation";
const char* AITreeArg::AttackRange = "attack_range";
const char* AITreeArg::MinKeepRange = "min_keep_range";
const char* AITreeArg::IsCastingSkill = "is_casting_skill";
const char* AITreeArg::IsFighting = "is_fighting";
const char* AITreeArg::IsQteState = "is_qte_state";
const char* AITreeArg::BornPos = "bornpos";
const char* AITreeArg::SkillID = "skillid";

#endif
