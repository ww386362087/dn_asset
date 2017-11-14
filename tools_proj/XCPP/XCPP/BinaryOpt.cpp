#include "BinaryOpt.h"
#include "Logger.h"

BinaryOpt::BinaryOpt(void)
{
	currentMode = None;
}

BinaryOpt::~BinaryOpt(void)
{
	if(writer.is_open())
	{
		ERROR("你忘了在完成文件后调用close");
		writer.close(); 
	}
	if(reader.is_open())
	{
		ERROR("你忘了在完成后调用close with the file！Closing it ...");
		reader.close();
	}
}

bool BinaryOpt::Open(string fileFullPath, IOMode mode)
{
	filePath = fileFullPath; 
	LOG("Opening file: " + filePath);
	if(mode == IOMode::Write)
    {
        currentMode = mode;
        if(writer.is_open()) writer.close();
        writer.open(filePath, ios::binary);
        if(!writer.is_open())
        {
            ERROR("Could not open file for write: " + filePath);
            currentMode = None;
        }
	}
	else if(mode == IOMode::Read)
    {
        currentMode = mode;
        if(reader.is_open())reader.close();
		reader.imbue(locale("chs"));
        reader.open(filePath, ios::binary|ios::in);
        if(!reader.is_open())
        {
            ERROR("Could not open file for read: " + filePath);
            currentMode = None;
        }
    }
    return currentMode != IOMode::None;
}

void BinaryOpt::Close()
{
	if(currentMode == IOMode::Write)
    {
		writer.close();
		currentMode=None;
    }
	else if(currentMode == IOMode::Read)
    {
         reader.close();
		 currentMode=None;
    }
}

bool BinaryOpt::CheckReadabilityStatus()
{
    if(currentMode != IOMode::Read)
    {
        WARN("Trying to write with a non Writable mode!");
        return false;
     }
     return true;
}

bool BinaryOpt::CheckWritabilityStatus()
{
    if(currentMode != IOMode::Write)
    {
        WARN("Trying to write with a non Writable mode!");
        return false;
     }
     return true;
}

void BinaryOpt::setPath(string str)
{
	this->filePath=str;
	LOG("set path" + str);
}

string BinaryOpt::getPath()
{
	if(this->filePath.empty()) ERROR("FILEPATH is null");
	return this->filePath;
}


// Generic write method that will write any value to a file (except a string,
// for strings use writeString instead).
void BinaryOpt::Write(void *value, size_t size)
{
      if(!CheckWritabilityStatus()) return;
     writer.write((const char*)value, size);
}

void BinaryOpt::WriteString(string str)
{ 
	if(!CheckWritabilityStatus())return;
    str += '\0';
    char* text = (char *)(str.c_str());
    unsigned long size = str.size();
    writer.write((const char *)text, size);
}

bool BinaryOpt::ReadBoolean()
{
	if(CheckReadabilityStatus())
	{
        bool value = false;
        BINARY_READ(reader, value);
        return value;
     }
	return false;
}

char BinaryOpt::ReadChar()
{
	if(CheckReadabilityStatus())
	{
        char value = 0;
        BINARY_READ(reader, value);
        return value;
     }
	return false;
}

int BinaryOpt::ReadInt()
{
	if(CheckReadabilityStatus())
	{
        int value = 0;
        BINARY_READ(reader, value);
        return value;
     }
	return 0;
}

long long BinaryOpt::ReadInt64()
{
	if(CheckReadabilityStatus())
	{
        long long value = 0;
        BINARY_READ(reader, value);
        return value;
     }
	return 0;
}

float BinaryOpt::ReadFloat()
{
	if(CheckReadabilityStatus())
	{
        float value = 0;
        BINARY_READ(reader, value);
        return value;
     }
	return 0;
}


string BinaryOpt::ReadString()
{
	if(CheckReadabilityStatus())
	{
       char c;
       string result = "";
	   int len = ReadChar();
	   //cout<<"string len:"<<len<<endl;
	   string tmp="";
       while(len > 0)
	   {
		   result += ReadChar();
		   len--;
	   }
	   return result;
    }
	return "";
}
