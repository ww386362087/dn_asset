#pragma once

#include "Common.h"
#include "QteStatusList.h"

void QteStatusList::ReadTable()
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

void QteStatusList::ReadTable(string name)
{
	LOG("read:"+name);
	this->Open(name);
	long long filesize =0;
	int lineCnt = 0;
	Read(&filesize);
	Read(&lineCnt);
	m_data.clear();
	for(int i=0;i<lineCnt;i++)
	{
		QteStatusListRow *row = new QteStatusListRow();
		this->ReadString(row->comment);
		this->ReadString(row->name);
		this->Read(&(row->val));
		LOG("read vlue:"+tostring(row->val));
		LOG("read comt:"+tostring(row->comment));
		LOG("read nam:"+tostring(row->name));
		m_data.push_back(*row);
	}
	this->Close();
}

void QteStatusList::GetRow(int val,QteStatusListRow* row)
{
	size_t len = m_data.size();
	for(size_t i=0;i<len;i++)
	{
		if(m_data[i].val==val)
		{
			*row = m_data[i];
		}
	}
}


extern "C"
{
	QteStatusList *f;

	void  iReadQteStatusList(const char* name)
	{
		f = new QteStatusList();
		f->ReadTable(name);

		QteStatusListRow* row=new QteStatusListRow();
		iGetQteStatusListRow(24,row);
		LOG("********************************");
		LOG("val:"+tostring(row->val)+" comment: "+tostring(row->comment)+" name: "+tostring(row->name));
	}

	void iGetQteStatusListRow(int val,QteStatusListRow* row)
	{
		if(f) f->GetRow(val,row);
	}
}