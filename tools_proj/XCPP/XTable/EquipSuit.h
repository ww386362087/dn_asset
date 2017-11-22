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
	std::vector<int> euipid;
	char effect1[MaxStringSize];
	char effect2[MaxStringSize];
	char effect3[MaxStringSize];
	char effect4[MaxStringSize];
	char effect5[MaxStringSize];
	char effect6[MaxStringSize];
	char effect7[MaxStringSize];
	char effect8[MaxStringSize];
	char effect9[MaxStringSize];
	char effect10[MaxStringSize];
	int sortid;
};

class EquipSuit:public NativeReader
{
public:
	EquipSuit(void);
	void ReadTable();
	void GetRow(int val,EquipSuitRow* row);

protected:
	std::string name;
	std::vector<EquipSuitRow> m_data;
};


extern "C"
{
	ENGINE_INTERFACE_EXPORT void iReadEquipSuitList();
	ENGINE_INTERFACE_EXPORT void iGetEquipSuitRow(int suitid,EquipSuitRow* row);
};