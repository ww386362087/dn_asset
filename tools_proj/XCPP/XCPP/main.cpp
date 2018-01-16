#include <iostream>
#include<Windows.h>
#include<time.h>
#include<string> 
#include<unordered_map>
#include<deque>

using namespace std;

typedef bool(*InitDef)(unsigned char command, const char*);
typedef void(*EntyDef)(int entityid, unsigned char method, unsigned int arg);
typedef void(*CompDef)(int entityid, unsigned char compnent, const char* arg);
typedef void(*EntySyncDef)(unsigned int entityid, unsigned char command, float* arr);
typedef void(*DllInit)(InitDef);
typedef void(*DllEntityCallback)(EntyDef);
typedef void(*DllCompCallback)(CompDef);
typedef void(*DllEntitySncCallback)(EntySyncDef);
typedef void(*DllInitial)(char*, char*);
typedef int(*DllAdd)(int, int);
typedef int(*DllSub)(int*, int*);
typedef int(*DllReadQteTable)();
typedef int(*DllReadSuitTable)();
typedef void(*DllReadJson)(const char*);
typedef void(*DllPatch)(const char*, const char*, const char*);
typedef void(*DllCallWithVoid)();
typedef void(*DllCallFloatWithVoid)(float);
typedef void(*DllGetRow)(int, void*);

class Node
{
public:
	int a;
	const char* s;
};

DllInit cb;
DllEntityCallback ent;
DllCompCallback com;
DllEntitySncCallback sync;
DllInitial init;
DllAdd add;
DllSub sub;
DllReadQteTable qte;
DllReadSuitTable suit;
DllReadJson json;
DllPatch patch;
DllCallWithVoid vect;
DllCallWithVoid start;
DllCallWithVoid stop;
DllCallWithVoid quit;
DllCallFloatWithVoid tick;
DllGetRow row;

void DebugInfo()
{
	cout << "** a-Add  s-Sub  t-Read j-Json  q-Quit **" << endl;
	cout << "** input your command:";
}

bool CheckIn()
{
	if (cin.fail())
	{
		cin.clear(); 
		cin.sync();  
		cout << "input format error" << endl << endl;
		return false;
	}
	return true;
}


void EAdd()
{
	/*int a, b, c;
	cout << "input a:";
	cin >> a;
	if (!CheckIn()) return;
	cout << "input b:";
	cin >> b;
	if (!CheckIn()) return;
	c = add(a, b);
	cout << "add result:" << c << endl << endl;*/

	cout << "long:" << sizeof(long) << " point:" << sizeof(void*) << endl;
	deque<int> que;
	que.push_back(2);
	que.push_back(4);
	que.push_back(8);
	cout << "que size1:" << que.size() << endl;
	cout << "front:" << que.front() << endl;
	cout << "back:" << que.back() << endl;
	que.pop_front();
	cout << "front:" << que.front() << endl;
	que.clear();
	cout << "que size2:" << que.size() << endl;
}

void ESub()
{
	int a, b, c;
	cout << "input a:";
	cin >> a;
	if (!CheckIn()) return;
	cout << "input b:";
	cin >> b;
	if (!CheckIn()) return;
	c = sub(&a, &b);
	cout << "add result:" << c << endl << endl;
}

void ERead()
{
	int len1 = qte();
	int len2 = suit();
	cout << "qtestatus table line cnt:" << len1 << endl;
	cout << "equipsuit table line cnt:" << len2 << endl;
	cout << endl << endl;
}

bool OnCallback(unsigned char type, const char* cont)
{
	switch (type)
	{
	case 'L':
		SetConsoleTextAttribute(GetStdHandle(STD_OUTPUT_HANDLE), FOREGROUND_INTENSITY | FOREGROUND_GREEN);
		cout << "[log]> " << cont << endl ;
		break;
	case 'W':
		SetConsoleTextAttribute(GetStdHandle(STD_OUTPUT_HANDLE), FOREGROUND_INTENSITY | FOREGROUND_RED | FOREGROUND_GREEN);
		cout << "[warn]> " << cont << endl ;
		break;
	case 'E':
		SetConsoleTextAttribute(GetStdHandle(STD_OUTPUT_HANDLE), FOREGROUND_INTENSITY | FOREGROUND_RED);
		cout << "[error]> " << cont << endl ;
		break;
	case 'G':
		SetConsoleTextAttribute(GetStdHandle(STD_OUTPUT_HANDLE), FOREGROUND_INTENSITY | FOREGROUND_BLUE);
		cout << "[load]> " << cont << endl << endl;
		break;
	case 'U':
		cout << "[unload]> " << cont << endl << endl;
		break;
	default:
		break;
	}
	SetConsoleTextAttribute(GetStdHandle(STD_OUTPUT_HANDLE), FOREGROUND_INTENSITY);
	return true;
}

