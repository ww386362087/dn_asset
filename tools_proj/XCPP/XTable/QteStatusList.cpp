#include "QteStatusList.h"

QteStatusList::QteStatusList(void)
{
	name = UNITY_STREAM_PATH + "Table/QteStatusList.bytes";
	ReadTable();
}

void QteStatusList::ReadTable()
{
	LOG(this->name);
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

int QteStatusList::GetLength()
{
	return (int)m_data.size();
}


extern "C"
{
	QteStatusList *qtestatuslist;

	int iGetQteStatueListLength()
	{
		if(qtestatuslist == NULL)
		{
			qtestatuslist = new QteStatusList();
		}
		return qtestatuslist->GetLength();
	}

	void iGetQteStatusListRow(int val,QteStatusListRow* row)
	{
		if(qtestatuslist == NULL)
		{
			qtestatuslist = new QteStatusList();
		}
		qtestatuslist->GetRow(val,row);
	}
}