#ifndef _LevelSpawnType_
#define _LevelSpawnType_


enum LevelSpawnType
{
	Spawn_Monster,
	Spawn_Role,
	Spawn_NPC,
	Spawn_Buff,
};


enum LevelInfoType
{
	TypeNone,
	TypeId,
	BaseInfo,
	PreWave,
	EditorInfo,
	TransformInfo,
	Script,
	ExString,
	SpawnType,
};

#endif