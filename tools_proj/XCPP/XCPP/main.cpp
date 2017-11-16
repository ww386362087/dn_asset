#include <iostream>
#include<Windows.h>
#include<time.h>
#include<string>

using namespace std;

typedef int(*DllInitial)();
typedef int(*DllAdd)(int,int);
typedef int(*DllSub)(int*,int*);
typedef void(*DllTable)();

void main()
{
	DllInitial init;
	DllAdd add;
	DllSub sub;
	DllTable tab;

	HINSTANCE hInst = LoadLibrary("XTable.dll");
	if(hInst==NULL)
	{
		cout<<"hInst is null"<<endl;
		return;
	}
	cout<<"load library succ"<<endl;
	init = (DllInitial)GetProcAddress(hInst,"iInitial");
	add = (DllAdd)GetProcAddress(hInst,"iAdd");
	sub = (DllSub)GetProcAddress(hInst,"iSub");
	tab = (DllTable)GetProcAddress(hInst,"iReadTable");

	int rest = init();
	int a = 4;
	int b = 2;
	int add_rst=add(a,b);
	int sub_rst=sub(&a,&b);
	cout<<"ex rst is:"<<rest<<endl;
	cout<<"cul add: "<<add_rst<<" sub_rst:"<<sub_rst<<endl;
	
	std::cout<<"start read table.."<<std::endl;
	tab();

	FreeLibrary(hInst);
	system("pause");
}