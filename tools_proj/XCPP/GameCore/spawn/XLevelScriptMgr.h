#ifndef  __XLevelScriptMgr__
#define  __XLevelScriptMgr__

#include "../Singleton.h"
#include "../Common.h"
#include <unordered_map>
#include <vector>
#include <string>
#include <deque>
#include "LevelCmd.h"

using namespace std;

class XLevelScriptMgr :public Singleton<XLevelScriptMgr>
{

public:
	XLevelScriptMgr();
	~XLevelScriptMgr();
	void RunScript(string funcName);
	bool IsCurrentCmdFinished();
	void ClearWallInfo();
	void PreloadLevelScript(string file);
	void Update();
	void ExecuteNextCmd();
	void Reset();
	void Execute(LevelCmdDesc* cmd);
	void SetExternalString(string str, bool bOnce);
	bool IsTalkScript(string funcName);
	bool QueryExternalString(string str, bool autoRemove);

private:
	void HandlerCmd(string cmd);

public:
	uint CommandCount = 0;

private:
	std::deque<LevelCmdDesc*> _CmdQueue;
	LevelCmdDesc* _currentCmd;
	vector<string> 	_externalString;
	vector<string> _onceString;
	unordered_map<string, vector<LevelCmdDesc*>> _LevelScripts;
	vector<XLevelInfo*> _LevelInfos;
	string curFunc;
};

#endif