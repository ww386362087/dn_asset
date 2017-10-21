using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace Level
{

    class XLevelScriptMgr : XSingleton<XLevelScriptMgr>
    {
        public uint CommandCount = 0;
        List<LevelCmdDesc> _CmdQueue = new List<LevelCmdDesc>();
        LevelCmdDesc _currentCmd;
        public List<string> _externalString = new List<string>();
        public List<string> _onceString = new List<string>();
        Dictionary<string, List<LevelCmdDesc>> _LevelScripts = new Dictionary<string, List<LevelCmdDesc>>();
        List<XLevelInfo> _LevelInfos = new List<XLevelInfo>();

        public void RunScript(string funcName)
        {
            //  XDebug.Log("add script " + funcName);
            if (!_LevelScripts.ContainsKey(funcName)) return;
            if (_CmdQueue != null && _CmdQueue.Count > 0)
            {
                 XDebug.Log("script function append");
            }
            if (_CmdQueue.Count == 0) _currentCmd = null;
            List<LevelCmdDesc> funcCmds = _LevelScripts[funcName];
            for (int i = 0; i < funcCmds.Count; i++)
            {
                funcCmds[i].Reset();
                _CmdQueue.Add(funcCmds[i]);
            }
            Update();
        }

        public bool IsCurrentCmdFinished()
        {
            if (_currentCmd == null) return true;
            if (_currentCmd.state == XCmdState.Cmd_Finished) return true;
            return false;
        }

        public void ClearWallInfo()
        {
            _LevelInfos.Clear();
        }

        public void PreloadLevelScript(string file)
        {
            Reset();
            ClearWallInfo();
            Stream s = XResources.ReadText("Table/" + file);
            {
                using (StreamReader sr = new StreamReader(s))
                {
                    string line;
                    string curFunc = "";
                    while (true)
                    {
                        line = sr.ReadLine();
                        if (line == null) break;
                        line = line.Trim();
                        if (line.StartsWith("func:"))
                        {
                            string func = line.Substring(5);
                            List<LevelCmdDesc> subQueue = new List<LevelCmdDesc>();
                            _LevelScripts.Add(func, subQueue);
                            curFunc = func;
                        }
                        if (line.StartsWith("talkl"))
                        {
                            string[] str = line.Split(XGlobalConfig.TabSeparator);
                            LevelCmdDesc cmd = new LevelCmdDesc();
                            cmd.cmd = LevelCmd.Level_Cmd_TalkL;
                            cmd.Param.Add(str[1]);
                            cmd.Param.Add(str[2]);
                            if (str.Length > 3)
                                cmd.Param.Add(str[3]);
                            _LevelScripts[curFunc].Add(cmd);
                        }
                        else if (line.StartsWith("talkr"))
                        {
                            string[] str = line.Split(XGlobalConfig.TabSeparator);
                            LevelCmdDesc cmd = new LevelCmdDesc();
                            cmd.cmd = LevelCmd.Level_Cmd_TalkR;
                            cmd.Param.Add(str[1]);
                            cmd.Param.Add(str[2]);
                            if (str.Length > 3)
                                cmd.Param.Add(str[3]);
                            _LevelScripts[curFunc].Add(cmd);
                        }
                        else if (line.StartsWith("stoptalk"))
                        {
                            LevelCmdDesc cmd = new LevelCmdDesc();
                            cmd.cmd = LevelCmd.Level_Cmd_Notalk;
                            _LevelScripts[curFunc].Add(cmd);
                        }
                        else if (line.StartsWith("addbuff"))
                        {
                            string[] str = line.Split(XGlobalConfig.TabSeparator);

                            if (str.Length >= 4)
                            {
                                LevelCmdDesc cmd = new LevelCmdDesc();
                                cmd.cmd = LevelCmd.Level_Cmd_Addbuff;
                                cmd.Param.Add(str[1]);
                                cmd.Param.Add(str[2]);
                                cmd.Param.Add(str[3]);
                                _LevelScripts[curFunc].Add(cmd);
                            }
                        }
                        else if (line.StartsWith("removebuff"))
                        {
                            string[] str = line.Split(XGlobalConfig.TabSeparator);
                            if (str.Length >= 3)
                            {
                                LevelCmdDesc cmd = new LevelCmdDesc();
                                cmd.cmd = LevelCmd.Level_Cmd_Removebuff;
                                cmd.Param.Add(str[1]);
                                cmd.Param.Add(str[2]);
                                _LevelScripts[curFunc].Add(cmd);
                            }
                        }
                        else if (line.StartsWith("hidebillboard"))
                        {
                            string[] str = line.Split(XGlobalConfig.TabSeparator);
                            if (str.Length >= 3)
                            {
                                LevelCmdDesc cmd = new LevelCmdDesc();
                                cmd.cmd = LevelCmd.Level_Cmd_HideBillboard;
                                cmd.Param.Add(str[1]);
                                cmd.Param.Add(str[2]);
                                _LevelScripts[curFunc].Add(cmd);
                            }
                        }
                        else if (line.StartsWith("changebody"))
                        {
                            string[] str = line.Split(XGlobalConfig.TabSeparator);
                            if (str.Length >= 2)
                            {
                                LevelCmdDesc cmd = new LevelCmdDesc();
                                cmd.cmd = LevelCmd.Level_Cmd_ChangeBody;
                                cmd.Param.Add(str[1]);
                                _LevelScripts[curFunc].Add(cmd);
                            }
                        }
                        else if (line.StartsWith("justfx"))
                        {
                            string[] str = line.Split(XGlobalConfig.TabSeparator);
                            if (str.Length >= 2)
                            {
                                LevelCmdDesc cmd = new LevelCmdDesc();
                                cmd.cmd = LevelCmd.Level_Cmd_JustFx;
                                cmd.Param.Add(str[1]);
                                _LevelScripts[curFunc].Add(cmd);
                            }
                        }
                        else if (line.StartsWith("playfx"))
                        {
                            string[] str = line.Split(XGlobalConfig.TabSeparator);
                            if (str.Length >= 3)
                            {
                                LevelCmdDesc cmd = new LevelCmdDesc();
                                cmd.cmd = LevelCmd.Level_Cmd_PlayFx;
                                cmd.Param.Add(str[1]);
                                cmd.Param.Add(str[2]);
                                _LevelScripts[curFunc].Add(cmd);
                            }
                        }
                        else if (line.StartsWith("settutorial"))
                        {
                            string[] str = line.Split(XGlobalConfig.TabSeparator);
                            if (str.Length >= 2)
                            {
                                LevelCmdDesc cmd = new LevelCmdDesc();
                                cmd.cmd = LevelCmd.Level_Cmd_Tutorial;
                                cmd.Param.Add(str[1]);
                                _LevelScripts[curFunc].Add(cmd);
                            }
                        }
                        else if (line.StartsWith("notice"))
                        {
                            string[] str = line.Split(XGlobalConfig.TabSeparator);
                            if (str.Length >= 2)
                            {
                                LevelCmdDesc cmd = new LevelCmdDesc();
                                cmd.cmd = LevelCmd.Level_Cmd_Notice;
                                cmd.Param.Add(str[1]);
                                if (str.Length >= 3)
                                {
                                    cmd.Param.Add(str[2]);
                                }
                                _LevelScripts[curFunc].Add(cmd);
                            }
                        }
                        else if (line.StartsWith("stopnotice"))
                        {
                            LevelCmdDesc cmd = new LevelCmdDesc();
                            cmd.cmd = LevelCmd.Level_Cmd_StopNotice;
                            _LevelScripts[curFunc].Add(cmd);
                        }
                        else if (line.StartsWith("opendoor"))
                        {
                            string[] str = line.Split(XGlobalConfig.TabSeparator);
                            if (str.Length >= 2)
                            {
                                LevelCmdDesc cmd = new LevelCmdDesc();
                                cmd.cmd = LevelCmd.Level_Cmd_Opendoor;
                                cmd.Param.Add(str[1]);
                                if (str.Length > 2)
                                    cmd.Param.Add(str[2]);
                                _LevelScripts[curFunc].Add(cmd);
                            }
                        }
                        else if (line.StartsWith("killspawn"))
                        {
                            string[] str = line.Split(XGlobalConfig.TabSeparator);
                            if (str.Length >= 2)
                            {
                                LevelCmdDesc cmd = new LevelCmdDesc();
                                cmd.cmd = LevelCmd.Level_Cmd_KillSpawn;
                                cmd.Param.Add(str[1]);
                                _LevelScripts[curFunc].Add(cmd);
                            }
                        }
                        else if (line.StartsWith("killallspawn"))
                        {
                            LevelCmdDesc cmd = new LevelCmdDesc();
                            cmd.cmd = LevelCmd.Level_Cmd_KillAllSpawn;
                            _LevelScripts[curFunc].Add(cmd);
                        }
                        else if (line.StartsWith("killally"))
                        {
                            LevelCmdDesc cmd = new LevelCmdDesc();
                            cmd.cmd = LevelCmd.Level_Cmd_KillAlly;
                            _LevelScripts[curFunc].Add(cmd);
                        }
                        else if (line.StartsWith("killwave"))
                        {
                            string[] str = line.Split(XGlobalConfig.TabSeparator);
                            if (str.Length >= 2)
                            {
                                LevelCmdDesc cmd = new LevelCmdDesc();
                                cmd.cmd = LevelCmd.Level_Cmd_KillWave;
                                cmd.Param.Add(str[1]);
                                _LevelScripts[curFunc].Add(cmd);
                            }
                        }
                        else if (line.StartsWith("showcutscene"))
                        {
                            string[] str = line.Split(XGlobalConfig.TabSeparator);
                            if (str.Length >= 2)
                            {
                                LevelCmdDesc cmd = new LevelCmdDesc();
                                cmd.cmd = LevelCmd.Level_Cmd_Cutscene;
                                cmd.Param.Add(str[1]);
                                if (str.Length >= 6)
                                {
                                    cmd.Param.Add(str[2]);
                                    cmd.Param.Add(str[3]);
                                    cmd.Param.Add(str[4]);
                                    cmd.Param.Add(str[5]);
                                    if (str.Length >= 7)
                                    {
                                        cmd.Param.Add(str[6]);
                                    }
                                }
                                else if (str.Length == 3)
                                {
                                    cmd.Param.Add(str[2]);
                                }
                                _LevelScripts[curFunc].Add(cmd);
                            }
                        }
                        else if (line.StartsWith("levelupfx"))
                        {
                            LevelCmdDesc cmd = new LevelCmdDesc();
                            cmd.cmd = LevelCmd.Level_Cmd_LevelupFx;
                            _LevelScripts[curFunc].Add(cmd);
                        }
                        else if (line.StartsWith("continue_UI"))
                        {
                            LevelCmdDesc cmd = new LevelCmdDesc();
                            cmd.cmd = LevelCmd.Level_Cmd_Continue;
                            _LevelScripts[curFunc].Add(cmd);
                        }
                        else if (line.StartsWith("showskillslot"))
                        {
                            string[] str = line.Split(XGlobalConfig.TabSeparator);

                            if (str.Length >= 2)
                            {
                                LevelCmdDesc cmd = new LevelCmdDesc();
                                cmd.cmd = LevelCmd.Level_Cmd_ShowSkill;
                                cmd.Param.Add(str[1]);
                                _LevelScripts[curFunc].Add(cmd);
                            }
                        }
                        else if (line.StartsWith("bubble"))
                        {
                            string[] str = line.Split(XGlobalConfig.TabSeparator);
                            if (str.Length >= 4)
                            {
                                LevelCmdDesc cmd = new LevelCmdDesc();
                                cmd.cmd = LevelCmd.Level_Cmd_Bubble;
                                cmd.Param.Add(str[1]);
                                cmd.Param.Add(str[2]);
                                cmd.Param.Add(str[3]);
                                _LevelScripts[curFunc].Add(cmd);
                            }
                        }
                        else if (line.StartsWith("showdirection"))
                        {
                            string[] str = line.Split(XGlobalConfig.TabSeparator);
                            if (str.Length >= 2)
                            {
                                LevelCmdDesc cmd = new LevelCmdDesc();
                                cmd.cmd = LevelCmd.Level_Cmd_Direction;
                                cmd.Param.Add(str[1]);
                                _LevelScripts[curFunc].Add(cmd);
                            }
                        }
                        else if (line.StartsWith("outline"))
                        {
                            string[] str = line.Split(XGlobalConfig.TabSeparator);
                            if (str.Length >= 2)
                            {
                                LevelCmdDesc cmd = new LevelCmdDesc();
                                cmd.cmd = LevelCmd.Level_Cmd_Outline;
                                cmd.Param.Add(str[1]);
                                _LevelScripts[curFunc].Add(cmd);
                            }
                        }
                        else if (line.StartsWith("clientrecord"))
                        {
                            string[] str = line.Split(XGlobalConfig.TabSeparator);
                            if (str.Length >= 2)
                            {
                                LevelCmdDesc cmd = new LevelCmdDesc();
                                cmd.cmd = LevelCmd.Level_Cmd_Record;
                                cmd.Param.Add(str[1]);
                                _LevelScripts[curFunc].Add(cmd);
                            }
                        }
                        else if (line.StartsWith("callnewbiehelper"))
                        {
                            LevelCmdDesc cmd = new LevelCmdDesc();
                            cmd.cmd = LevelCmd.Level_Cmd_NewbieHelper;
                            _LevelScripts[curFunc].Add(cmd);
                        }
                        else if (line.StartsWith("newbienotice"))
                        {
                            string[] str = line.Split(XGlobalConfig.TabSeparator);
                            if (str.Length >= 2)
                            {
                                LevelCmdDesc cmd = new LevelCmdDesc();
                                cmd.cmd = LevelCmd.Level_Cmd_NewbieNotice;
                                cmd.Param.Add(str[1]);
                                _LevelScripts[curFunc].Add(cmd);
                            }
                        }
                        else if (line.StartsWith("summon"))
                        {
                            string[] str = line.Split(XGlobalConfig.TabSeparator);
                            if (str.Length >= 4)
                            {
                                LevelCmdDesc cmd = new LevelCmdDesc();
                                cmd.cmd = LevelCmd.Level_Cmd_Summon;
                                cmd.Param.Add(str[1]);
                                cmd.Param.Add(str[2]);
                                cmd.Param.Add(str[3]);
                                _LevelScripts[curFunc].Add(cmd);
                            }
                        }
                        else if (line.StartsWith("npcpopspeek"))
                        {
                            string[] str = line.Split(XGlobalConfig.TabSeparator);
                            if (str.Length >= 5)
                            {
                                LevelCmdDesc cmd = new LevelCmdDesc();
                                cmd.cmd = LevelCmd.Level_Cmd_NpcPopSpeek;
                                cmd.Param.Add(str[1]);
                                cmd.Param.Add(str[2]);
                                cmd.Param.Add(str[3]);
                                cmd.Param.Add(str[4]);
                                if (str.Length >= 6)
                                    cmd.Param.Add(str[5]);
                                _LevelScripts[curFunc].Add(cmd);
                            }
                            else
                            {
                                // XDebug.Log("level script error!" + line);
                            }
                        }
                        else if (line.StartsWith("aicommand"))
                        {
                            string[] str = line.Split(XGlobalConfig.TabSeparator);
                            if (str.Length >= 3)
                            {
                                LevelCmdDesc cmd = new LevelCmdDesc();
                                cmd.cmd = LevelCmd.Level_Cmd_SendAICmd;
                                cmd.Param.Add(str[1]);
                                cmd.Param.Add(str[2]);
                                if (str.Length >= 4)
                                    cmd.Param.Add(str[3]);
                                else
                                    cmd.Param.Add("0");

                                _LevelScripts[curFunc].Add(cmd);
                            }
                        }
                        else if (line.StartsWith("info"))
                        {
                            string[] str = line.Split(XGlobalConfig.TabSeparator);
                            string[] pos = str[1].Split(XGlobalConfig.ListSeparator);

                            XLevelInfo xInfo = new XLevelInfo();
                            xInfo.infoName = str[0].Substring(5);
                            xInfo.x = float.Parse(pos[0]);
                            xInfo.y = float.Parse(pos[1]);
                            xInfo.z = float.Parse(pos[2]);
                            xInfo.face = float.Parse(pos[3]);

                            if (str.Length >= 3) xInfo.enable = (str[2] == "on" ? true : false);
                            if (str.Length >= 4) xInfo.width = float.Parse(str[3]);
                            if (str.Length >= 5)
                                xInfo.height = float.Parse(str[4]);
                            else
                                xInfo.height = float.MaxValue;

                            if (str.Length >= 6)
                                xInfo.thickness = float.Parse(str[5]);

                            _LevelInfos.Add(xInfo);
                        }
                    }
                }
            }
            XResources.ClearStream(s);
        }

        public List<XLevelInfo> GetLevelScriptInfos()
        {
            return _LevelInfos;
        }

        public void Update()
        {
            if (_CmdQueue == null || _CmdQueue.Count == 0) return;

            if (_currentCmd == null || _currentCmd.state == XCmdState.Cmd_Finished)
            {
                _currentCmd = _CmdQueue.Count > 0 ? _CmdQueue[0] : null;
                if (_currentCmd == null)
                {
                    // XDebug.Log("level script end");
                }
                else
                {
                    _CmdQueue.RemoveAt(0);
                    CommandCount++;
                    Execute(_currentCmd);
                }
            }
        }

        public void ExecuteNextCmd()
        {
            if (_currentCmd != null)
                _currentCmd.state = XCmdState.Cmd_Finished;
            if (_CmdQueue.Count == 0)
            {
                _currentCmd = null;
                return;
            }

            _currentCmd = _CmdQueue.Count > 0 ? _CmdQueue[0] : null;
            if (_currentCmd == null)
            {
                // XDebug.Log("level script end");
            }
            else
            {
                _CmdQueue.RemoveAt(0);
                Execute(_currentCmd);
            }
        }

        public void Reset()
        {
            _externalString.Clear();
            _onceString.Clear();
            _CmdQueue.Clear();
            _currentCmd = null;
            _LevelScripts.Clear();
        }

        protected void Execute(LevelCmdDesc cmd)
        {
            switch (cmd.cmd)
            {
                case LevelCmd.Level_Cmd_TalkL:
                    {
                        //to-do Level_Cmd_TalkL

                        _currentCmd.state = XCmdState.Cmd_In_Process;
                    }
                    break;
                case LevelCmd.Level_Cmd_TalkR:
                    {
                        //to-do Level_Cmd_TalkR

                        _currentCmd.state = XCmdState.Cmd_In_Process;
                    }
                    break;
                case LevelCmd.Level_Cmd_Notalk:
                    {
                        //to-do Level_Cmd_Notalk
                        _currentCmd.state = XCmdState.Cmd_Finished;
                    }
                    break;
                case LevelCmd.Level_Cmd_Addbuff:
                    {
                        //to-do Level_Cmd_Addbuff

                        _currentCmd.state = XCmdState.Cmd_Finished;
                    }
                    break;
                case LevelCmd.Level_Cmd_Removebuff:
                    {
                        //to-do Level_Cmd_Removebuff

                        _currentCmd.state = XCmdState.Cmd_Finished;
                    }
                    break;
                case LevelCmd.Level_Cmd_HideBillboard:
                    {
                        //to-do Level_Cmd_HideBillboard

                    }
                    break;
                case LevelCmd.Level_Cmd_ChangeBody:
                    {
                        //XPlayer p = XEntityMgr.singleton.Player;
                        //if (p != null) p.OnTransform(uint.Parse(cmd.Param[0]));

                        _currentCmd.state = XCmdState.Cmd_Finished;
                    }
                    break;
                case LevelCmd.Level_Cmd_JustFx:
                    {
                        //to-do Level_Cmd_JustFx

                        _currentCmd.state = XCmdState.Cmd_Finished;
                    }
                    break;
                case LevelCmd.Level_Cmd_PlayFx:
                    {
                        //to-do Level_Cmd_PlayFx

                        _currentCmd.state = XCmdState.Cmd_Finished;
                    }
                    break;
                case LevelCmd.Level_Cmd_Tutorial:
                    {
                        //to-do Level_Cmd_Tutorial

                        _currentCmd.state = XCmdState.Cmd_Finished;
                    }
                    break;
                case LevelCmd.Level_Cmd_Continue:
                    {
                        //to-do Level_Cmd_Continue

                        _currentCmd.state = XCmdState.Cmd_Finished;

                    }
                    break;
                case LevelCmd.Level_Cmd_Notice:
                    {
                        //to-do Level_Cmd_Notice

                        if (_currentCmd != null)
                            _currentCmd.state = XCmdState.Cmd_Finished;

                    }
                    break;
                case LevelCmd.Level_Cmd_StopNotice:
                    {
                        //to-do Level_Cmd_StopNotice
                        if (_currentCmd != null)
                            _currentCmd.state = XCmdState.Cmd_Finished;
                    }
                    break;
                case LevelCmd.Level_Cmd_Opendoor:
                    {
                        //to-do Level_Cmd_Opendoor

                        _currentCmd.state = XCmdState.Cmd_Finished;
                    }
                    break;
                case LevelCmd.Level_Cmd_KillSpawn:
                    {
                        //to-do Level_Cmd_KillSpawn

                        _currentCmd.state = XCmdState.Cmd_Finished;
                    }
                    break;
                case LevelCmd.Level_Cmd_Bubble:
                    {
                        //to-do Level_Cmd_Bubble

                        _currentCmd.state = XCmdState.Cmd_Finished;
                    }
                    break;
                case LevelCmd.Level_Cmd_KillAllSpawn:
                    {
                        //XLevelFinishMgr.singleton.KillAllOpponent();
                        _currentCmd.state = XCmdState.Cmd_Finished;
                    }
                    break;
                case LevelCmd.Level_Cmd_KillAlly:
                    {
                        XDebug.LogError("找小邹: kill ally");
                        // XEntityMgr.singleton.KillAlly(XEntityMgr.singleton.Player);
                        _currentCmd.state = XCmdState.Cmd_Finished;
                    }
                    break;
                case LevelCmd.Level_Cmd_KillWave:
                    {
                        //to-do Level_Cmd_KillWave

                        _currentCmd.state = XCmdState.Cmd_Finished;
                    }
                    break;
                case LevelCmd.Level_Cmd_Cutscene:
                    {
                        //to-do Level_Cmd_Cutscene

                        _currentCmd.state = XCmdState.Cmd_Finished;
                    }
                    break;
                case LevelCmd.Level_Cmd_LevelupFx:
                    {
                        //to-do Level_Cmd_LevelupFx

                        _currentCmd.state = XCmdState.Cmd_Finished;
                    }
                    break;
                case LevelCmd.Level_Cmd_ShowSkill:
                    {
                        //to-do Level_Cmd_ShowSkill

                        _currentCmd.state = XCmdState.Cmd_Finished;
                    }
                    break;
                case LevelCmd.Level_Cmd_Direction:
                    {
                        //to-do Level_Cmd_Direction

                        _currentCmd.state = XCmdState.Cmd_Finished;
                    }
                    break;
                case LevelCmd.Level_Cmd_Outline:
                    {
                        //to-do Level_Cmd_Outline

                        _currentCmd.state = XCmdState.Cmd_Finished;
                    }
                    break;
                case LevelCmd.Level_Cmd_Record:
                    {
                        //to-do Level_Cmd_Record

                        _currentCmd.state = XCmdState.Cmd_Finished;
                    }
                    break;
                case LevelCmd.Level_Cmd_NewbieHelper:
                    {
                        //to-do Level_Cmd_NewbieHelper

                        if (_currentCmd != null) _currentCmd.state = XCmdState.Cmd_Finished;
                    }
                    break;
                case LevelCmd.Level_Cmd_NewbieNotice:
                    {
                        //to-do Level_Cmd_PlayFx

                        if (_currentCmd != null) _currentCmd.state = XCmdState.Cmd_In_Process;
                    }
                    break;
                case LevelCmd.Level_Cmd_NpcPopSpeek:
                    {
                        //to-do Level_Cmd_NpcPopSpeek

                        _currentCmd.state = XCmdState.Cmd_Finished;
                    }
                    break;
                case LevelCmd.Level_Cmd_SendAICmd:
                    {
                        //to-do Level_Cmd_SendAICmd

                        _currentCmd.state = XCmdState.Cmd_Finished;

                    }
                    break;
                case LevelCmd.Level_Cmd_Summon:
                    {
                        //to-do Level_Cmd_Summon

                        if (_currentCmd != null)
                            _currentCmd.state = XCmdState.Cmd_Finished;
                    }
                    break;
                default:
                    _currentCmd.state = XCmdState.Cmd_Finished;
                    break;
            }
        }

        public void SetExternalString(string str, bool bOnce)
        {
            if (bOnce)
            {
                if (_onceString.Contains(str)) return;
                _externalString.Add(str);
                _onceString.Add(str);
            }
            else
            {
                _externalString.Add(str);
            }
        }

        public bool IsTalkScript(string funcName)
        {
            if (!_LevelScripts.ContainsKey(funcName))
            {
                 XDebug.LogError("invalid script func");
                return false;
            }
            LevelCmdDesc top = _LevelScripts[funcName][0];
            if (top.cmd == LevelCmd.Level_Cmd_TalkL || top.cmd == LevelCmd.Level_Cmd_TalkR)
            {
                return true;
            }
            return false;
        }

        public bool QueryExternalString(string str, bool autoRemove)
        {
            bool bFind = false;
            foreach (string s in _externalString)
            {
                if (s == str)
                {
                    bFind = true;
                    break;
                }
            }
            if (bFind && autoRemove)
            {
                _externalString.Remove(str);
            }
            return bFind;
        }


        protected void SwitchWallState(string name, bool enabled)
        {
            for (int i = 0; i < _LevelInfos.Count; i++)
            {
                if (_LevelInfos[i].infoName == name)
                {
                    _LevelInfos[i].enable = enabled;
                }
            }
        }

        public void SyncWallState(string name, bool isOn)
        {
            SetClientWallState(name, isOn);
            SwitchWallState(name, isOn);
        }

        public void SetClientWallState(string name, bool isOn)
        {
            GameObject dynamicRoot = GameObject.Find(XScene.singleton.GetSceneDynamicPrefix());
            if (dynamicRoot == null) return;
            Transform go = XCommon.singleton.FindChildRecursively(dynamicRoot.transform, name);
            if (go == null)
            {
                 XDebug.LogError("Wall no exists: " , name);
                return;
            }
            go.gameObject.SetActive(isOn);
        }

    }
}
