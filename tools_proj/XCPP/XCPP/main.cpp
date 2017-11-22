#include <iostream>
#include<Windows.h>
#include<time.h>
#include<string>

using namespace std;

typedef void(*CB)(const char*);
typedef void(*DllCommand)(CB);
typedef void(*DllInitial)(char*);
typedef int(*DllAdd)(int,int);
typedef int(*DllSub)(int*,int*);
typedef char*(*DllStr)();
typedef void(*DllTable)(const char*);
DllCommand cb;
DllInitial init;
DllAdd add;
DllSub sub;
DllStr str;
DllTable tab;

void DebugInfo()
{
	cout<<"********* op *********"<<endl;
	cout<<"** a stands for Add **"<<endl;
	cout<<"** s stands for Sub **"<<endl;
	cout<<"** t stands for Read *"<<endl;
	cout<<"** q stands for Quit *"<<endl;
	cout<<"** input your command:";
}

bool CheckIn()
{
	if(cin.fail()) 
	{
		cin.clear();  //Çå³ýfail×´Ì¬	
		cin.sync();   //Çå³ý»º³åÇø
		cout<<"input format error"<<endl<<endl;
		return false;
	}
	return true;
}

void ExAdd()
{
	int a,b,c;
	cout<<"input a:";
	cin>>a;
	if(!CheckIn()) return;
	cout<<"input b:";
	cin>>b;
	if(!CheckIn()) return;
	c=add(a,b);
	cout<<"add result:"<<c<<endl<<endl;
}

void ExSub()
{
	int a,b,c;
	cout<<"input a:";
	cin>>a;
	if(!CheckIn()) return;
	cout<<"input b:";
	cin>>b;
	if(!CheckIn()) return;
	c=sub(&a,&b);
	cout<<"add result:"<<c<<endl<<endl;
}


void ExRead()
{
	cout<<"input table name:";
	string name="QteStatusList.bytes";
	tab(name.c_str());
	cout<<endl<<endl;
}

void OnCallback(const char* command)
{
	cout<<"> "<<command<<endl;
}

void main()
{
	cout<<"**********  main **********"<<endl;

	HINSTANCE hInst = LoadLibrary("XTable.dll");
	if(hInst==NULL)
	{
		cout<<"hInst is null"<<endl;
		return;
	}
	cout<<"load library succ"<<endl;
	cb = (DllCommand)GetProcAddress(hInst,"iInitCallbackCommand");
	init = (DllInitial)GetProcAddress(hInst,"iInitial");
	add = (DllAdd)GetProcAddress(hInst,"iAdd");
	sub = (DllSub)GetProcAddress(hInst,"iSub");
	str = (DllStr)GetProcAddress(hInst,"GetStr");
	tab = (DllTable)GetProcAddress(hInst,"iReadTable");
	cb(OnCallback);
	init("");

	while(true)
	{
		DebugInfo();
		char input;
		cin>>input;
		bool jump =false;
		switch(input)
		{
			case 'a':
				ExAdd();
				break;
			case 's':
				ExSub();
				break;
			case 't':
				ExRead();
				break;
			case 'q':
				jump = true;
				break;
			case 'r':
				cout<< str()<<endl;
				break;
			default:
				cout<<"invalid command"<<endl<<endl;
		}
		if(jump)
			break;
	}
	
	FreeLibrary(hInst);
	system("pause");
}