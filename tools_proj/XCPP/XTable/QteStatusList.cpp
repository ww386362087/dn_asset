#include "QteStatusList.h"


QteStatusList::QteStatusList(void)
{
	name = UNITY_STREAM_PATH + "Table/QteStatusList.bytes";
}

void QteStatusList::ReadTable()
{
	LOG(this->name);
	LOG("Read");
	Open(name.c_str());
	long long filesize =0;
	int lineCnt = 0;
	Read(&filesize);
	Read(&lineCnt);
	m_data.clear();
	for(int i=0;i<lineCnt;i++)
	{
		QteStatusListRow *row = new QteStatusListRow();
		ReadString(row->comment);
		ReadString(row->name);
		Read(&(row->val));
		LOG("read vlue:"+tostring(row->val));
		LOG("read comt:"+tostring(row->comment));
		LOG("read nam:"+tostring(row->name));
		m_data.push_back(*row);
	}
	Close();
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
	QteStatusList *qtestatuslist;

	void  iReadQteStatusList()
	{
		if(qtestatuslist==NULL)
		{
			qtestatuslist = new QteStatusList();
		}
		qtestatuslist->ReadTable();
	}

	void iGetQteStatusListRow(int val,QteStatusListRow* row)
	{
		if(qtestatuslist) qtestatuslist->GetRow(val,row);
	}
}