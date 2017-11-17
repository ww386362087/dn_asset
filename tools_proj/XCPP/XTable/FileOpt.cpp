#include "FileOpt.h"
#include "NativeInterface.h"

void FileOpt::ReadTable()
{
	if(m_data.empty())
	{
		LOG("start read QteStatusList");
		string s="start read QteStatusList";
		XCommand(s.c_str());
		ReadTable("QteStatusList.bytes");
	}
	else
	{
		WARN("TABLE HAS READ, DON'T READ AGAIN");
	}
}

void FileOpt::ReadTable(string name)
{
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

	void  iReadTable()
	{
		FileOpt *f=new FileOpt();
			f->ReadTable();
	}

}