void OnEnty(int entityid, unsigned char method, unsigned int arg)
{
	cout << "create entity: " << entityid << " arg:" << arg << endl;
}

void OnComp(int entityid, unsigned char compnent, const char* arg)
{
	cout << "call component method: " << entityid << " arg:" << arg << endl;
}

void OnSync(unsigned int entityid, unsigned char command, float* arr)
{
	cout << "sync transform: " << entityid << " arg:" << arr[0] << endl;
}

void main()
{
	cout << "**********  main **********" << endl;
	LPWSTR dll = TEXT("GameCore.dll");
	HINSTANCE hInst = LoadLibrary(dll);
	if (hInst == NULL)
	{
		cout << "hInst is null" << endl;
		return;
	}
	cout << "load library succ" << endl;
	cb = (DllInit)GetProcAddress(hInst, "iInitCallbackCommand");
	ent = (DllEntityCallback)GetProcAddress(hInst, "iInitEntityCall");
	com = (DllCompCallback)GetProcAddress(hInst, "iInitCompnentCall");
	sync = (DllEntitySncCallback)GetProcAddress(hInst, "iInitEntitySyncCall");
	init = (DllInitial)GetProcAddress(hInst, "iInitial");
	add = (DllAdd)GetProcAddress(hInst, "iAdd");
	sub = (DllSub)GetProcAddress(hInst, "iSub");
	qte = (DllReadQteTable)GetProcAddress(hInst, "iGetQteStatusListLength");
	suit = (DllReadSuitTable)GetProcAddress(hInst, "iGetEquipSuitLength");
	json = (DllReadJson)GetProcAddress(hInst, "iJson");
	patch = (DllPatch)GetProcAddress(hInst, "iPatch");
	vect = (DllCallWithVoid)GetProcAddress(hInst, "iVector");
	start = (DllCallWithVoid)GetProcAddress(hInst, "iStartCore");
	stop = (DllCallWithVoid)GetProcAddress(hInst, "iStopCore");
	quit = (DllCallWithVoid)GetProcAddress(hInst, "iQuitCore");
	tick = (DllCallFloatWithVoid)GetProcAddress(hInst, "iQuitCore");
	row = (DllGetRow)GetProcAddress(hInst, "iGetXEntityPresentationRow");
	cb(OnCallback);
	ent(OnEnty);
	com(OnComp);
	sync(OnSync);
	init("", "");

	while (true)
	{
		DebugInfo();
		char input;
		cin >> input;
		if (input >= 'A'&&input <= 'Z')
			input += 32;
		bool jump = false;
		switch (input)
		{
		case 'a':
			EAdd();
			break;
		case 's':
			ESub();
			break;
		case 't':
			ERead();
			break;
		case 'q':
			jump = true;
			break;
		case 'j':
			json("json.txt"); //json("PlayerAutoFight.txt");
			cout << endl << endl;
			break;
		case 'p':
			patch("D:/projects/dn_asset/Assets/StreamingAssets/Patch/old.txt",
				"D:/projects/dn_asset/Assets/StreamingAssets/Patch/diff.patch",
				"D:/projects/dn_asset/Assets/StreamingAssets/Patch/nex.txt");
			cout << "patch finish!" << endl << endl;
			break;
		case 'v':
			vect();
			break;
		case 'r':
			start();
			break;
		case 'o':
			stop();
			break;
		case 'u':
			quit();
			cout << "has quit game" << endl;
			break;
		case 'c':
			tick(0.4f);
			break;
		default:
			cout << "invalid command" << endl << endl;
		}
		if (jump) break;
	}

	FreeLibrary(hInst);
	system("pause");
}