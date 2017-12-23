#ifndef __NativeReader__
#define __NativeReader__

#include <string>
#include <fstream>
#include <iostream>
#include <vector>
#include "Common.h"
#include "Log.h"

#define BINARY_READ(reader,value) reader.read((char *)&value, sizeof(value))

template<class T> struct Seq
{
	T value0;
	T value1;
};


class NativeReader
{
public:
	~NativeReader(void);
	void Close();
	void Open(const char* path);

	template<typename T> void Read(T* v);
	template<typename T> void ReadArray(T buff[]);
	template<typename T> void ReadSeq(Seq<T>& v);
	void ReadString(char buff[]);
	void ReadStringArray(char buff[MaxArraySize][MaxStringSize]);

private:
    std::ifstream reader;
};

template<typename T>
void NativeReader::Read(T* v)
{
	BINARY_READ(reader, *v);
}

template <typename T>
void NativeReader::ReadArray(T buff[])
{
	char length = 0;
	Read(&length);
	memset(buff, 0, MaxArraySize * sizeof(T));
	T val;
	for (int i = 0; i < length; i++)
	{
		Read(&val);
		buff[i] = val;
	}
}



template<typename T>
void NativeReader::ReadSeq(Seq<T>& v)
{
	Read(&v.value0);
	Read(&v.value1);
}

#endif