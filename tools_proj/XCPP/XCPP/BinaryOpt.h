#pragma once
#include<string>
#include<fstream>

using namespace std;

#define BINARY_READ(reader,value) reader.read((char *)&value, sizeof(value))

enum IOMode
{
    None = 0,
    Read,
    Write
};


class BinaryOpt
{
public:
	BinaryOpt(void);
	~BinaryOpt(void);
	
	void Close();
	bool CheckReadabilityStatus();
	bool CheckWritabilityStatus();
	void setPath(string path);
	string getPath();
	bool Open(string fileFullPath, IOMode mode);

	void Write(void *value, size_t size);
	void WriteString(string);

	bool ReadBoolean();
	char ReadChar();
	int ReadInt();
	float ReadFloat();
	string ReadString();
private:
    ofstream writer;
    ifstream reader;
	string filePath;
    IOMode currentMode;
};

