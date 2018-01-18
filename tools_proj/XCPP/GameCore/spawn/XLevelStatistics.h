#ifndef  __XLevelStatistics__
#define  __XLevelStatistics__

#include "../Singleton.h"
#include "../Common.h"
#include <vector>
#include <unordered_map>

class XLevelState
{
public :
	uint _current_scene_id;
	int _total_monster = 0;
	int _total_kill = 0;
	int _before_force_kill = 0;
	int _after_force_kill = 0;
	std::unordered_map<ulong, int> _entity_in_level_spawn;
	std::unordered_map<ulong, int> _entity_die;
	int _boss_total = 0;
	int _boss_kill = 0;
	int _remain_monster = 0;
	int _boss_rush_kill = 0;

	int _abnormal_monster = 0;
	int _boss_exist_time = 0;
	int _monster_exist_time = 0;

	int _BossWave = 0;
	Vector3 _lastDieEntityPos;
	float _lastDieEntityHeight = 0.0f;
	bool _refuseRevive = false;
	int _player_continue_index = 0;

	uint _my_team_alive = 0;
	uint _op_team_alive = 0;
	uint _revive_count = 0;
	uint _death_count = 0;

	uint _max_combo;
	uint _player_behit;
	float _end_time;
	bool _key_npc_die;
	uint _enemy_in_fight;

	int _box_enemy_kill = 0;
	float _total_damage = 0;
	float _total_hurt = 0;
	float _total_heal = 0;

	void AddEntityDieCount(ulong entityID) {}

	void AddLevelSpawnEntityCount(ulong entityID)
	{
		if (_entity_in_level_spawn.find(entityID) == _entity_in_level_spawn.end())
		{
			_entity_in_level_spawn.insert(std::make_pair(entityID, 1));
		}
		else
		{
			_entity_in_level_spawn[entityID] += 1;
		}
	}

	bool CheckEntityInLevelSpawn(ulong entityID)
	{
		int count = 0;
		if (_entity_in_level_spawn.find(entityID) != _entity_in_level_spawn.end())
		{
			return _entity_in_level_spawn[entityID] > 0;
		}
		return false;
	}

	void Reset()
	{
		_current_scene_id = 0;
		_total_monster = 0;
		_total_kill = 0;
		_remain_monster = 0;
		_before_force_kill = 0;
		_after_force_kill = 0;
		_entity_in_level_spawn.clear();
		_entity_die.clear();
		_boss_kill = 0;
		_boss_total = 0;
		_boss_rush_kill = 0;
		_BossWave = 0;
		_lastDieEntityPos = Vector3::zero;
		_lastDieEntityHeight = 0.0f;
		_refuseRevive = false;
		_revive_count = 0;
		_player_continue_index = 0;

		_abnormal_monster = 0;
		_boss_exist_time = 0;
		_monster_exist_time = 0;

		_my_team_alive = 1;     // 应该是当前小队的人数
		_op_team_alive = 0;
		_max_combo = 0;
		_player_behit = 0;

		_end_time = 0;

		_box_enemy_kill = 0;
		_key_npc_die = false;
		_enemy_in_fight = 0;

		_total_damage = 0;
		_total_hurt = 0;
		_total_heal = 0;


	}
};



class XLevelStatistics :public Singleton<XLevelStatistics>
{
public:
	XLevelStatistics()
	{
		ls = new XLevelState();
	}

	~XLevelStatistics()
	{
		delete ls;
		ls = NULL;
	}

	XLevelState* ls;
};

#endif