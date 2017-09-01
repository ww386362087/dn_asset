using System.Collections.Generic;

namespace Level
{

    internal enum LevelCmd
    {
        Level_Cmd_Invalid,
        Level_Cmd_TalkL,
        Level_Cmd_TalkR,
        Level_Cmd_Notalk,
        Level_Cmd_Addbuff,
        Level_Cmd_Tutorial,
        Level_Cmd_Notice,
        Level_Cmd_StopNotice,
        Level_Cmd_Opendoor,
        Level_Cmd_Cutscene,
        Level_Cmd_LevelupFx,
        Level_Cmd_ShowSkill,
        Level_Cmd_KillSpawn,
        Level_Cmd_KillAlly,
        Level_Cmd_KillWave,
        Level_Cmd_Direction,
        Level_Cmd_Outline,
        Level_Cmd_Record,
        Level_Cmd_Continue,
        Level_Cmd_NewbieHelper,
        Level_Cmd_NewbieNotice,
        Level_Cmd_Removebuff,
        Level_Cmd_Summon,
        Level_Cmd_KillAllSpawn,
        Level_Cmd_NpcPopSpeek,
        Level_Cmd_SendAICmd,
        Level_Cmd_Bubble,
        Level_Cmd_HideBillboard,
        Level_Cmd_ChangeBody,
        Level_Cmd_JustFx,
        Level_Cmd_PlayFx,
    }

    internal enum XCmdState
    {
        Cmd_In_Queue,
        Cmd_In_Process,
        Cmd_Finished
    }

    internal class LevelCmdDesc
    {
        public LevelCmd cmd = LevelCmd.Level_Cmd_Invalid;
        public List<string> Param = new List<string>();

        public XCmdState state = XCmdState.Cmd_In_Queue;

        public void Reset()
        {
            state = XCmdState.Cmd_In_Queue;
        }
    }


    class XLevelInfo
    {
        public XLevelInfo()
        {
            infoName = "";
            x = y = z = face = width = height = thickness = 0;
            enable = false;
        }

        public string infoName;
        public float x, y, z, face, width, height, thickness;
        public bool enable;
    }

}