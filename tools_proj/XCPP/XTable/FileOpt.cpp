#pragma once
#include "FileOpt.h"


void FileOpt::ReadTable()
{
	if(m_data.empty())
	{
		ReadTable("QteStatusList.bytes");
	}
	else
	{
		WARN("TABLE HAS READ, DON'T READ AGAIN");
	}
}

void FileOpt::ReadTable(string name)
{
	LOG("read:"+name);
	this->Open(name);
	long long filesize= this->ReadInt64();
	int lineCnt=this->ReadInt();
	m_data.clear();
	for(int i=0;i<lineCnt;i++)
	{
		FileRaw *row = new FileRaw();
		row->comment = this->ReadString();
		row->name = this->ReadString();
		row->val = this->ReadInt();
		LOG("read line: "+row->name+" "+row->comment);
		m_data.push_back(*row);
	}
	this->Close();
}


extern "C"
{

	void  iReadTable(const char* name)
	{
		FileOpt *f = new FileOpt();
		f->ReadTable(name);
	}

}