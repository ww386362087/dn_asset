#include "NativeReader.h"
#include "Log.h"

NativeReader::NativeReader(void) { }

NativeReader::~NativeReader(void)
{
	if(reader.is_open())
	{
		ERROR("你忘了在完成后调用close with the file！Closing it ...");
		reader.close();
	}
}

void NativeReader::Open(string fileFullPath)
{
	filePath = fileFullPath; 
	LOG("Opening file: " + filePath);
    if(reader.is_open())reader.close();
	reader.imbue(locale("chs"));
    reader.open(filePath, ios::binary|ios::in);
    if(!reader.is_open())
    {
        ERROR("Could not open file for read: " + filePath);
    }
}

void NativeReader::Close()
{
	if(reader.is_open())   reader.close();
}


void NativeReader::setPath(string str)
{
	this->filePath=str;
	LOG("set path" + str);
}

string NativeReader::getPath()
{
	if(this->filePath.empty()) ERROR("FILEPATH is null");
	return this->filePath;
}


void NativeReader::ReadString(char buff[])
{
	char len =0;
	Read(&len);
	memset(buff, 0, MaxStringSize * sizeof(char));
	int ptr=0;
    while(len > 0)
	{
		WChar c;
		Read(&buff[ptr++]);
		len--;
	}
}