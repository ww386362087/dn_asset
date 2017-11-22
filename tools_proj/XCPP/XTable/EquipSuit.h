#pragma once
#include"NativeReader.h"
#include <vector>
#include"Log.h"

struct EquipSuitRow
{
	int suitid;
	char suitname[MaxStringSize];
	int level;
	int profid;
	int suitquality;
	bool iscreate;
	int euipid[MaxArraySize];
	char effect1[MaxStringSize];
	Seq<int> effect2;
	Seq<int> effect3;
	Seq<int> effect4;
	Seq<int> effect5;
	Seq<int> effect6;
	Seq<int> effect7;
	Seq<int> effect8;
	Seq<int> effect9;
	Seq<int> effect10;
	int sortid;
};

class EquipSuit:public NativeReader
{
public:
	EquipSuit(void);
	void ReadTable();
	void GetRow(int val,EquipSuitRow* row);
	int GetLength();

protected:
	std::string name;
	std::vector<EquipSuitRow> m_data;
};


extern "C"
{
	ENGINE_INTERFACE_EXPORT int iGetEquipSuitLength();
	ENGINE_INTERFACE_EXPORT void iGetEquipSuitRow(int suitid,EquipSuitRow* row);
};