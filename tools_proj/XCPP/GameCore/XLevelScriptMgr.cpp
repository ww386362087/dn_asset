#include "XLevelScriptMgr.h"
#include "Log.h"
#include <assert.h>

XLevelScriptMgr::XLevelScriptMgr()
{
}

XLevelScriptMgr::~XLevelScriptMgr()
{
	delete _currentCmd;
	_currentCmd = NULL;
	for (size_t i = 0; i < _CmdQueue.size(); i++)
	{
		delete _CmdQueue[i];
	}
	_CmdQueue.clear();
	for (size_t i = 0; i < _LevelInfos.size(); i++)
	{
		delete _LevelInfos[i];
	}
	_LevelInfos.clear();
	for (unordered_map<string, vector<LevelCmdDesc*>>::iterator itr = _LevelScripts.begin();
		itr != _LevelScripts.end(); itr++)
	{
		for (size_t i = 0; i < itr->second.size(); i++)
		{
			delete itr->second[i];
		}
		itr->second.clear();
	}
	_LevelInfos.clear();
}

void XLevelScriptMgr::RunScript(string funcName)
{
	if (_LevelScripts.find(funcName) == _LevelScripts.end()) return;
	if (_CmdQueue.size() == 0)
	{
		delete _currentCmd;
		_currentCmd = NULL;
	}
	vector<LevelCmdDesc*> funcCmds = _LevelScripts[funcName];
	for (int i = 0; i < funcCmds.size(); i++)
	{
		funcCmds[i]->Reset();
		_CmdQueue.push_back(funcCmds[i]);
	}
	Update();
}

bool XLevelScriptMgr::IsCurrentCmdFinished()
{
	if (_currentCmd == NULL) return true;
	if (_currentCmd->state == Cmd_Finished) return true;
	return false;
}

void XLevelScriptMgr::ClearWallInfo()
{
	_LevelInfos.clear();
}

void XLevelScriptMgr::PreloadLevelScript(string file)
{
	Reset();
	ClearWallInfo();
	ifstream infile;
	infile.open(file.data(), ios::in);
	assert(infile.is_open());   
	string line;
	while (getline(infile, line))
	{
		//cout << s << endl;
		
	}
	infile.close();
}


void XLevelScriptMgr::HandlerCmd(string line)
{
	if (line.find("func:") == 0)
	{
		string func = line.substr(5);
		std::vector<LevelCmdDesc*> subQueue;
		_LevelScripts.insert(std::make_pair(func, subQueue));
		curFunc = func;
	}
	else if (line.find("talkl") == 0)
	{
		std::vector<string> str = split(line, '\t');
		LevelCmdDesc* cmd = new LevelCmdDesc();
		cmd->cmd = Level_Cmd_TalkL;
		cmd->Param.push_back(str[1]);
		cmd->Param.push_back(str[2]);
		if (str.size() > 3)
			cmd->Param.push_back(str[3]);
		_LevelScripts[curFunc].push_back(cmd);
	}
	else if (line.find("talkr") == 0)
	{
		std::vector<string> str = split(line, '\t');
		LevelCmdDesc* cmd = new LevelCmdDesc();
		cmd->cmd = Level_Cmd_TalkR;
		cmd->Param.push_back(str[1]);
		cmd->Param.push_back(str[2]);
		if (str.size() > 3)
			cmd->Param.push_back(str[3]);
		_LevelScripts[curFunc].push_back(cmd);
	}
	else if (line.find("hidebillboard"))
	{
		std::vector<string> str = split(line, '\t');
		if (str.size() >= 3)
		{
			LevelCmdDesc* cmd = new LevelCmdDesc();
			cmd->cmd = Level_Cmd_HideBillboard;
			cmd->Param.push_back(str[1]);
			cmd->Param.push_back(str[2]);
			_LevelScripts[curFunc].push_back(cmd);
		}
	}
	else if (line.find("playfx"))
	{
		std::vector<string> str = split(line, '\t');
		if (str.size() >= 3)
		{
			LevelCmdDesc* cmd = new LevelCmdDesc();
			cmd->cmd = Level_Cmd_PlayFx;
			cmd->Param.push_back(str[1]);
			cmd->Param.push_back(str[2]);
			_LevelScripts[curFunc].push_back(cmd);
		}
	}
	else if (line.find("opendoor"))
	{
		std::vector<string> str = split(line, '\t');
		if (str.size() >= 2)
		{
			LevelCmdDesc* cmd = new LevelCmdDesc();
			cmd->cmd = Level_Cmd_Opendoor;
			cmd->Param.push_back(str[1]);
			if (str.size() > 2)
				cmd->Param.push_back(str[2]);
			_LevelScripts[curFunc].push_back(cmd);
		}
	}
	else if (line.find("killspawn"))
	{
		std::vector<string> str = split(line, '\t');
		if (str.size() >= 2)
		{
			LevelCmdDesc* cmd = new LevelCmdDesc();
			cmd->cmd = Level_Cmd_KillSpawn;
			cmd->Param.push_back(str[1]);
			_LevelScripts[curFunc].push_back(cmd);
		}
	}
}

