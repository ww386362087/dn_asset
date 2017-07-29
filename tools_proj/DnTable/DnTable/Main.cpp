#define EXPORT __declspec (dllexport)  

char*  table_path="";

extern "C" bool EXPORT init(char* dir)
{
	table_path=dir;
	return true;
}


extern "C" int EXPORT add(int a,int b)
{
	return a+b;
}


void read(char* table)
{
}


void ReadAll(char* table[],int len)
{
	for(int i =0;i<len;i++)
	{
		read(table[i]);
	}
}


int main()
{
	return 1;
}