#include "NativeReader.h"
#include "Log.h"


NativeReader::~NativeReader(void)
{
	if(reader.is_open())
	{
		ERROR("你忘了在完成后调用close with the file！Closing it ...");
		reader.close();
	}
}

void NativeReader::Open(const char* path)
{
	LOG("Opening file: " + tostring(path));
    if(reader.is_open())reader.close();
	reader.imbue(std::locale("chs"));
    reader.open(path, std::ios::binary|std::ios::in);
    if(!reader.is_open())
    {
        ERROR("Could not open file for read: " + tostring(path));
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