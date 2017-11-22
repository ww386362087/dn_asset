#pragma once
#include<string>
#include<fstream>
#include<vector>
#include"Common.h"

#define BINARY_READ(reader,value) reader.read((char *)&value, sizeof(value))

class NativeReader
{
public:
	~NativeReader(void);
	void Close();
	void Open(const char* path);

	template<typename T> void Read(T* v);
	template<typename T> void ReadArray(std::vector<T>& v);
	void ReadString(char buff[]);

private:
    std::ifstream reader;
};


template<typename T>
void NativeReader::Read(T* v)
{
	BINARY_READ(reader, *v);
}

template <typename  T>
void NativeReader::ReadArray(std::vector<T>& v)
{
	char length = 0;
	Read(&length);
	if(length > 0)
	{
		v.clear();
		T val;
		for(int i = 0; i < length; i++)
		{
			Read(&val);
			v.push_back(val);
		}
	}
}