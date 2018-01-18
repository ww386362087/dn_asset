#ifndef  __LevelCmd__
#define  __LevelCmd__

#include <string>
#include <vector>

enum LevelCmd
{
	Level_Cmd_Invalid,
	Level_Cmd_TalkL,
	Level_Cmd_TalkR,
	Level_Cmd_Notalk,
	Level_Cmd_Addbuff,
	Level_Cmd_Opendoor,
	Level_Cmd_Cutscene,
	Level_Cmd_ShowSkill,
	Level_Cmd_KillSpawn,
	Level_Cmd_KillWave,
	Level_Cmd_Direction,
	Level_Cmd_Continue,
	Level_Cmd_Removebuff,
	Level_Cmd_KillAllSpawn,
	Level_Cmd_NpcPopSpeek,
	Level_Cmd_SendAICmd,
	Level_Cmd_HideBillboard,
	Level_Cmd_PlayFx,
}; 


enum XCmdState
{
	Cmd_In_Queue,
	Cmd_In_Process,
	Cmd_Finished
};

class LevelCmdDesc
{
public:
	LevelCmd cmd = Level_Cmd_Invalid;
	std::vector<std::string> Param;
	XCmdState state = Cmd_In_Queue;

	void Reset()
	{
		state = Cmd_In_Queue;
	}
};


class XLevelInfo
{
public:
	std::string infoName;
	float x, y, z, face, width, height, thickness;
	bool enable;
};

#endif