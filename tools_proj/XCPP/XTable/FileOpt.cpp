#include "FileOpt.h"


void FileOpt::ReadTable()
{
	ReadTable("QteStatusList.bytes");
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