#pragma once
#include<string>
#include<fstream>

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

	bool ReadBoolean();
	char ReadChar();
	int ReadInt();
	long long ReadInt64();
	float ReadFloat();
	string ReadString();
	virtual void ReadTable() = 0;

private:
    ifstream reader;
	string filePath;
};