void XLevelScriptMgr::Update()
{
	if (_CmdQueue.size() == 0) return;

	if (_currentCmd == NULL || _currentCmd->state == Cmd_Finished)
	{
		_currentCmd = _CmdQueue.size() > 0 ? _CmdQueue.front() : NULL;
		if (_currentCmd != NULL)
		{
			_CmdQueue.pop_front();
			CommandCount++;
			Execute(_currentCmd);
		}
	}
}

void XLevelScriptMgr::ExecuteNextCmd()
{
	if (_currentCmd )
		_currentCmd->state = Cmd_Finished;
	if (_CmdQueue.size() == 0)
	{
		delete _currentCmd;
		_currentCmd = NULL;
		return;
	}

	_currentCmd = _CmdQueue.size() > 0 ? _CmdQueue.front() : NULL;
	if (_currentCmd)
	{
		_CmdQueue.pop_front();
		Execute(_currentCmd);
	}
}

void XLevelScriptMgr::Reset()
{
	_externalString.clear();
	_onceString.clear();
	_CmdQueue.clear();
	delete _currentCmd;
	_currentCmd = NULL;
	_LevelScripts.clear();
}

void XLevelScriptMgr::Execute(LevelCmdDesc* cmd)
{
	switch (cmd->cmd)
	{
	case Level_Cmd_TalkL:
	{
		//to-do Level_Cmd_TalkL

		_currentCmd->state = Cmd_In_Process;
	}
	break;
	case Level_Cmd_TalkR:
	{
	}
	break;
	case Level_Cmd_NpcPopSpeek:
		_currentCmd->state = Cmd_Finished;

		break;
	case Level_Cmd_SendAICmd:
		_currentCmd->state = Cmd_Finished;

		break;
	default:
		_currentCmd->state = Cmd_Finished;
		break;
	}
}

void XLevelScriptMgr::SetExternalString(string str, bool bOnce)
{
	if (bOnce)
	{
		for (size_t i = 0; i < _onceString.size(); i++)
		{
			if (_onceString[i] == str)
			{
				return;
			}
		}
		_externalString.push_back(str);
		_onceString.push_back(str);
	}
	else
	{
		_externalString.push_back(str);
	}
}

bool XLevelScriptMgr::IsTalkScript(string funcName)
{
	if (_LevelScripts.find(funcName) == _LevelScripts.end())
	{
		ERR("invalid script func");
		return false;
	}
	LevelCmdDesc* top = _LevelScripts[funcName][0];
	return top->cmd == Level_Cmd_TalkL || top->cmd == Level_Cmd_TalkR;
}

bool XLevelScriptMgr::QueryExternalString(string str, bool autoRemove)
{
	bool bFind = false;
	for (size_t i = 0; i < _externalString.size(); i++)
	{
		if (_externalString[i] == str && autoRemove)
		{
			_externalString.erase(_externalString.begin() + i);
			bFind = true;
			break;
		}
	}
	return bFind;
}