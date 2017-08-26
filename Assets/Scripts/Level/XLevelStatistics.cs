using System.Collections.Generic;
using UnityEngine;

namespace Level
{

    public class XLevelState
    {
        public uint _current_scene_id;
        public int _total_monster = 0;
        public int _total_kill = 0;
        public int _before_force_kill = 0;
        public int _after_force_kill = 0;
        public Dictionary<ulong, int> _entity_in_level_spawn = new Dictionary<ulong, int>();
        public Dictionary<ulong, int> _entity_die = new Dictionary<ulong, int>();
        public int _boss_total = 0;
        public int _boss_kill = 0;
        public int _remain_monster = 0;
        public int _boss_rush_kill = 0;

        public int _abnormal_monster = 0;
        public int _boss_exist_time = 0;
        public int _monster_exist_time = 0;

        public int _BossWave = 0;
        public Vector3 _lastDieEntityPos;
        public float _lastDieEntityHeight = 0.0f;
        public bool _refuseRevive = false;
        public int _player_continue_index = 0;

        public uint _my_team_alive = 0;
        public uint _op_team_alive = 0;
        public uint _revive_count = 0;
        public uint _death_count = 0;

        public uint _max_combo;
        public uint _player_behit;
        public float _start_time;
        public float _end_time;
        public bool _key_npc_die;
        public uint _enemy_in_fight;

        public int _box_enemy_kill = 0;
        public float _total_damage = 0;
        public float _total_hurt = 0;
        public float _total_heal = 0;
        public List<uint> _monster_refresh_time = new List<uint>();

        public void AddEntityDieCount(ulong entityID)
        {
        }

        public void AddLevelSpawnEntityCount(ulong entityID)
        {
            if (!_entity_in_level_spawn.ContainsKey(entityID))
            {
                _entity_in_level_spawn.Add(entityID, 1);
            }
            else
            {
                _entity_in_level_spawn[entityID] += 1;
            }
        }

        public bool CheckEntityInLevelSpawn(ulong entityID)
        {
            int count = 0;
            if (_entity_in_level_spawn.TryGetValue(entityID, out count))
            {
                return count > 0;
            }

            return false;
        }

        public void Reset()
        {
            _current_scene_id = 0;
            _total_monster = 0;
            _total_kill = 0;
            _remain_monster = 0;
            _before_force_kill = 0;
            _after_force_kill = 0;
            _entity_in_level_spawn.Clear();
            _entity_die.Clear();
            _boss_kill = 0;
            _boss_total = 0;
            _boss_rush_kill = 0;
            _BossWave = 0;
            _lastDieEntityPos = Vector3.zero;
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

            _start_time = 0;
            _end_time = 0;

            _box_enemy_kill = 0;
            _key_npc_die = false;
            _enemy_in_fight = 0;

            _total_damage = 0;
            _total_hurt = 0;
            _total_heal = 0;

            _monster_refresh_time.Clear();

        }
    }


    public class XLevelStatistics : XSingleton<XLevelStatistics>
    {
        public XLevelState ls = new XLevelState();


        public void AddEntityDieCount(ulong entityID)
        {
        }

        public void AddLevelSpawnEntityCount(ulong entityID)
        {
        }


    }


}
