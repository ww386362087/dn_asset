#include "EquipSuit.h"

EquipSuit::EquipSuit(void)
{
	name = UNITY_STREAM_PATH + "Table/EquipSuit.bytes";
	ReadTable();
}

void EquipSuit::ReadTable()
{
	LOG("read:"+name);
	Open(name.c_str());
	long long filesize =0;
	int lineCnt = 0;
	Read(&filesize);
	Read(&lineCnt);
	m_data.clear();
	for(int i=0;i<lineCnt;i++)
	{
		EquipSuitRow *row = new EquipSuitRow();
		Read(&(row->suitid));
		ReadString(row->suitname);
		Read(&(row->level));
		Read(&(row->profid));
		Read(&(row->suitquality));
		Read(&(row->iscreate));
		ReadArray<int>(row->euipid);
		ReadString(row->effect1);
		ReadSeq(row->effect2);
		ReadSeq(row->effect3);
		ReadSeq(row->effect4);
		ReadSeq(row->effect5);
		ReadSeq(row->effect6);
		ReadSeq(row->effect7);
		ReadSeq(row->effect8);
		ReadSeq(row->effect9);
		ReadSeq(row->effect10);

		LOG("read id:"+tostring(row->suitid)
			+"\tlevel: "+tostring(row->level)
			+"\tlevel:"+tostring(row->level));
		m_data.push_back(*row);
	}
	this->Close();
}

void EquipSuit::GetRow(int id,EquipSuitRow* row)
{
	size_t len = m_data.size();
	for(size_t i=0;i<len;i++)
	{
		if(m_data[i].suitid==id)
		{
			*row = m_data[i];
		}
	}
}

int EquipSuit::GetLength()
{
	return (int)m_data.size();
}


extern "C"
{
	EquipSuit *equipsuit;

	int iGetEquipSuitLength()
	{
		if(equipsuit==NULL)
		{
			equipsuit = new EquipSuit();
		}
		return equipsuit->GetLength();
	}

	void iGetEquipSuitRow(int suitid,EquipSuitRow* row)
	{
		if(equipsuit==NULL)
		{
			equipsuit = new EquipSuit();
		}
		equipsuit->GetRow(suitid,row);
	}
}