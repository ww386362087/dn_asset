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


bool NativeReader::ReadBoolean()
{
    bool value = false;
    BINARY_READ(reader, value);
    return value;
}

char NativeReader::ReadChar()
{
    char value = 0;
    BINARY_READ(reader, value);
    return value;
}

int NativeReader::ReadInt()
{
    int value = 0;
    BINARY_READ(reader, value);
    return value;
}

long long NativeReader::ReadInt64()
{
    long long value = 0;
    BINARY_READ(reader, value);
    return value;
}

float NativeReader::ReadFloat()
{
    float value = 0;
    BINARY_READ(reader, value);
    return value;
}

string NativeReader::ReadString()
{
    string result = "";
	int len = ReadChar();
	string tmp="";
    while(len > 0)
	{
		result += ReadChar();
		len--;
	}
	return result;
}
