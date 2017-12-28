#include "NativeReader.h"

NativeReader::~NativeReader(void)
{
	if(reader.is_open())
	{
		ERR("warn: not close stream. Closing it ...");
		reader.close();
	}
}

void NativeReader::Open(const char* path)
{
	LOG("Opening file: " + tostring(path));
    if(reader.is_open())reader.close();
    reader.open(path, std::ios::binary|std::ios::in);
    if(!reader.is_open())
    {
        ERR("Could not open file for read: " + tostring(path));
    }
}

void NativeReader::Close()
{
	if(reader.is_open()) reader.close();
}

void NativeReader::ReadString(char buff[])
{
	char len =0;
	Read(&len);
	memset(buff, 0, MaxStringSize * sizeof(char));
	int ptr=0;
    while(len > 0)
	{
		Read(&buff[ptr++]);
		len--;
	}
}


void NativeReader::ReadStringArray(char buff[MaxArraySize][MaxStringSize])
{
	char length = 0;
	Read(&length);
	memset(buff, 0, MaxStringSize* MaxArraySize * sizeof(char));
	for (int i = 0; i < length; i++)
	{
		char* ch = buff[i];
		ReadString(ch);
	}
}