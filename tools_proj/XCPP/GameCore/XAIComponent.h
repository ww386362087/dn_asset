#ifndef  __XAIComponent__
#define  __XAIComponent__

#include "XObject.h"
#include "AITree.h"
#include "XDelegate.h"
#include "XEventArgs.h"

class  XEntity;


class XAIComponent :public XComponent
{
public:
	XAIComponent();
	~XAIComponent();
	virtual void OnInitial(XObject* _obj);
	virtual void OnUninit();
	virtual void EventSubscribe();
	virtual void OnUpdate(float delta);
	void SetTarget(XEntity* target);
	void InitTree();
	void SetBehaviorTree(const char* tree);
	void InitVariables();
	void OnTickAI();
	bool OnStartSkill(IArgs*, void*);
	bool OnEndSkill(IArgs*, void*);

public:
	bool getIsCastingSkill(); 
	bool getIsOppoCastingSkill(); 
	bool getIsFixedInCd();
	bool getIsHurtOppo();
	float getEnterFightRange();

private:
	void UpdateVariable();
	void SetTreeVariable(AITree* tree);

private:
	bool _is_inited = false;
	AITree* _tree;
	float _ai_tick = 1.0f;  //AI心跳间隔 
	float _tick_factor = 1.0f;
	uint _cast_skillid = 0;
	float _tick = 0;
	float _timer = 0;
	XEntity* _entity;

	// 行为树相关的变量
	XEntity* _target;
	bool _is_oppo_casting_skill = false;
	bool _is_hurt_oppo = false;
	float _target_distance = 0.0f;
	float _master_distance = 9999.0f;
	bool _is_fixed_in_cd = false;
	float _normal_attack_prob = 0.5f;
	float _enter_fight_range = 10.0f;
	float _fight_together_dis = 10.0f;
	float _max_hp = 1000.0f;
	float _current_hp = 0.0f;
	float _max_super_armor = 100.0f;
	float _current_super_armor = 50.0f;
	float _target_rotation = 0.0f;
	float _attack_range = 1.0f;
	float _min_keep_range = 1.0f;
	bool _is_casting_skill = false;
	bool _is_fighting = false;
	bool _is_qte_state = false;
};

#endif