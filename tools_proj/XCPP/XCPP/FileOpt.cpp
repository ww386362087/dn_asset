#include "FileOpt.h"
#include "Logger.h"


void FileOpt::ReadBinary()
{
	cout<<"******** start read *********"<<endl;
	int num ;
	string buff;
	bool b,b2;
	this->Open("a.txt",IOMode::Read);
	num = this->ReadInt();
	b = this->ReadBoolean();
	buff = this->ReadString();
	b2 = this->ReadBoolean();
	this->Close();
	cout<<"read num:"<<num<<" b:"<<b<<" b2:"<<b2<<" str:"<<buff<<endl;
}


void FileOpt::ReadTable()
{
	this->Open("QteStatusList.bytes",IOMode::Read);
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
