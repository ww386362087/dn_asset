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
	b=this->ReadBoolean();
	buff=this->ReadString();
	b2= this->ReadBoolean();
	this->Close();
	cout<<"read num:"<<num<<" b:"<<b<<" b2:"<<b2<<" str:"<<buff<<endl;
}


void FileOpt::WriteBinary()
{
	cout<<"******** start write *********"<<endl;
	int num=20;
	string str("ÄãºÃ");
	bool b = true;
	bool b2 = false;
	this->Open("a.txt",IOMode::Write);
	this->Write(&num,sizeof(int));
	this->Write(&b,sizeof(bool));
	this->WriteString(str.c_str());
	this->Write(&b2,sizeof(bool));
	this->Close();
	cout<<"file out >"<<this->getPath()<<" con:"<<str.c_str()<< endl<<endl;
}


