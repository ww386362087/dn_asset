#pragma once
#include<string>
#include<fstream>
#include"Common.h"

using namespace std;

#define BINARY_READ(reader,value) reader.read((char *)&value, sizeof(value))

class NativeReader
{
public:
	NativeReader(void);
	~NativeReader(void);
	void Close();
	void setPath(string path);
	string getPath();
	void Open(string fileFullPath);

	template<typename T> void Read(T* v);
	void ReadString(char buff[]);

private:
    ifstream reader;
	string filePath;
};


template<typename T>
void NativeReader::Read(T* v)
{
	BINARY_READ(reader, *v);
}